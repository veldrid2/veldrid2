using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct MTLSamplerDescriptor
    {
        private static readonly ObjCClass s_class = new("MTLSamplerDescriptor"u8);

        public readonly IntPtr NativePtr;

        public static MTLSamplerDescriptor New() => s_class.AllocInit<MTLSamplerDescriptor>();

        public MTLSamplerAddressMode rAddressMode
        {
            get => (MTLSamplerAddressMode)uint_objc_msgSend(NativePtr, sel_rAddressMode);
            set => objc_msgSend(NativePtr, sel_setRAddressMode, (uint)value);
        }

        public MTLSamplerAddressMode sAddressMode
        {
            get => (MTLSamplerAddressMode)uint_objc_msgSend(NativePtr, sel_sAddressMode);
            set => objc_msgSend(NativePtr, sel_setSAddressMode, (uint)value);
        }

        public MTLSamplerAddressMode tAddressMode
        {
            get => (MTLSamplerAddressMode)uint_objc_msgSend(NativePtr, sel_tAddressMode);
            set => objc_msgSend(NativePtr, sel_setTAddressMode, (uint)value);
        }

        public MTLSamplerMinMagFilter minFilter
        {
            get => (MTLSamplerMinMagFilter)uint_objc_msgSend(NativePtr, sel_minFilter);
            set => objc_msgSend(NativePtr, sel_setMinFilter, (uint)value);
        }

        public MTLSamplerMinMagFilter magFilter
        {
            get => (MTLSamplerMinMagFilter)uint_objc_msgSend(NativePtr, sel_magFilter);
            set => objc_msgSend(NativePtr, sel_setMagFilter, (uint)value);
        }

        public MTLSamplerMipFilter mipFilter
        {
            get => (MTLSamplerMipFilter)uint_objc_msgSend(NativePtr, sel_mipFilter);
            set => objc_msgSend(NativePtr, sel_setMipFilter, (uint)value);
        }

        public float lodMinClamp
        {
            get => float_objc_msgSend(NativePtr, sel_lodMinClamp);
            set => objc_msgSend(NativePtr, sel_setLodMinClamp, value);
        }

        public float lodMaxClamp
        {
            get => float_objc_msgSend(NativePtr, sel_lodMaxClamp);
            set => objc_msgSend(NativePtr, sel_setLodMaxClamp, value);
        }

        public Bool8 lodAverage
        {
            get => bool8_objc_msgSend(NativePtr, sel_lodAverage);
            set => objc_msgSend(NativePtr, sel_setLodAverage, value);
        }

        public UIntPtr maxAnisotropy
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_maxAnisotropy);
            set => objc_msgSend(NativePtr, sel_setMaAnisotropy, value);
        }

        public MTLCompareFunction compareFunction
        {
            get => (MTLCompareFunction)uint_objc_msgSend(NativePtr, sel_compareFunction);
            set => objc_msgSend(NativePtr, sel_setCompareFunction, (uint)value);
        }

        public MTLSamplerBorderColor borderColor
        {
            get => (MTLSamplerBorderColor)uint_objc_msgSend(NativePtr, sel_borderColor);
            set => objc_msgSend(NativePtr, sel_setBorderColor, (uint)value);
        }

        private static readonly Selector sel_rAddressMode = "rAddressMode"u8;
        private static readonly Selector sel_setRAddressMode = "setRAddressMode:"u8;
        private static readonly Selector sel_sAddressMode = "sAddressMode"u8;
        private static readonly Selector sel_setSAddressMode = "setSAddressMode:"u8;
        private static readonly Selector sel_tAddressMode = "tAddressMode"u8;
        private static readonly Selector sel_setTAddressMode = "setTAddressMode:"u8;
        private static readonly Selector sel_minFilter = "minFilter"u8;
        private static readonly Selector sel_setMinFilter = "setMinFilter:"u8;
        private static readonly Selector sel_magFilter = "magFilter"u8;
        private static readonly Selector sel_setMagFilter = "setMagFilter:"u8;
        private static readonly Selector sel_mipFilter = "mipFilter"u8;
        private static readonly Selector sel_setMipFilter = "setMipFilter:"u8;
        private static readonly Selector sel_lodMinClamp = "lodMinClamp"u8;
        private static readonly Selector sel_setLodMinClamp = "setLodMinClamp:"u8;
        private static readonly Selector sel_lodMaxClamp = "lodMaxClamp"u8;
        private static readonly Selector sel_setLodMaxClamp = "setLodMaxClamp:"u8;
        private static readonly Selector sel_lodAverage = "lodAverage"u8;
        private static readonly Selector sel_setLodAverage = "setLodAverage:"u8;
        private static readonly Selector sel_maxAnisotropy = "maxAnisotropy"u8;
        private static readonly Selector sel_setMaAnisotropy = "setMaxAnisotropy:"u8;
        private static readonly Selector sel_compareFunction = "compareFunction"u8;
        private static readonly Selector sel_setCompareFunction = "setCompareFunction:"u8;
        private static readonly Selector sel_borderColor = "borderColor"u8;
        private static readonly Selector sel_setBorderColor = "setBorderColor:"u8;
    }
}
