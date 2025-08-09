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
    public partial class AudioGridVisual : UserControl
    {
        private DispatcherTimer? _timer;
        private AppViewModel? _viewModel;
        private int _gridSize = 20; // Number of cells in the grid

        public AudioGridVisual()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawGrid();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as AppViewModel;
            if (_viewModel == null) return;

            _viewModel.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(AppViewModel.GridSize))
                {
                    _gridSize = _viewModel.GridSize;
                    DrawGrid();
                }
            };

            DrawGrid();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) // 60 FPS
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void DrawGrid()
        {
            GridCanvas.Children.Clear();
            if (_viewModel == null || ActualWidth == 0 || ActualHeight == 0) return;

            double cellWidth = ActualWidth / _gridSize;
            double cellHeight = ActualHeight / _gridSize;

            for (int row = 0; row < _gridSize; row++)
            {
                for (int col = 0; col < _gridSize; col++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellWidth * 0.8, // Leave some space between cells
                        Height = cellHeight * 0.8,
                        Fill = Brushes.DarkGray
                    };
                    Canvas.SetLeft(rect, col * cellWidth + cellWidth * 0.1);
                    Canvas.SetTop(rect, row * cellHeight + cellHeight * 0.1);
                    GridCanvas.Children.Add(rect);
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_viewModel == null || ActualWidth == 0 || ActualHeight == 0) return;

            double totalReactivity = 0;
            if (_viewModel.SpectrumData != null && _viewModel.SpectrumData.Length > 0)
            {
                totalReactivity = _viewModel.SpectrumData.Sum() * _viewModel.Gain;
            }

            double cellWidth = ActualWidth / _gridSize;
            double cellHeight = ActualHeight / _gridSize;

            for (int i = 0; i < GridCanvas.Children.Count; i++)
            {
                if (GridCanvas.Children[i] is Rectangle rect)
                {
                    int row = i / _gridSize;
                    int col = i % _gridSize;

                    // Simple reaction: scale and color based on audio
                    double scale = 1 + (totalReactivity * _viewModel.GridReactivity);
                    rect.Width = cellWidth * 0.8 * scale;
                    rect.Height = cellHeight * 0.8 * scale;

                    // Center the scaled rectangle
                    Canvas.SetLeft(rect, col * cellWidth + (cellWidth - rect.Width) / 2);
                    Canvas.SetTop(rect, row * cellHeight + (cellHeight - rect.Height) / 2);

                    byte colorValue = (byte)Math.Min(255, 50 + (totalReactivity * 200));
                    rect.Fill = new SolidColorBrush(Color.FromRgb(colorValue, colorValue, colorValue));
                }
            }
        }
    }
}
