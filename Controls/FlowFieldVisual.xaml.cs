using AudioVisualizer.Model;
using AudioVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AudioVisualizer.Controls
{
    public partial class FlowFieldVisual : UserControl
    {
        private DispatcherTimer? _timer;
        private List<Particle> _particles = new List<Particle>();
        private Random _random = new Random();
        private AppViewModel? _viewModel;

        // Simple Perlin noise simulation (for demonstration)
        private double GetNoise(double x, double y, double z)
        {
            // In a real scenario, you'd use a proper Perlin noise library.
            // This is a very basic, non-optimized placeholder.
            return (Math.Sin(x * 0.1 + z) + Math.Cos(y * 0.1 + z)) / 2.0;
        }

        public FlowFieldVisual()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitializeParticles();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as AppViewModel;
            if (_viewModel == null) return;

            _viewModel.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == nameof(AppViewModel.ParticleCount) ||
                    a.PropertyName == nameof(AppViewModel.FlowFieldStrength))
                {
                    InitializeParticles();
                }
            };

            InitializeParticles();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) // 60 FPS
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void InitializeParticles()
        {
            if (_viewModel == null || ActualWidth == 0) return;

            _particles.Clear();
            for (int i = 0; i < _viewModel.ParticleCount; i++)
            {
                _particles.Add(new Particle(
                    new Point(_random.NextDouble() * ActualWidth, _random.NextDouble() * ActualHeight),
                    _random
                ));
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_viewModel == null || ActualWidth == 0) return;

            double reactivity = 0;
            if (_viewModel.SpectrumData != null && _viewModel.SpectrumData.Length > 0)
            {
                reactivity = _viewModel.SpectrumData.Sum() * _viewModel.Gain;
            }

            FlowCanvas.Children.Clear();

            foreach (var p in _particles)
            {
                // Calculate flow vector based on noise
                double angle = GetNoise(p.Position.X / 100.0, p.Position.Y / 100.0, Environment.TickCount / 1000.0) * Math.PI * 2;
                Vector flowVector = new Vector(Math.Cos(angle), Math.Sin(angle));

                // Apply flow and reactivity
                p.Velocity = (p.Velocity * (1 - _viewModel.FlowFieldStrength)) + (flowVector * _viewModel.FlowFieldStrength);
                p.Velocity.Normalize(); // Keep magnitude consistent

                p.Update(new Size(ActualWidth, ActualHeight), _viewModel.ParticleSpeed, reactivity);

                double size = _viewModel.ParticleSize + (reactivity * 50);
                Shape shape = CreateShapeForParticle(p, _viewModel.SelectedParticleShape, size);
                Canvas.SetLeft(shape, p.Position.X);
                Canvas.SetTop(shape, p.Position.Y);
                FlowCanvas.Children.Add(shape);
            }
        }

        private Shape CreateShapeForParticle(Particle p, ParticleShape shapeType, double size)
        {
            Shape shape;
            switch (shapeType)
            {
                case ParticleShape.Circle:
                    shape = new Ellipse { Width = size, Height = size };
                    break;
                case ParticleShape.Star:
                    shape = new Polygon { Points = CreateStarPoints(5, size / 2, size / 4) };
                    break;
                case ParticleShape.Hexagon:
                    shape = new Polygon { Points = CreatePolygonPoints(6, size / 2) };
                    break;
                default:
                    shape = new Rectangle { Width = size, Height = size };
                    break;
            }
            shape.Fill = new SolidColorBrush(p.Color);
            return shape;
        }

        private PointCollection CreatePolygonPoints(int sides, double radius)
        {
            var points = new PointCollection();
            double angleStep = 2 * Math.PI / sides;
            for (int i = 0; i < sides; i++)
            {
                double angle = i * angleStep;
                points.Add(new Point(radius * Math.Cos(angle), radius * Math.Sin(angle)));
            }
            return points;
        }

        private PointCollection CreateStarPoints(int numPoints, double outerRadius, double innerRadius)
        {
            var points = new PointCollection();
            double angleStep = Math.PI / numPoints;
            for (int i = 0; i < 2 * numPoints; i++)
            {
                double r = (i % 2 == 0) ? outerRadius : innerRadius;
                double angle = i * angleStep;
                points.Add(new Point(r * Math.Cos(angle), r * Math.Sin(angle)));
            }
            return points;
        }
    }
}
