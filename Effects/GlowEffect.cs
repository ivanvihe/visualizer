using System;
using System.Windows;
using System.Windows.Media.Effects;

namespace AudioVisualizer.Effects
{
    public class GlowEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(GlowEffect), 0);

sampler2D input : register(s0);
float amount : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    float glow = (color.r + color.g + color.b) / 3.0;
    glow = pow(glow, 2.0) * amount;
    return color + float4(glow, glow, glow, 0);
}
