using System;
using System.Windows;
using System.Windows.Media.Effects;

namespace AudioVisualizer.Effects
{
    public class DistortionEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(DistortionEffect), 0);

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(double), typeof(DistortionEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty IntensityProperty =
            DependencyProperty.Register("Intensity", typeof(double), typeof(DistortionEffect),
                new UIPropertyMetadata(0.5, PixelShaderConstantCallback(1)));

        public DistortionEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = new Uri("pack://application:,,,/AudioVisualizer;component/Shaders/DistortionEffect.ps")
            };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(IntensityProperty);
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
