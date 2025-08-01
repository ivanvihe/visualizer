using System;
using NAudio.Wave;
using NAudio.Dsp;

namespace AudioVisualizer
{
    public class AudioEngine
    {
        private WasapiLoopbackCapture capture;
        private readonly Action<float[]> fftCallback;
        private const int FFT_SIZE = 1024;
        private readonly Complex[] fftBuffer = new Complex[FFT_SIZE];
        private int bufferIndex = 0;

        public AudioEngine(Action<float[]> callback)
        {
            fftCallback = callback;
        }

        public void Start()
        {
            capture = new WasapiLoopbackCapture();
            capture.DataAvailable += OnDataAvailable;
            capture.StartRecording();
        }

        public void Stop()
        {
            if (capture != null)
            {
                capture.StopRecording();
                capture.Dispose();
                capture = null;
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = new float[e.BytesRecorded / 4];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            foreach (var sample in buffer)
            {
                fftBuffer[bufferIndex].X = (float)(sample * FastFourierTransform.HannWindow(bufferIndex, FFT_SIZE));
                fftBuffer[bufferIndex].Y = 0;
                bufferIndex++;

                if (bufferIndex >= FFT_SIZE)
                {
                    bufferIndex = 0;
                    var fftCopy = new Complex[FFT_SIZE];
                    fftBuffer.CopyTo(fftCopy, 0);

                    FastFourierTransform.FFT(true, (int)Math.Log(FFT_SIZE, 2.0), fftCopy);

                    var magnitude = new float[FFT_SIZE / 2];
                    for (int i = 0; i < magnitude.Length; i++)
                    {
                        magnitude[i] = (float)Math.Sqrt(fftCopy[i].X * fftCopy[i].X + fftCopy[i].Y * fftCopy[i].Y);
                    }

                    fftCallback(magnitude);
                }
            }
        }
    }
}
