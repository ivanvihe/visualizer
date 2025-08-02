using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio.Midi;

namespace AudioVisualizer
{
    public class VisualizerEngine : IDisposable
    {
        private readonly Canvas canvas;
        private readonly Label tempoLabel;
        private readonly ComboBox modeComboBox;
        private readonly Slider sensitivitySlider, sizeSlider, cycleSpeedSlider, rotationSlider;
        private readonly CheckBox autoCycleCheckBox, randomShapeTypeCheckBox;
        private readonly ComboBox colorPaletteComboBox;

        private MidiIn? midiIn;
        private bool isMidiConnected = false;
        private int clockCount = 0;
        private DateTime lastBeat;

        private readonly DispatcherTimer autoCycleTimer;
        private readonly DispatcherTimer rotationTimer;
        private double currentRotation = 0;

        private Color[] currentPalette = new Color[] { Colors.LimeGreen };
        private Random rand = new Random();

        public VisualizerEngine(Canvas c, Label tl, ComboBox mode, Slider sens, Slider size, CheckBox autoCycle, Slider cycleSpeed, ComboBox colorPalette, Slider rotation, CheckBox randomShapeType)
        {
            canvas = c;
            tempoLabel = tl;
            modeComboBox = mode;
            sensitivitySlider = sens;
            sizeSlider = size;
            autoCycleCheckBox = autoCycle;
            cycleSpeedSlider = cycleSpeed;
            colorPaletteComboBox = colorPalette;
            rotationSlider = rotation;
            randomShapeTypeCheckBox = randomShapeType;

            autoCycleTimer = new DispatcherTimer();
            autoCycleTimer.Tick += (s, e) => CycleVisualizerMode();

            rotationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            rotationTimer.Tick += (s, e) => UpdateRotation();
            rotationTimer.Start();

            autoCycleCheckBox.Checked += (s, e) => UpdateAutoCycleTimer();
            autoCycleCheckBox.Unchecked += (s, e) => UpdateAutoCycleTimer();
            cycleSpeedSlider.ValueChanged += (s, e) => UpdateAutoCycleTimer();
            colorPaletteComboBox.SelectionChanged += (s, e) => UpdateColorPalette();
        }

        private void UpdateAutoCycleTimer()
        {
            if (autoCycleCheckBox.IsChecked == true)
            {
                autoCycleTimer.Interval = TimeSpan.FromSeconds(cycleSpeedSlider.Value);
                autoCycleTimer.Start();
            }
            else
            {
                autoCycleTimer.Stop();
            }
        }

        private void CycleVisualizerMode()
        {
            int nextIndex = (modeComboBox.SelectedIndex + 1) % modeComboBox.Items.Count;
            modeComboBox.SelectedIndex = nextIndex;
        }

        private void UpdateColorPalette()
        {
            string? selection = (colorPaletteComboBox.SelectedItem as ComboBoxItem)?.Content as string;
            switch (selection)
            {
                case "Synthwave":
                    currentPalette = new Color[] { Color.FromRgb(255, 105, 180), Color.FromRgb(0, 191, 255) }; // Hot Pink, Deep Sky Blue
                    break;
                case "Ocean":
                    currentPalette = new Color[] { Color.FromRgb(0, 255, 255), Color.FromRgb(0, 128, 128) }; // Cyan, Teal
                    break;
                case "Fire":
                    currentPalette = new Color[] { Color.FromRgb(255, 215, 0), Color.FromRgb(255, 69, 0) }; // Gold, OrangeRed
                    break;
                default: // Lime Green
                    currentPalette = new Color[] { Colors.LimeGreen };
                    break;
            }
        }

        private void UpdateRotation()
        {
            if (rotationSlider.Value == 0) return;
            currentRotation += rotationSlider.Value * 0.1;
            if (currentRotation > 360) currentRotation -= 360;
            canvas.RenderTransform = new RotateTransform(currentRotation, canvas.ActualWidth / 2, canvas.ActualHeight / 2);
        }

