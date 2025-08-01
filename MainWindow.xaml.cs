using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using NAudio.Midi;
using AudioVisualizer.Utils;

namespace AudioVisualizer
{
    public partial class MainWindow : Window
    {
        private VisualizerEngine visualizerEngine;
        private AudioEngine audioEngine;
        private DispatcherTimer animationTimer;
        private bool isAudioRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            visualizerEngine = new VisualizerEngine(visualCanvas);
            audioEngine = new AudioEngine(OnFFTData);

            midiButton.Click += (s, e) => visualizerEngine.ToggleMidiConnection(midiButton);
            audioButton.Click += ToggleAudio;

            animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            animationTimer.Tick += (s, e) => visualizerEngine.Update();
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
