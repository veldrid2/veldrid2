using System;
using System.Runtime.InteropServices;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct NSView
    {
        public readonly IntPtr NativePtr;
        
        public NSView(IntPtr ptr) => NativePtr = ptr;

        public Bool8 wantsLayer
        {
            get => bool8_objc_msgSend(NativePtr, "wantsLayer"u8);
            set => objc_msgSend(NativePtr, "setWantsLayer:"u8, value);
        }

        public IntPtr layer
        {
            get => IntPtr_objc_msgSend(NativePtr, "layer"u8);
            set => objc_msgSend(NativePtr, "setLayer:"u8, value);
        }

        public CGRect frame
        {
            get
            {
                return RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? CGRect_objc_msgSend(NativePtr, "frame"u8)
                    : objc_msgSend_stret<CGRect>(NativePtr, "frame"u8);
            }
        }
    }
}
