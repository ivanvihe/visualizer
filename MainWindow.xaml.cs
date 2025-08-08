using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AudioVisualizer.Utils;
using NAudio.Midi;
using System.Linq;
using System.Configuration;

namespace AudioVisualizer
{
    public partial class MainWindow : Window
    {
        private VisualizerEngine visualizerEngine;
        private AudioEngine audioEngine;
        private DispatcherTimer updateTimer;
        private bool audioRunning = false;
        private int selectedMidiDevice = -1;

        public MainWindow()
        {
            InitializeComponent();
            visualizerEngine = new VisualizerEngine(visualCanvas, tempoLabel);
            audioEngine = new AudioEngine(visualizerEngine.RenderFFT);

            PopulateMidiDevices();
            midiDeviceComboBox.SelectionChanged += MidiDeviceComboBox_SelectionChanged;

            

            midiButton.Click += (s, e) => visualizerEngine.ToggleMidiConnection(midiButton);
            audioButton.Click += ToggleAudio;
            swingSlider.ValueChanged += (s, e) =>
            {
                swingLabel.Text = $"Swing: {(int)swingSlider.Value}%";
                visualizerEngine.Swing = swingSlider.Value / 100.0;
            };

            shapeCircleCheck.Checked += (s, e) => visualizerEngine.EnableShape(ShapeType.Circle, true);
            shapeCircleCheck.Unchecked += (s, e) => visualizerEngine.EnableShape(ShapeType.Circle, false);
            shapeRectCheck.Checked += (s, e) => visualizerEngine.EnableShape(ShapeType.Rectangle, true);
            shapeRectCheck.Unchecked += (s, e) => visualizerEngine.EnableShape(ShapeType.Rectangle, false);
            shapeTriCheck.Checked += (s, e) => visualizerEngine.EnableShape(ShapeType.Triangle, true);
            shapeTriCheck.Unchecked += (s, e) => visualizerEngine.EnableShape(ShapeType.Triangle, false);
            shapeStarCheck.Checked += (s, e) => visualizerEngine.EnableShape(ShapeType.Star, true);
            shapeStarCheck.Unchecked += (s, e) => visualizerEngine.EnableShape(ShapeType.Star, false);

            densitySlider.ValueChanged += (s, e) =>
            {
                densityLabel.Text = ((int)densitySlider.Value).ToString();
                visualizerEngine.Density = (int)densitySlider.Value;
            };

            sizeSlider.ValueChanged += (s, e) =>
            {
                sizeLabel.Text = $"{sizeSlider.Value:F1}x";
                visualizerEngine.SizeModifier = sizeSlider.Value;
            };

            volumeSlider.ValueChanged += (s, e) =>
            {
                volumeLabel.Text = volumeSlider.Value.ToString("F2");
                visualizerEngine.VolumeSensitivity = volumeSlider.Value;
            };

            deformSlider.ValueChanged += (s, e) =>
            {
                deformLabel.Text = deformSlider.Value.ToString("F2");
                visualizerEngine.Deformation = deformSlider.Value;
            };

            colorSlider.ValueChanged += (s, e) =>
            {
                colorLabel.Text = colorSlider.Value.ToString("F2");
                visualizerEngine.ColorVariation = colorSlider.Value;
            };

            textureCombo.SelectionChanged += (s, e) =>
            {
                visualizerEngine.TextureType = (TextureType)textureCombo.SelectedIndex;
            };

            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            updateTimer.Tick += (s, e) => visualizerEngine.Update();
            updateTimer.Start();
        }

        private void PopulateMidiDevices()
        {
            midiDeviceComboBox.Items.Clear();
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                midiDeviceComboBox.Items.Add(MidiIn.DeviceInfo(i).ProductName);
            }
            if (midiDeviceComboBox.Items.Count > 0)
            {
                midiDeviceComboBox.SelectedIndex = 0; // Select the first device by default
            }
        }

        private void MidiDeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMidiDevice = midiDeviceComboBox.SelectedIndex;
        }

        private void ToggleAudio(object sender, RoutedEventArgs e)
        {
            if (audioRunning)
            {
                audioEngine.Stop();
                audioButton.Content = "Start Audio";
            }
            else
            {
                audioEngine.Start();
                audioButton.Content = "Stop Audio";
            }
            audioRunning = !audioRunning;
        }
    }
}