using AudioVisualizer.Model;
using AudioVisualizer.Services;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AudioVisualizer.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private double _gain = 0.5;
        private double _smoothing = 0.5;
        private int _bpm = 120;
        private bool _alwaysOnTop;
        private string _selectedPreset = "Particles";
        private string? _selectedMidiController;
        private int _particleCount = 100;
        private double _particleSize = 5;
        private double _particleSpeed = 2;
        private ParticleShape _selectedParticleShape = ParticleShape.Circle;
        private float[]? _spectrumData;

        // New properties for FlowFieldVisual
        private double _flowFieldStrength = 0.5;

        // New properties for AudioGridVisual
        private int _gridSize = 20;
        private double _gridReactivity = 0.5;

        // New properties for WavePatternVisual
        private double _waveAmplitude = 50;
        private double _waveFrequency = 0.05;

        private readonly MidiService _midiService;

        public double Gain { get => _gain; set => SetProperty(ref _gain, value); }
        public double Smoothing { get => _smoothing; set => SetProperty(ref _smoothing, value); }
        public int Bpm { get => _bpm; set => SetProperty(ref _bpm, value); }
        public bool AlwaysOnTop { get => _alwaysOnTop; set { SetProperty(ref _alwaysOnTop, value); OnAlwaysOnTopChanged(); } }
        public string SelectedPreset { get => _selectedPreset; set => SetProperty(ref _selectedPreset, value); }
        public string? SelectedMidiController
        {
            get => _selectedMidiController;
            set
            {
                if (SetProperty(ref _selectedMidiController, value))
                {
                    _midiService.StartListening(value);
                }
            }
        }
        public int ParticleCount { get => _particleCount; set => SetProperty(ref _particleCount, value); }
        public double ParticleSize { get => _particleSize; set => SetProperty(ref _particleSize, value); }
        public double ParticleSpeed { get => _particleSpeed; set => SetProperty(ref _particleSpeed, value); }
        public ParticleShape SelectedParticleShape { get => _selectedParticleShape; set => SetProperty(ref _selectedParticleShape, value); }
        public float[]? SpectrumData { get => _spectrumData; private set => SetProperty(ref _spectrumData, value); }

        // New properties for FlowFieldVisual
        public double FlowFieldStrength { get => _flowFieldStrength; set => SetProperty(ref _flowFieldStrength, value); }

        // New properties for AudioGridVisual
        public int GridSize { get => _gridSize; set => SetProperty(ref _gridSize, value); }
        public double GridReactivity { get => _gridReactivity; set => SetProperty(ref _gridReactivity, value); }

        // New properties for WavePatternVisual
        public double WaveAmplitude { get => _waveAmplitude; set => SetProperty(ref _waveAmplitude, value); }
        public double WaveFrequency { get => _waveFrequency; set => SetProperty(ref _waveFrequency, value); }

        public ObservableCollection<string> Presets { get; } = new ObservableCollection<string> { "Particles", "Flow Field", "Audio Grid", "Wave Pattern", "OpenGL Visual", "Veldrid Visual" };
        public ObservableCollection<string> MidiControllers { get; } = new ObservableCollection<string>();
        public ObservableCollection<ParticleShape> ParticleShapes { get; } = new ObservableCollection<ParticleShape>((ParticleShape[])Enum.GetValues(typeof(ParticleShape)));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? AlwaysOnTopChanged;

        public AppViewModel(AudioService audioService, MidiService midiService)
        {
            _midiService = midiService;
            LoadMidiDevices();
            audioService.AudioDataAvailable += OnAudioDataAvailable;

            // Start MIDI listening with the initially selected device (from settings or default)
            _midiService.StartListening(SelectedMidiController);
        }

        private void OnAudioDataAvailable(object? sender, AudioDataEventArgs e)
        {
            SpectrumData = e.SpectrumData;
        }

        private void LoadMidiDevices()
        {
            MidiControllers.Add("None");
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                MidiControllers.Add(MidiIn.DeviceInfo(i).ProductName);
            }
            SelectedMidiController = MidiControllers.FirstOrDefault();
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private void OnAlwaysOnTopChanged()
        {
            AlwaysOnTopChanged?.Invoke();
        }
    }
}
