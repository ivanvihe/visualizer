using NAudio.Midi;
using System;
using System.Linq;

namespace AudioVisualizer.Services
{
    public class MidiService : IDisposable
    {
        private MidiIn? _midiIn;
        public event EventHandler<MidiInMessageEventArgs>? MidiMessageReceived;

        public void StartListening(string? deviceName)
        {
            StopListening();

            if (string.IsNullOrEmpty(deviceName) || deviceName == "None")
            {
                return;
            }

            int deviceId = -1;
            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                if (MidiIn.DeviceInfo(i).ProductName == deviceName)
                {
                    deviceId = i;
                    break;
                }
            }

            if (deviceId != -1)
            {
                _midiIn = new MidiIn(deviceId);
                _midiIn.MessageReceived += (sender, e) => MidiMessageReceived?.Invoke(sender, e);
                _midiIn.Start();
            }
        }

        public void StopListening()
        {
            if (_midiIn != null)
            {
                _midiIn.Stop();
                _midiIn.Dispose();
                _midiIn = null;
            }
        }

        public void Dispose()
        {
            StopListening();
        }
    }
}
