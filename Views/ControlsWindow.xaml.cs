using AudioVisualizer.Services;
using System.ComponentModel;
using System.Windows;

namespace AudioVisualizer.Views
{
    public partial class ControlsWindow : Window
    {
        public ControlsWindow()
        {
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            WindowPlacementService.LoadWindowPlacement(this, "ControlsWindow");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowPlacementService.SaveWindowPlacement(this, "ControlsWindow");
        }
    }
}
