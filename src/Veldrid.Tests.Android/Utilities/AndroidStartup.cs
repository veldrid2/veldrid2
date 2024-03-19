using System;
using System.Threading;
using Android.Views;

namespace Veldrid.Tests.Android.Utilities
{
    internal static class AndroidStartup
    {
        public static void CreateWindowAndGraphicsDevice(
            int width,
            int height,
            GraphicsDeviceOptions options,
            GraphicsBackend backend,
            out IDisposable? window,
            out GraphicsDevice? gd)
        {
            MainActivity activity = MainActivity.Current;

            VeldridSurfaceView view = new(activity, backend, options);

            void destroyAction()
            {
                activity.RunOnUiThread(() =>
                {
                    IViewManager? manager = view.Parent as IViewManager;
                    manager?.RemoveView(view);
                    view.Holder?.RemoveCallback(view);
                });
            }
            ViewHolder viewHolder = new() { View = view, DestroyAction = destroyAction };

            using ManualResetEventSlim ev = new();

            view.OnDeviceCreated += () =>
            {
                ev.Set();
            };

            activity.RunOnUiThread(() =>
            {
                activity.AddContentView(view, new ViewGroup.LayoutParams(width, height));
            });

            if (!ev.Wait(10000))
            {
                destroyAction();
                throw new TimeoutException("Device not created in time.");
            }

            window = viewHolder;
            gd = view.GraphicsDevice;
        }

        class ViewHolder : IDisposable
        {
            public VeldridSurfaceView? View;
            public Action? DestroyAction;

            public void Dispose()
            {
                if (DestroyAction != null)
                {
                    DestroyAction.Invoke();

                    DestroyAction = null;
                    View = null;
                }
            }
        }
    }
}
