using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct NSWindow
    {
        public readonly IntPtr NativePtr;

        public NSWindow(IntPtr ptr)
        {
            NativePtr = ptr;
        }

        public NSView contentView => objc_msgSend<NSView>(NativePtr, "contentView"u8);
    }
}
