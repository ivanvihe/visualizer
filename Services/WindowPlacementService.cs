using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;

namespace AudioVisualizer.Services
{
    public static class WindowPlacementService
    {
        private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Visualizer");

        public static void SaveWindowPlacement(Window window, string name)
        {
            try
            {
                Directory.CreateDirectory(AppDataPath);
                var placement = GetPlacement(new WindowInteropHelper(window).Handle);
                var json = JsonSerializer.Serialize(placement);
                File.WriteAllText(Path.Combine(AppDataPath, $"{name}_placement.json"), json);
            }
            catch { /* Ignore errors */ }
        }

        public static void LoadWindowPlacement(Window window, string name)
        {
            try
            {
                var filePath = Path.Combine(AppDataPath, $"{name}_placement.json");
                if (!File.Exists(filePath)) return;

                var json = File.ReadAllText(filePath);
                var placement = JsonSerializer.Deserialize<WINDOWPLACEMENT>(json);
                SetPlacement(new WindowInteropHelper(window).Handle, placement);
            }
            catch { /* Ignore errors */ }
        }

        private static WINDOWPLACEMENT GetPlacement(IntPtr windowHandle)
        {
            GetWindowPlacement(windowHandle, out var placement);
            return placement;
        }

        private static void SetPlacement(IntPtr windowHandle, WINDOWPLACEMENT placement)
        {
            if (placement.showCmd == 2) // SW_SHOWMINIMIZED
            {
                placement.showCmd = 1; // SW_SHOWNORMAL
            }
            SetWindowPlacement(windowHandle, ref placement);
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rect rcNormalPosition;
        }
    }
}
