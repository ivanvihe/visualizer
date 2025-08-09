using System;

namespace AudioVisualizer.Services
{
    public class AudioService
    {
        private readonly AudioEngine _audioEngine;

        public event EventHandler<AudioDataEventArgs>? AudioDataAvailable;

        public AudioService()
        {
            _audioEngine = new AudioEngine(spectrum => { }); // We'll handle data via the event
            _audioEngine.AudioDataAvailable += (s, e) => AudioDataAvailable?.Invoke(s, e);
        }

        public void Start()
        {
            _audioEngine.Start();
        }

        public void Stop()
        {
            _audioEngine.Stop();
        }
    }
}
