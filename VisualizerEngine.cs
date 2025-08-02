using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NAudio.Midi;
using AudioVisualizer.Utils;

namespace AudioVisualizer
{
    public enum ShapeType { Circle, Rectangle, Triangle, Star }
    public enum TextureType { None, Lines, Grid, Dots }

    public class VisualizerEngine : IDisposable
    {
        public double Swing { get; set; }
        public int Density { get; set; }
        public double SizeModifier { get; set; }
        public double VolumeSensitivity { get; set; }
        public double Deformation { get; set; }
        public double ColorVariation { get; set; }
        public TextureType TextureType { get; set; }

        private Canvas canvas;
        private Label tempoLabel;
        private Random rand = new Random();

        // store enabled shapes
        private Dictionary<ShapeType, bool> shapeEnabled = new Dictionary<ShapeType, bool>
        {
            { ShapeType.Circle, true },
            { ShapeType.Rectangle, true },
            { ShapeType.Triangle, true },
            { ShapeType.Star, false }
        };

        public VisualizerEngine(Canvas visualCanvas, Label tempoLbl)
        {
            canvas = visualCanvas;
            tempoLabel = tempoLbl;
            Swing = 0;
            Density = 50;
            SizeModifier = 1;
            VolumeSensitivity = 0.5;
            Deformation = 0;
            ColorVariation = 1;
            TextureType = TextureType.None;
        }

        public void EnableShape(ShapeType type, bool enabled)
        {
            shapeEnabled[type] = enabled;
        }

        public void ToggleMidiConnection(Button button)
        {
            // existing code...
        }

        public void RenderFFT(float[] data)
        {
            // existing FFT drawing...
        }

        public void Update()
        {
            // existing per-frame update...
        }

        public void Dispose()
        {
            // cleanup...
        }
    }
}

