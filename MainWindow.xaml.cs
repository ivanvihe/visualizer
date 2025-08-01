using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AudioVisualizer.Effects;

namespace AudioVisualizer
{
    public partial class MainWindow : Window
    {
        private VisualizerEngine visualizerEngine;
        private AudioEngine audioEngine;
        private DispatcherTimer animationTimer;

        private DistortionEffect distortionEffect;
        private GlowEffect glowEffect;
        private double timeValue = 0;
        private double lastVolume = 0;
        private bool isAudioRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            visualizerEngine = new VisualizerEngine(visualCanvas);
            audioEngine = new AudioEngine(OnFFTData);

            midiButton.Click += (s, e) => visualizerEngine.ToggleMidiConnection(midiButton);
            audioButton.Click += ToggleAudio;

            // Inicializamos efectos
            distortionEffect = new DistortionEffect { Intensity = 0.5 };
            glowEffect = new GlowEffect { Amount = 1.0 };

            visualCanvas.Effect = distortionEffect;

            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            animationTimer.Tick += (s, e) =>
            {
                visualizerEngine.Update();
                timeValue += 0.016;
                distortionEffect.Time = timeValue;
                glowEffect.Amount = Math.Min(3, lastVolume * 3);
            };
            animationTimer.Start();
        }

        private void ToggleAudio(object sender, RoutedEventArgs e)
        {
            if (isAudioRunning)
            {
                audioEngine.Stop();
                audioButton.Content = "Start Audio";
                isAudioRunning = false;
            }
            else
            {
                audioEngine.Start();
                audioButton.Content = "Stop Audio";
                isAudioRunning = true;
            }
        }

        private void OnFFTData(float[] fftBuffer)
        {
            Dispatcher.Invoke(() =>
            {
                visualizerEngine.RenderFFT(fftBuffer);
                lastVolume = fftBuffer.Average();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            audioEngine.Stop();
            visualizerEngine.Dispose();
            base.OnClosed(e);
        }
    }
}