        public void OnAudioData(object? sender, AudioDataEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string selectedMode = (modeComboBox.SelectedItem as ComboBoxItem)?.Content as string ?? "Waveform";
                ClearAllVisuals();

                switch (selectedMode)
                {
                    case "Waveform":
                        DrawWaveform(args.WaveformData);
                        break;
                    case "Spectrum":
                        DrawSpectrum(args.SpectrumData);
                        break;
                    case "Circular Waveform":
                        DrawCircularWaveform(args.WaveformData);
                        break;
                    case "Circular Spectrum":
                        DrawCircularSpectrum(args.SpectrumData);
                        break;
                }
            });
        }

        private void DrawWaveform(float[] data)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            double height_2 = height / 2;
            double sensitivity = sensitivitySlider.Value;

            if (randomShapeTypeCheckBox.IsChecked == true)
            {
                for (int i = 0; i < data.Length; i += 10) // Draw fewer shapes for performance
                {
                    double x = (double)i / (data.Length - 1) * width;
                    double y = height_2 - (data[i] * height_2 * sensitivity);

                    Shape shape;
                    int shapeType = rand.Next(3);
                    if (shapeType == 0) // Line
                    {
                        shape = new Line { X1 = x, Y1 = height_2, X2 = x, Y2 = y };
                    }
                    else if (shapeType == 1) // Rectangle
                    {
                        double rectSize = sizeSlider.Value * 5;
                        shape = new Rectangle { Width = rectSize, Height = Math.Abs(y - height_2) };
                        Canvas.SetLeft(shape, x - rectSize / 2);
                        Canvas.SetTop(shape, Math.Min(y, height_2));
                    }
                    else // Ellipse
                    {
                        double ellipseSize = sizeSlider.Value * 5;
                        shape = new Ellipse { Width = ellipseSize, Height = ellipseSize };
                        Canvas.SetLeft(shape, x - ellipseSize / 2);
                        Canvas.SetTop(shape, y - ellipseSize / 2);
                    }
                    shape.Stroke = new SolidColorBrush(currentPalette[0]);
                    shape.Fill = new SolidColorBrush(currentPalette[0]) { Opacity = 0.5 };
                    shape.StrokeThickness = sizeSlider.Value;
                    canvas.Children.Add(shape);
                }
            }
            else
            {
                Polyline waveformLine = new Polyline(); // Create new instance
                waveformLine.Stroke = new SolidColorBrush(currentPalette[0]);
                waveformLine.StrokeThickness = sizeSlider.Value;
                PointCollection points = new PointCollection();

                for (int i = 0; i < data.Length; i++)
                {
                    double x = (double)i / (data.Length - 1) * width;
                    double y = height_2 - (data[i] * height_2 * sensitivity);
                    points.Add(new Point(x, y));
                }
                waveformLine.Points = points;
                canvas.Children.Add(waveformLine);
            }
        }

        private void DrawSpectrum(float[] data)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            double barWidth = width / data.Length;
            double sensitivity = sensitivitySlider.Value * 100;

            if (randomShapeTypeCheckBox.IsChecked == true)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    double barHeight = Math.Min(height, data[i] * sensitivity);
                    double x = i * barWidth;

                    Shape shape;
                    int shapeType = rand.Next(3);
                    if (shapeType == 0) // Line
                    {
                        shape = new Line { X1 = x + barWidth / 2, Y1 = height, X2 = x + barWidth / 2, Y2 = height - barHeight };
                    }
                    else if (shapeType == 1) // Rectangle
                    {
                        shape = new Rectangle { Width = barWidth, Height = barHeight };
                        Canvas.SetLeft(shape, x);
                        Canvas.SetTop(shape, height - barHeight);
                    }
                    else // Ellipse
                    {
                        double ellipseSize = Math.Min(barWidth, barHeight);
                        shape = new Ellipse { Width = ellipseSize, Height = ellipseSize };
                        Canvas.SetLeft(shape, x + (barWidth - ellipseSize) / 2);
                        Canvas.SetTop(shape, height - ellipseSize);
                    }
                    shape.Fill = new SolidColorBrush(GetColorForIndex(i));
                    shape.Stroke = new SolidColorBrush(GetColorForIndex(i)) { Opacity = 0.5 };
                    shape.StrokeThickness = sizeSlider.Value;
                    canvas.Children.Add(shape);
                }
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    Path spectrumPath = new Path(); // Create new instance
                    double barHeight = Math.Min(height, data[i] * sensitivity);
                    double x = i * barWidth;
                    spectrumPath.Data = new LineGeometry(new Point(x, height), new Point(x, height - barHeight));
                    spectrumPath.Stroke = new SolidColorBrush(GetColorForIndex(i));
                    spectrumPath.StrokeThickness = sizeSlider.Value;
                    canvas.Children.Add(spectrumPath);
                }
            }
        }

        private void DrawCircularWaveform(float[] data)
        {
            double centerX = canvas.ActualWidth / 2;
            double centerY = canvas.ActualHeight / 2;
            double baseRadius = 150;
            double sensitivity = sensitivitySlider.Value * 150;

            Path circularWaveformPath = new Path(); // Create new instance
            PathFigure figure = new PathFigure { IsClosed = true, StartPoint = new Point(centerX + baseRadius, centerY) };
            PathSegmentCollection segments = new PathSegmentCollection();

            for (int i = 0; i < data.Length; i++)
            {
                double angle = (double)i / data.Length * 2 * Math.PI;
                double radius = baseRadius + (data[i] * sensitivity);
                double x = centerX + Math.Cos(angle) * radius;
                double y = centerY + Math.Sin(angle) * radius;
                segments.Add(new LineSegment(new Point(x, y), true));
            }

            figure.Segments = segments;
            circularWaveformPath.Data = new PathGeometry(new[] { figure });
            circularWaveformPath.Stroke = new SolidColorBrush(currentPalette[0]);
            circularWaveformPath.StrokeThickness = sizeSlider.Value;
            canvas.Children.Add(circularWaveformPath);
        }

        private void DrawCircularSpectrum(float[] data)
        {
            double centerX = canvas.ActualWidth / 2;
            double centerY = canvas.ActualHeight / 2;
            double baseRadius = 100;
            double maxBarLength = 200;
            double sensitivity = sensitivitySlider.Value;

            for (int i = 0; i < data.Length; i++)
            {
                Path spectrumPath = new Path(); // Create new instance
                double angle = (double)i / data.Length * 2 * Math.PI;
                double barLength = data[i] * maxBarLength * sensitivity;

                double x1 = centerX + Math.Cos(angle) * baseRadius;
                double y1 = centerY + Math.Sin(angle) * baseRadius;
                double x2 = centerX + Math.Cos(angle) * (baseRadius + barLength);
                double y2 = centerY + Math.Sin(angle) * (baseRadius + barLength);

                spectrumPath.Data = new LineGeometry(new Point(x1, y1), new Point(x2, y2));
                spectrumPath.Stroke = new SolidColorBrush(GetColorForIndex(i));
                spectrumPath.StrokeThickness = sizeSlider.Value;
                canvas.Children.Add(spectrumPath);
            }
        }

        private Color GetColorForIndex(int index)
        {
            return currentPalette[index % currentPalette.Length];
        }

        private void ClearAllVisuals()
        {
            canvas.Children.Clear(); // Clear all children from the canvas
        }

        public void ConnectMidi(int deviceId, Button connectButton)
        {
            if (isMidiConnected)
            {
                DisconnectMidi();
                connectButton.Content = "Connect";
                return;
            }

            if (deviceId < 0 || deviceId >= MidiIn.NumberOfDevices) return;

            try
            {
                midiIn = new MidiIn(deviceId);
                midiIn.MessageReceived += OnMidiReceived;
                midiIn.Start();
                isMidiConnected = true;
                connectButton.Content = "Disconnect";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to MIDI device: " + ex.Message);
            }
        }

        private void DisconnectMidi()
        {
            midiIn?.Stop();
            midiIn?.Dispose();
            midiIn = null;
            isMidiConnected = false;
            tempoLabel.Content = "BPM: --";
        }

        private void OnMidiReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.RawMessage == 0xF8) // MIDI Clock
            {
                clockCount++;
                if (clockCount >= 24)
                {
                    clockCount = 0;
                    var now = DateTime.Now;
                    if (lastBeat != default)
                    {
                        double bpm = 60000.0 / (now - lastBeat).TotalMilliseconds;
                        Application.Current.Dispatcher.Invoke(() => tempoLabel.Content = $"BPM: {bpm:F0}");
                    }
                    lastBeat = now;
                }
            }
        }

        public void Dispose()
        {
            DisconnectMidi();
            rotationTimer.Stop();
            autoCycleTimer.Stop();
        }
    }
}
