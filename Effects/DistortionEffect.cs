using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AudioVisualizer.Effects
{
    public class DistortionEffect : ShaderEffect
    {
        private static PixelShader _shader = new PixelShader
        {
            UriSource = new Uri("/Shaders/DistortionEffect.ps", UriKind.Relative)
        };

        public DistortionEffect()
        {
            PixelShader = _shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(IntensityProperty);
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(DistortionEffect), 0);

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(double), typeof(DistortionEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty IntensityProperty =
            DependencyProperty.Register("Intensity", typeof(double), typeof(DistortionEffect),
                new UIPropertyMetadata(0.5, PixelShaderConstantCallback(1)));

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Time
        {
            get => (double)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public double Intensity
        {
            get => (double)GetValue(IntensityProperty);
            set => SetValue(IntensityProperty, value);
        }
    }
}
