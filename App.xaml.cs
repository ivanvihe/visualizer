using AudioVisualizer.Services;
using AudioVisualizer.ViewModels;
using AudioVisualizer.Views;
using System.Windows;

namespace AudioVisualizer
{
    public partial class App : Application
    {
        private AppViewModel? _appViewModel;
        private SettingsService _settingsService = new SettingsService();
        private AudioService _audioService = new AudioService();
        private MidiService _midiService = new MidiService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _appViewModel = new AppViewModel(_audioService, _midiService);
            _settingsService.LoadSettings(_appViewModel);

            var controlsWindow = new ControlsWindow { DataContext = _appViewModel };
            var visualWindow = new VisualWindow { DataContext = _appViewModel };

            controlsWindow.Show();
            visualWindow.Show();

            _audioService.Start();
            // MidiService will be started by AppViewModel based on saved settings
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_appViewModel != null)
            {
                _settingsService.SaveSettings(_appViewModel);
            }
            _audioService.Stop();
            _midiService.StopListening();
            base.OnExit(e);
        }
    }
}
