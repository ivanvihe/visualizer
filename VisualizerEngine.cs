using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NAudio.Midi;
using AudioVisualizer.Utils;

namespace AudioVisualizer
{
    public class VisualizerEngine : IDisposable
    {
        private readonly Canvas canvas;
        private readonly Random random = new Random();
        private readonly List<Shape> activeShapes = new();
        private MidiIn midiIn;
        private bool isConnectedToMidi = false;

        public VisualizerEngine(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void ToggleMidiConnection(Button button)
        {
            if (isConnectedToMidi)
            {
                DisconnectMidi();
                button.Content = "Connect MIDI";
                button.Background = Brushes.DarkSlateGray;
            }
            else
            {
                ConnectMidi();
                button.Content = "Disconnect MIDI";
                button.Background = Brushes.DarkGreen;
            }
        }

        private void ConnectMidi()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                var deviceInfo = MidiIn.DeviceInfo(i);
                if (deviceInfo.ProductName.ToLower().Contains("loop") || deviceInfo.ProductName.ToLower().Contains("virtual"))
                {
                    midiIn = new MidiIn(i);
                    midiIn.MessageReceived += OnMidiMessage;
                    midiIn.Start();
                    isConnectedToMidi = true;
                    return;
                }
            }
        }

        private void DisconnectMidi()
        {
            midiIn?.Stop();
            midiIn?.Dispose();
            midiIn = null;
            isConnectedToMidi = false;
        }

        private void OnMidiMessage(object sender, MidiInMessageEventArgs e)
        {
            var midiEvent = MidiEvent.FromRawMessage(e.RawMessage);
            if (midiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
            {
                var color = ColorUtils.HSVToRGB((noteOn.NoteNumber * 10) % 360, 0.8, noteOn.Velocity / 127.0);
                CreateShape(color, noteOn.Velocity / 127.0);
            }
        }

        public void RenderFFT(float[] fftData)
        {
            double maxHeight = canvas.ActualHeight;
            double barWidth = canvas.ActualWidth / fftData.Length;

            canvas.Children.Clear();
            for (int i = 0; i < fftData.Length; i++)
            {
                var magnitude = fftData[i] * 500;
                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = magnitude,
                    Fill = new SolidColorBrush(ColorUtils.HSVToRGB(i % 360, 0.8, 1)),
                    Opacity = 0.8
                };
                Canvas.SetLeft(rect, i * barWidth);
                Canvas.SetTop(rect, maxHeight - magnitude);
                canvas.Children.Add(rect);
            }
        }

        public void Update()
        {
            if (activeShapes.Count > 50)
            {
                canvas.Children.Remove(activeShapes[0]);
                activeShapes.RemoveAt(0);
            }
        }

        private void CreateShape(Color color, double intensity)
        {
            var shape = new Ellipse
            {
                Width = 20 + intensity * 100,
                Height = 20 + intensity * 100,
                Fill = new SolidColorBrush(color)
            };

            Canvas.SetLeft(shape, random.NextDouble() * (canvas.ActualWidth - 100));
            Canvas.SetTop(shape, random.NextDouble() * (canvas.ActualHeight - 100));

            canvas.Children.Add(shape);
            activeShapes.Add(shape);

            var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(3));
            fadeAnim.Completed += (s, e) =>
            {
                canvas.Children.Remove(shape);
                activeShapes.Remove(shape);
            };
            shape.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
        }

        public void Dispose()
        {
            DisconnectMidi();
        }
    }
}
