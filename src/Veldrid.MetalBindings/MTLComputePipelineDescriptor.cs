using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct MTLComputePipelineDescriptor
    {
        public readonly IntPtr NativePtr;

        public MTLFunction computeFunction
        {
            get => objc_msgSend<MTLFunction>(NativePtr, sel_computeFunction);
            set => objc_msgSend(NativePtr, sel_setComputeFunction, value.NativePtr);
        }

        public MTLPipelineBufferDescriptorArray buffers
            => objc_msgSend<MTLPipelineBufferDescriptorArray>(NativePtr, sel_buffers);

        private static readonly Selector sel_computeFunction = "computeFunction"u8;
        private static readonly Selector sel_setComputeFunction = "setComputeFunction:"u8;
        private static readonly Selector sel_buffers = "buffers"u8;
    }
}
