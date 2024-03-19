using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct MTLVertexDescriptor
    {
        public readonly IntPtr NativePtr;

        public MTLVertexBufferLayoutDescriptorArray layouts
            => objc_msgSend<MTLVertexBufferLayoutDescriptorArray>(NativePtr, sel_layouts);

        public MTLVertexAttributeDescriptorArray attributes
            => objc_msgSend<MTLVertexAttributeDescriptorArray>(NativePtr, sel_attributes);

        private static readonly Selector sel_layouts = "layouts"u8;
        private static readonly Selector sel_attributes = "attributes"u8;
    }
}
