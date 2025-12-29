using System;
using System.Runtime.InteropServices;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct UIView
    {
        public readonly IntPtr NativePtr;

        public UIView(IntPtr ptr) => NativePtr = ptr;

        public CALayer layer => objc_msgSend<CALayer>(NativePtr, "layer"u8);

        public CGRect frame =>
            RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                ? CGRect_objc_msgSend(NativePtr, "frame"u8)
                : objc_msgSend_stret<CGRect>(NativePtr, "frame"u8);
    }
}
