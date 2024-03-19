using System;
using System.Runtime.InteropServices;
using Veldrid.MetalBindings;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.OpenGL.EAGL
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CAEAGLLayer
    {
        public readonly IntPtr NativePtr;

        public static CAEAGLLayer New()
        {
            return MTLUtil.AllocInit<CAEAGLLayer>("CAEAGLLayer"u8);
        }

        public CGRect frame
        {
            get => CGRect_objc_msgSend(NativePtr, "frame"u8);
            set => objc_msgSend(NativePtr, "setFrame:"u8, value);
        }

        public Bool8 opaque
        {
            get => bool8_objc_msgSend(NativePtr, "isOpaque"u8);
            set => objc_msgSend(NativePtr, "setOpaque:"u8, value);
        }

        public void removeFromSuperlayer() => objc_msgSend(NativePtr, "removeFromSuperlayer"u8);

        public void Release() => release(NativePtr);
    }
}
