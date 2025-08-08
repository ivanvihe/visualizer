using NAudio.Wave;
using NAudio.Dsp;
using System;

namespace AudioVisualizer
{
    public class AudioDataEventArgs : EventArgs
    {
        public float[] WaveformData { get; }
        public float[] SpectrumData { get; }
        public AudioDataEventArgs(float[] waveform, float[] spectrum) { WaveformData = waveform; SpectrumData = spectrum; }
    }

    public class AudioEngine : IDisposable
        {
            private const int FFT_SIZE = 1024;
            private const int WAVEFORM_POINTS = 512;

            private WaveInEvent? capture;
            private Complex[] fftBuffer = new Complex[FFT_SIZE];
            private float[] waveformBuffer = new float[WAVEFORM_POINTS];
            private int bufferPos = 0;
            private Action<float[]> renderFFTAction;

            public event EventHandler<AudioDataEventArgs>? AudioDataAvailable;

            public AudioEngine(Action<float[]> renderFFTAction)
            {
                this.renderFFTAction = renderFFTAction;
            }

            public void Start()
            {
                if (capture != null) return;
                capture = new WaveInEvent { WaveFormat = new WaveFormat(44100, 1) };
                capture.DataAvailable += OnDataAvailable;
                capture.StartRecording();
            }

            public void Stop()
            {
                capture?.StopRecording();
                capture?.Dispose();
                capture = null;
            }

            private void OnDataAvailable(object? sender, WaveInEventArgs e)
            {
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    float sample = BitConverter.ToInt16(e.Buffer, i) / 32768f;

                    // FFT buffer
                    fftBuffer[bufferPos].X = sample;
                    fftBuffer[bufferPos].Y = 0;

                    // Waveform buffer (simple circular buffer)
                    waveformBuffer[bufferPos % WAVEFORM_POINTS] = sample;

                    bufferPos++;

                    if (bufferPos >= FFT_SIZE)
                    {
                        ProcessBuffers();
                        bufferPos = 0;
                    }
                }
            }

            private void ProcessBuffers()
            {
                // Process FFT
                var fftBufferCopy = new Complex[FFT_SIZE];
                Array.Copy(fftBuffer, fftBufferCopy, FFT_SIZE);
                FastFourierTransform.FFT(true, (int)Math.Log(FFT_SIZE, 2), fftBufferCopy);
                float[] spectrum = new float[FFT_SIZE / 2];
                for (int j = 0; j < spectrum.Length; j++)
                    spectrum[j] = (float)Math.Sqrt(fftBufferCopy[j].X * fftBufferCopy[j].X + fftBufferCopy[j].Y * fftBufferCopy[j].Y);

                // Prepare waveform data
                float[] finalWaveform = new float[WAVEFORM_POINTS];
                Array.Copy(waveformBuffer, finalWaveform, WAVEFORM_POINTS);

                AudioDataAvailable?.Invoke(this, new AudioDataEventArgs(finalWaveform, spectrum));
                renderFFTAction?.Invoke(spectrum);
            }

            public void Dispose()
            {
                Stop();
            }
        }
}