using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;

namespace Veldrid.Tests.Android
{
    public class VeldridSurfaceView : SurfaceView, ISurfaceHolderCallback
    {
        private readonly GraphicsBackend _backend;

        protected GraphicsDeviceOptions DeviceOptions { get; }

        public GraphicsDevice? GraphicsDevice { get; protected set; }
        public Swapchain? MainSwapchain { get; protected set; }

        public event Action? OnDeviceCreated;
        public event Action? OnDeviceDestroyed;
        public event Action? OnSwapchainChanged;
        public event Action? OnSwapchainDestroyed;

        public VeldridSurfaceView(Context context, GraphicsBackend backend, GraphicsDeviceOptions deviceOptions) : base(context)
        {
            if (!(backend == GraphicsBackend.Vulkan || backend == GraphicsBackend.OpenGLES))
            {
                throw new NotSupportedException($"{backend} is not supported on Android.");
            }

            _backend = backend;
            DeviceOptions = deviceOptions;
            Holder!.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            HandleSurfaceDestroyed();
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            Surface? surface = holder.Surface;
            if (surface != null)
            {
                HandleSurfaceChanged(surface, format, width, height);
            }
        }

        private void HandleSurfaceChanged(Surface surface, Format format, int width, int height)
        {
            SwapchainSource ss = SwapchainSource.CreateAndroidSurface(surface.Handle, JNIEnv.Handle);
            SwapchainDescription sd = new(
                ss,
                (uint)width,
                (uint)height,
                DeviceOptions.SwapchainDepthFormat,
                DeviceOptions.SyncToVerticalBlank);

            if (GraphicsDevice == null)
            {
                if (_backend == GraphicsBackend.Vulkan)
                {
                    GraphicsDevice = GraphicsDevice.CreateVulkan(DeviceOptions, sd);
                }
                else
                {
                    GraphicsDevice = GraphicsDevice.CreateOpenGLES(DeviceOptions, sd);
                }

                MainSwapchain = GraphicsDevice.MainSwapchain;
                OnDeviceCreated?.Invoke();
            }
            else
            {
                MainSwapchain?.Resize(sd.Width, sd.Height);
            }

            OnSwapchainChanged?.Invoke();
        }

        private void HandleSurfaceDestroyed()
        {
            if (MainSwapchain != null)
            {
                MainSwapchain.Dispose();
                OnSwapchainDestroyed?.Invoke();
                MainSwapchain = null!;
            }
        }
    }
}
