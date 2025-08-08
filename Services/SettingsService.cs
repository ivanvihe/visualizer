using AudioVisualizer.Model;
using AudioVisualizer.ViewModels;
using System;
using System.IO;
using System.Text.Json;

namespace AudioVisualizer.Services
{
    public class AppSettings
    {
        public double Gain { get; set; }
        public double Smoothing { get; set; }
        public int Bpm { get; set; }
        public bool AlwaysOnTop { get; set; }
        public string? SelectedPreset { get; set; }
        public string? SelectedMidiController { get; set; }
        public int ParticleCount { get; set; }
        public double ParticleSize { get; set; }
        public double ParticleSpeed { get; set; }
        public ParticleShape SelectedParticleShape { get; set; }
    }

    public class SettingsService
    {
        private readonly string _settingsPath;
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Visualizer");
            _settingsFilePath = Path.Combine(_settingsPath, "settings.json");
        }

        public void SaveSettings(AppViewModel viewModel)
        {
            var settings = new AppSettings
            {
                Gain = viewModel.Gain,
                Smoothing = viewModel.Smoothing,
                Bpm = viewModel.Bpm,
                AlwaysOnTop = viewModel.AlwaysOnTop,
                SelectedPreset = viewModel.SelectedPreset,
                SelectedMidiController = viewModel.SelectedMidiController,
                ParticleCount = viewModel.ParticleCount,
                ParticleSize = viewModel.ParticleSize,
                ParticleSpeed = viewModel.ParticleSpeed,
                SelectedParticleShape = viewModel.SelectedParticleShape
            };

            Directory.CreateDirectory(_settingsPath);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }

        public void LoadSettings(AppViewModel viewModel)
        {
            if (!File.Exists(_settingsFilePath)) return;

            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    viewModel.Gain = settings.Gain;
                    viewModel.Smoothing = settings.Smoothing;
                    viewModel.Bpm = settings.Bpm;
                    viewModel.AlwaysOnTop = settings.AlwaysOnTop;
                    viewModel.SelectedPreset = settings.SelectedPreset ?? "Particles";
                    viewModel.SelectedMidiController = settings.SelectedMidiController;
                    viewModel.ParticleCount = settings.ParticleCount;
                    viewModel.ParticleSize = settings.ParticleSize;
                    viewModel.ParticleSpeed = settings.ParticleSpeed;
                    viewModel.SelectedParticleShape = settings.SelectedParticleShape;
                }
            }
            catch (Exception)
            {
                // Failed to load settings, continue with defaults
            }
        }
    }
}
