using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Veldrid;
using Veldrid.StartupUtilities;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace AudioVisualizer.Controls
{
    public partial class VeldridVisual : UserControl
    {
        private GraphicsDevice? _graphicsDevice;
        private CommandList? _commandList;
        private Swapchain? _swapchain;
        private Texture? _framebufferTexture;
        private WriteableBitmap? _writeableBitmap;
        private DispatcherTimer? _renderTimer;

        public VeldridVisual()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeVeldrid();
            _renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) };
            _renderTimer.Tick += OnRenderTimerTick;
            _renderTimer.Start();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _renderTimer?.Stop();
            DisposeVeldrid();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_graphicsDevice != null && _swapchain != null && ActualWidth > 0 && ActualHeight > 0)
            {
                ResizeSwapchain((uint)ActualWidth, (uint)ActualHeight);
            }
        }

        private void InitializeVeldrid()
        {
            if (_graphicsDevice != null) return; // Already initialized

            // Create GraphicsDevice (OpenGL backend)
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(new GraphicsDeviceOptions(), GraphicsBackend.OpenGL);

            // Create SwapchainSource from WPF Hwnd
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            SwapchainSource swapchainSource = SwapchainSource.CreateWin32(source.Handle, Marshal.GetHINSTANCE(typeof(App).Module));

            SwapchainDescription swapchainDescription = new SwapchainDescription(
                swapchainSource,
                (uint)ActualWidth,
                (uint)ActualHeight,
                Veldrid.PixelFormat.R8_G8_B8_A8_UNorm,
                false, // No depth buffer for now
                false // No sRGB for now
            );
            _swapchain = _graphicsDevice.ResourceFactory.CreateSwapchain(swapchainDescription);

            // Create CommandList
            _commandList = _graphicsDevice.ResourceFactory.CreateCommandList();

            // Initial resize to create framebuffer texture and bitmap
            ResizeSwapchain((uint)ActualWidth, (uint)ActualHeight);
        }

        private void ResizeSwapchain(uint width, uint height)
        {
            if (_graphicsDevice == null || _swapchain == null || width == 0 || height == 0) return;

            _swapchain.Resize(width, height);

            // Dispose old texture and bitmap if they exist
            _framebufferTexture?.Dispose();
            _writeableBitmap = null;

            // Create new framebuffer texture from swapchain
            _framebufferTexture = _swapchain.Framebuffer.ColorTargets[0].Target;

            // Create WriteableBitmap for WPF display
            _writeableBitmap = new WriteableBitmap(
                (int)width, (int)height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32, null);
            RenderImage.Source = _writeableBitmap;

            // Resize temp buffer for ReadTexture
            _tempBuffer = new byte[width * height * 4]; // 4 bytes per pixel (RGBA)
        }

        private void OnRenderTimerTick(object? sender, EventArgs e)
        {
            if (_graphicsDevice == null || _commandList == null || _swapchain == null || _writeableBitmap == null || _framebufferTexture == null) return;

            // Begin rendering
            _commandList.Begin();

            // Set framebuffer to swapchain's framebuffer
            _commandList.SetFramebuffer(_swapchain.Framebuffer);

            // Clear the screen with a dark background color
            _commandList.ClearColorTarget(0, new RgbaFloat(0.1f, 0.1f, 0.1f, 1.0f));

            // End rendering
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            _graphicsDevice.SwapBuffers(_swapchain);

            // Copy Veldrid texture to WriteableBitmap
            CopyTextureToBitmap(_framebufferTexture, _writeableBitmap);
        }

        private void CopyTextureToBitmap(Texture sourceTexture, WriteableBitmap targetBitmap)
        {
            if (_graphicsDevice == null || sourceTexture == null || targetBitmap == null) return;

            // Get texture data from GPU
            _graphicsDevice.ReadTexture(sourceTexture, _tempBuffer, 0, 0, 0, sourceTexture.Width, sourceTexture.Height, 0, 0);

            // Copy to WriteableBitmap
            targetBitmap.Lock();
            Marshal.Copy(_tempBuffer, 0, targetBitmap.BackBuffer, _tempBuffer.Length);
            targetBitmap.AddDirtyRect(new Int32Rect(0, 0, targetBitmap.PixelWidth, targetBitmap.PixelHeight));
            targetBitmap.Unlock();
        }

        private byte[] _tempBuffer = new byte[0]; // Temporary buffer for ReadTexture

        private void DisposeVeldrid()
        {
            _commandList?.Dispose();
            _swapchain?.Dispose();
            _graphicsDevice?.Dispose();
        }
    }
}
