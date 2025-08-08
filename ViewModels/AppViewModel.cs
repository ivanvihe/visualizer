using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioVisualizer.Model;

namespace AudioVisualizer.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private double _gain = 0.5;
        private double _smoothing = 0.5;
        private int _bpm = 120;
        private bool _alwaysOnTop;
        private string _selectedPreset = "Particles";
        private string _selectedMidiController;
        private int _particleCount = 100;
        private double _particleSize = 5;
        private double _particleSpeed = 2;
        private ParticleShape _selectedParticleShape = ParticleShape.Circle;

        public double Gain { get => _gain; set => SetProperty(ref _gain, value); }
        public double Smoothing { get => _smoothing; set => SetProperty(ref _smoothing, value); }
        public int Bpm { get => _bpm; set => SetProperty(ref _bpm, value); }
        public bool AlwaysOnTop { get => _alwaysOnTop; set { SetProperty(ref _alwaysOnTop, value); OnAlwaysOnTopChanged(); } }
        public string SelectedPreset { get => _selectedPreset; set => SetProperty(ref _selectedPreset, value); }
        public string? SelectedMidiController { get => _selectedMidiController; set => SetProperty(ref _selectedMidiController, value); }
        public int ParticleCount { get => _particleCount; set => SetProperty(ref _particleCount, value); }
        public double ParticleSize { get => _particleSize; set => SetProperty(ref _particleSize, value); }
        public double ParticleSpeed { get => _particleSpeed; set => SetProperty(ref _particleSpeed, value); }
        public ParticleShape SelectedParticleShape { get => _selectedParticleShape; set => SetProperty(ref _selectedParticleShape, value); }

        public ObservableCollection<string> Presets { get; } = new ObservableCollection<string> { "Particles", "Spectrum" };
        public ObservableCollection<string> MidiControllers { get; } = new ObservableCollection<string> { "None", "MIDI Device 1", "MIDI Device 2" };
        public ObservableCollection<ParticleShape> ParticleShapes { get; } = new ObservableCollection<ParticleShape>((ParticleShape[])Enum.GetValues(typeof(ParticleShape)));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? AlwaysOnTopChanged;

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAlwaysOnTopChanged()
        {
            AlwaysOnTopChanged?.Invoke();
        }
    }
}
