using AudioVisualizer.Services;
using AudioVisualizer.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AudioVisualizer.Views
{
    public partial class VisualWindow : Window
    {
        public VisualWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AppViewModel oldVm)
            {
                oldVm.AlwaysOnTopChanged -= UpdateTopmost;
            }
            if (e.NewValue is AppViewModel newVm)
            {
                newVm.AlwaysOnTopChanged += UpdateTopmost;
                UpdateTopmost();
            }
        }

        private void UpdateTopmost()
        {
            if (DataContext is AppViewModel vm)
            {
                this.Topmost = vm.AlwaysOnTop;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Maximize_Click(sender, e);
            }
            else
            {
                DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            WindowPlacementService.LoadWindowPlacement(this, "VisualWindow");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WindowPlacementService.SaveWindowPlacement(this, "VisualWindow");
        }
    }
}
