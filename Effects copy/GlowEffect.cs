using System;
using System.Windows;
using System.Windows.Media.Effects;

namespace AudioVisualizer.Effects
{
    public class GlowEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(GlowEffect), 0);

        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(double), typeof(GlowEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

        public GlowEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = new Uri("pack://application:,,,/AudioVisualizer;component/Shaders/GlowEffect.ps")
            };

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(AmountProperty);
        }

        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
    }
}
