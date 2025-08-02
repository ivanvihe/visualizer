using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NAudio.Midi;

namespace AudioVisualizer
{
    public partial class MainWindow : Window
    {
        private VisualizerEngine visualizerEngine;
        private AudioEngine audioEngine;
        private bool isAudioRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            visualizerEngine = new VisualizerEngine(visualCanvas, tempoLabel, modeComboBox, sensitivitySlider, sizeSlider, autoCycleCheckBox, cycleSpeedSlider, colorPaletteComboBox, rotationSlider, randomShapeTypeCheckBox);
            audioEngine = new AudioEngine();
            audioEngine.AudioDataAvailable += visualizerEngine.OnAudioData;

            PopulateMidiDevices();
            connectMidiButton.Click += (s, e) => visualizerEngine.ConnectMidi(midiDeviceComboBox.SelectedIndex, connectMidiButton);
            audioButton.Click += ToggleAudio;
        }

        private void PopulateMidiDevices()
        {
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                midiDeviceComboBox.Items.Add(MidiIn.DeviceInfo(i).ProductName);
            }
            if (MidiIn.NumberOfDevices > 0)
            {
                midiDeviceComboBox.SelectedIndex = 0;
            }
        }

        private void ToggleAudio(object sender, RoutedEventArgs e)
        {
            if (isAudioRunning)
            {
                audioEngine.Stop();
                audioButton.Content = "Start Audio";
            }
            else
            {
                audioEngine.Start();
                audioButton.Content = "Stop Audio";
            }
            isAudioRunning = !isAudioRunning;
        }

        protected override void OnClosed(EventArgs e)
        {
            audioEngine.Dispose();
            visualizerEngine.Dispose();
            base.OnClosed(e);
        }
    }
}
