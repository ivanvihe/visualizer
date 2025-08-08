using AudioVisualizer.Services;
using AudioVisualizer.ViewModels;
using AudioVisualizer.Views;
using System.Windows;

namespace AudioVisualizer
{
    public partial class App : Application
    {
        private AppViewModel _appViewModel;
        private SettingsService _settingsService;
        private ControlsWindow _controlsWindow;
        private VisualWindow _visualWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _appViewModel = new AppViewModel();
            _settingsService = new SettingsService();

            _settingsService.LoadSettings(_appViewModel);

            _controlsWindow = new ControlsWindow { DataContext = _appViewModel };
            _visualWindow = new VisualWindow { DataContext = _appViewModel };

            _controlsWindow.Show();
            _visualWindow.Show();
        }

        private void Application_Shutdown(object sender, ShutdownEventArgs e)
        {
            _settingsService.SaveSettings(_appViewModel);
        }
    }
}
