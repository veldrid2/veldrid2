using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct MTLDepthStencilDescriptor
    {
        public readonly IntPtr NativePtr;
        public MTLDepthStencilDescriptor(IntPtr ptr) => NativePtr = ptr;

        public MTLCompareFunction depthCompareFunction
        {
            get => (MTLCompareFunction)uint_objc_msgSend(NativePtr, sel_depthCompareFunction);
            set => objc_msgSend(NativePtr, sel_setDepthCompareFunction, (uint)value);
        }

        public Bool8 depthWriteEnabled
        {
            get => bool8_objc_msgSend(NativePtr, sel_isDepthWriteEnabled);
            set => objc_msgSend(NativePtr, sel_setDepthWriteEnabled, value);
        }

        public MTLStencilDescriptor backFaceStencil
        {
            get => objc_msgSend<MTLStencilDescriptor>(NativePtr, sel_backFaceStencil);
            set => objc_msgSend(NativePtr, sel_setBackFaceStencil, value.NativePtr);
        }

        public MTLStencilDescriptor frontFaceStencil
        {
            get => objc_msgSend<MTLStencilDescriptor>(NativePtr, sel_frontFaceStencil);
            set => objc_msgSend(NativePtr, sel_setFrontFaceStencil, value.NativePtr);
        }

        private static readonly Selector sel_depthCompareFunction = "depthCompareFunction"u8;
        private static readonly Selector sel_setDepthCompareFunction = "setDepthCompareFunction:"u8;
        private static readonly Selector sel_isDepthWriteEnabled = "isDepthWriteEnabled"u8;
        private static readonly Selector sel_setDepthWriteEnabled = "setDepthWriteEnabled:"u8;
        private static readonly Selector sel_backFaceStencil = "backFaceStencil"u8;
        private static readonly Selector sel_setBackFaceStencil = "setBackFaceStencil:"u8;
        private static readonly Selector sel_frontFaceStencil = "frontFaceStencil"u8;
        private static readonly Selector sel_setFrontFaceStencil = "setFrontFaceStencil:"u8;
    }
}
