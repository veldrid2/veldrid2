using System;

namespace Veldrid.MetalBindings
{
    public readonly struct NSAutoreleasePool : IDisposable
    {
        private static readonly ObjCClass s_class = new("NSAutoreleasePool"u8);

        public readonly IntPtr NativePtr;

        public NSAutoreleasePool(IntPtr ptr) => NativePtr = ptr;

        public static NSAutoreleasePool Begin()
        {
            return s_class.AllocInit<NSAutoreleasePool>();
        }

        public void Dispose()
        {
            ObjectiveCRuntime.release(NativePtr);
        }
    }
}
