# visualizer
Visualizador WPF reactivo a Audio (FFT) y MIDI con efectos Shader (Glow + Distortion).

## ✅ Características
- Análisis FFT en tiempo real usando NAudio.
- Soporte MIDI (notas → formas dinámicas).
- Efectos gráficos avanzados con Pixel Shaders:
  - Glow
  - Distortion (wave)
- UI simple (Start Audio, Connect MIDI).

## ▶️ Ejecución
1. Instala dependencias:
   ```powershell
   dotnet add package NAudio --version 2.2.1
