using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace AudioVisualizer.Effects
{
    public class GlowEffect : ShaderEffect
    {
        private static PixelShader _shader = new PixelShader
        {
            UriSource = new Uri("/Shaders/GlowEffect.ps", UriKind.Relative)
        };

        public GlowEffect()
        {
            PixelShader = _shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(AmountProperty);
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GlowEffect), 0);

        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(double), typeof(GlowEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0)));

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
    }
}
