using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct UIScreen
    {
        public readonly IntPtr NativePtr;

        public UIScreen(IntPtr ptr)
        {
            NativePtr = ptr;
        }

        public CGFloat nativeScale => CGFloat_objc_msgSend(NativePtr, "nativeScale"u8);

        public static UIScreen mainScreen
            => objc_msgSend<UIScreen>(new ObjCClass("UIScreen"u8), "mainScreen"u8);
    }
}
