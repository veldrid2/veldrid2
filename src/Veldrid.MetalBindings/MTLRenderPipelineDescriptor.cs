using static Veldrid.MetalBindings.ObjectiveCRuntime;
using System;
using System.Runtime.InteropServices;

namespace Veldrid.MetalBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MTLRenderPipelineDescriptor
    {
        public readonly IntPtr NativePtr;

        public MTLRenderPipelineDescriptor(IntPtr ptr) => NativePtr = ptr;

        public static MTLRenderPipelineDescriptor New()
        {
            ObjCClass cls = new("MTLRenderPipelineDescriptor"u8);
            MTLRenderPipelineDescriptor ret = cls.AllocInit<MTLRenderPipelineDescriptor>();
            return ret;
        }

        public MTLFunction vertexFunction
        {
            get => objc_msgSend<MTLFunction>(NativePtr, sel_vertexFunction);
            set => objc_msgSend(NativePtr, sel_setVertexFunction, value.NativePtr);
        }

        public MTLFunction fragmentFunction
        {
            get => objc_msgSend<MTLFunction>(NativePtr, sel_fragmentFunction);
            set => objc_msgSend(NativePtr, sel_setFragmentFunction, value.NativePtr);
        }

        public MTLRenderPipelineColorAttachmentDescriptorArray colorAttachments
            => objc_msgSend<MTLRenderPipelineColorAttachmentDescriptorArray>(NativePtr, sel_colorAttachments);

        public MTLPixelFormat depthAttachmentPixelFormat
        {
            get => (MTLPixelFormat)uint_objc_msgSend(NativePtr, sel_depthAttachmentPixelFormat);
            set => objc_msgSend(NativePtr, sel_setDepthAttachmentPixelFormat, (uint)value);
        }

        public MTLPixelFormat stencilAttachmentPixelFormat
        {
            get => (MTLPixelFormat)uint_objc_msgSend(NativePtr, sel_stencilAttachmentPixelFormat);
            set => objc_msgSend(NativePtr, sel_setStencilAttachmentPixelFormat, (uint)value);
        }

        public UIntPtr sampleCount
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_sampleCount);
            set => objc_msgSend(NativePtr, sel_setSampleCount, value);
        }

        public MTLVertexDescriptor vertexDescriptor => objc_msgSend<MTLVertexDescriptor>(NativePtr, sel_vertexDescriptor);

        public Bool8 alphaToCoverageEnabled
        {
            get => bool8_objc_msgSend(NativePtr, sel_isAlphaToCoverageEnabled);
            set => objc_msgSend(NativePtr, sel_setAlphaToCoverageEnabled, value);
        }

        private static readonly Selector sel_vertexFunction = "vertexFunction"u8;
        private static readonly Selector sel_setVertexFunction = "setVertexFunction:"u8;
        private static readonly Selector sel_fragmentFunction = "fragmentFunction"u8;
        private static readonly Selector sel_setFragmentFunction = "setFragmentFunction:"u8;
        private static readonly Selector sel_colorAttachments = "colorAttachments"u8;
        private static readonly Selector sel_depthAttachmentPixelFormat = "depthAttachmentPixelFormat"u8;
        private static readonly Selector sel_setDepthAttachmentPixelFormat = "setDepthAttachmentPixelFormat:"u8;
        private static readonly Selector sel_stencilAttachmentPixelFormat = "stencilAttachmentPixelFormat"u8;
        private static readonly Selector sel_setStencilAttachmentPixelFormat = "setStencilAttachmentPixelFormat:"u8;
        private static readonly Selector sel_sampleCount = "sampleCount"u8;
        private static readonly Selector sel_setSampleCount = "setSampleCount:"u8;
        private static readonly Selector sel_vertexDescriptor = "vertexDescriptor"u8;
        private static readonly Selector sel_isAlphaToCoverageEnabled = "isAlphaToCoverageEnabled"u8;
        private static readonly Selector sel_setAlphaToCoverageEnabled = "setAlphaToCoverageEnabled:"u8;
    }
}
