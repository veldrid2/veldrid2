using System;
using System.Runtime.InteropServices;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MTLRenderPassDescriptor
    {
        private static readonly ObjCClass s_class = new("MTLRenderPassDescriptor"u8);

        public readonly IntPtr NativePtr;

        public static MTLRenderPassDescriptor New() => s_class.AllocInit<MTLRenderPassDescriptor>();

        public MTLRenderPassColorAttachmentDescriptorArray colorAttachments
            => objc_msgSend<MTLRenderPassColorAttachmentDescriptorArray>(NativePtr, sel_colorAttachments);

        public MTLRenderPassDepthAttachmentDescriptor depthAttachment
            => objc_msgSend<MTLRenderPassDepthAttachmentDescriptor>(NativePtr, sel_depthAttachment);

        public MTLRenderPassStencilAttachmentDescriptor stencilAttachment
            => objc_msgSend<MTLRenderPassStencilAttachmentDescriptor>(NativePtr, sel_stencilAttachment);

        private static readonly Selector sel_colorAttachments = "colorAttachments"u8;
        private static readonly Selector sel_depthAttachment = "depthAttachment"u8;
        private static readonly Selector sel_stencilAttachment = "stencilAttachment"u8;
    }
}
