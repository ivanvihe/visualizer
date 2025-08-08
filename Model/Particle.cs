using System;
using System.Windows;
using System.Windows.Media;

namespace AudioVisualizer.Model
{
    public class Particle
    {
        public Point Position { get; set; }
        public Vector Velocity { get; set; }
        public Color Color { get; set; }

        public Particle(Point position, Random random)
        {
            Position = position;
            Velocity = new Vector(random.NextDouble() * 2 - 1, random.NextDouble() * 2 - 1);
            Color = System.Windows.Media.Color.FromRgb((byte)random.Next(100, 255), (byte)random.Next(100, 255), (byte)random.Next(100, 255));
        }

        public void Update(Size bounds, double speed, double reactivity)
        {
            Position += Velocity * speed * (1 + reactivity);

            if (Position.X < 0 || Position.X > bounds.Width)
            {
                Velocity = new Vector(-Velocity.X, Velocity.Y);
                Position = new Point(Math.Clamp(Position.X, 0, bounds.Width), Position.Y);
            }
            if (Position.Y < 0 || Position.Y > bounds.Height)
            {
                Velocity = new Vector(Velocity.X, -Velocity.Y);
                Position = new Point(Position.X, Math.Clamp(Position.Y, 0, bounds.Height));
            }
        }
    }
}
