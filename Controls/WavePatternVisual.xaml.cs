using AudioVisualizer.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AudioVisualizer.Controls
{
    public partial class WavePatternVisual : UserControl
    {
        private DispatcherTimer? _timer;
        private AppViewModel? _viewModel;
        private Polyline _waveLine = new Polyline();

        public WavePatternVisual()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            WaveCanvas.Children.Add(_waveLine);
            _waveLine.Stroke = Brushes.Cyan;
            _waveLine.StrokeThickness = 2;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawWave();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as AppViewModel;
            if (_viewModel == null) return;

            _viewModel.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(AppViewModel.WaveAmplitude) ||
                    a.PropertyName == nameof(AppViewModel.WaveFrequency))
                {
                    DrawWave();
                }
            };

            DrawWave();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) // 60 FPS
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void DrawWave()
        {
            if (_viewModel == null || ActualWidth == 0 || ActualHeight == 0) return;

            _waveLine.Points.Clear();
            double centerY = ActualHeight / 2;

            for (int i = 0; i < ActualWidth; i++)
            {
                double x = i;
                double y = centerY + Math.Sin(i * _viewModel.WaveFrequency) * _viewModel.WaveAmplitude;
                _waveLine.Points.Add(new Point(x, y));
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_viewModel == null || ActualWidth == 0 || ActualHeight == 0) return;

            double reactivity = 0;
            if (_viewModel.SpectrumData != null && _viewModel.SpectrumData.Length > 0)
            {
                reactivity = _viewModel.SpectrumData.Sum() * _viewModel.Gain;
            }

            // Update wave based on audio reactivity
            _waveLine.Points.Clear();
            double centerY = ActualHeight / 2;
            double timeOffset = Environment.TickCount / 100.0; // Animate wave over time

            for (int i = 0; i < ActualWidth; i++)
            {
                double x = i;
                double y = centerY + Math.Sin(i * _viewModel.WaveFrequency + timeOffset) * (_viewModel.WaveAmplitude + reactivity * 100);
                _waveLine.Points.Add(new Point(x, y));
            }
        }
    }
}
