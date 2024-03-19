using System;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    public readonly struct MTLTextureDescriptor
    {
        private static readonly ObjCClass s_class = new("MTLTextureDescriptor"u8);

        public readonly IntPtr NativePtr;

        public static MTLTextureDescriptor New() => s_class.AllocInit<MTLTextureDescriptor>();

        public MTLTextureType textureType
        {
            get => (MTLTextureType)uint_objc_msgSend(NativePtr, sel_textureType);
            set => objc_msgSend(NativePtr, sel_setTextureType, (uint)value);
        }

        public MTLPixelFormat pixelFormat
        {
            get => (MTLPixelFormat)uint_objc_msgSend(NativePtr, Selectors.pixelFormat);
            set => objc_msgSend(NativePtr, Selectors.setPixelFormat, (uint)value);
        }

        public UIntPtr width
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_width);
            set => objc_msgSend(NativePtr, sel_setWidth, value);
        }

        public UIntPtr height
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_height);
            set => objc_msgSend(NativePtr, sel_setHeight, value);
        }

        public UIntPtr depth
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_depth);
            set => objc_msgSend(NativePtr, sel_setDepth, value);
        }

        public UIntPtr mipmapLevelCount
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_mipmapLevelCount);
            set => objc_msgSend(NativePtr, sel_setMipmapLevelCount, value);
        }

        public UIntPtr sampleCount
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_sampleCount);
            set => objc_msgSend(NativePtr, sel_setSampleCount, value);
        }

        public UIntPtr arrayLength
        {
            get => UIntPtr_objc_msgSend(NativePtr, sel_arrayLength);
            set => objc_msgSend(NativePtr, sel_setArrayLength, value);
        }

        public MTLResourceOptions resourceOptions
        {
            get => (MTLResourceOptions)uint_objc_msgSend(NativePtr, sel_resourceOptions);
            set => objc_msgSend(NativePtr, sel_setResourceOptions, (uint)value);
        }

        public MTLCPUCacheMode cpuCacheMode
        {
            get => (MTLCPUCacheMode)uint_objc_msgSend(NativePtr, sel_cpuCacheMode);
            set => objc_msgSend(NativePtr, sel_setCpuCacheMode, (uint)value);
        }

        public MTLStorageMode storageMode
        {
            get => (MTLStorageMode)uint_objc_msgSend(NativePtr, sel_storageMode);
            set => objc_msgSend(NativePtr, sel_setStorageMode, (uint)value);
        }

        public MTLTextureUsage textureUsage
        {
            get => (MTLTextureUsage)uint_objc_msgSend(NativePtr, sel_textureUsage);
            set => objc_msgSend(NativePtr, sel_setTextureUsage, (uint)value);
        }

        private static readonly Selector sel_textureType = "textureType"u8;
        private static readonly Selector sel_setTextureType = "setTextureType:"u8;
        private static readonly Selector sel_width = "width"u8;
        private static readonly Selector sel_setWidth = "setWidth:"u8;
        private static readonly Selector sel_height = "height"u8;
        private static readonly Selector sel_setHeight = "setHeight:"u8;
        private static readonly Selector sel_depth = "depth"u8;
        private static readonly Selector sel_setDepth = "setDepth:"u8;
        private static readonly Selector sel_mipmapLevelCount = "mipmapLevelCount"u8;
        private static readonly Selector sel_setMipmapLevelCount = "setMipmapLevelCount:"u8;
        private static readonly Selector sel_sampleCount = "sampleCount"u8;
        private static readonly Selector sel_setSampleCount = "setSampleCount:"u8;
        private static readonly Selector sel_arrayLength = "arrayLength"u8;
        private static readonly Selector sel_setArrayLength = "setArrayLength:"u8;
        private static readonly Selector sel_resourceOptions = "resourceOptions"u8;
        private static readonly Selector sel_setResourceOptions = "setResourceOptions:"u8;
        private static readonly Selector sel_cpuCacheMode = "cpuCacheMode"u8;
        private static readonly Selector sel_setCpuCacheMode = "setCpuCacheMode:"u8;
        private static readonly Selector sel_storageMode = "storageMode"u8;
        private static readonly Selector sel_setStorageMode = "setStorageMode:"u8;
        private static readonly Selector sel_textureUsage = "textureUsage"u8;
        private static readonly Selector sel_setTextureUsage = "setTextureUsage:"u8;
    }
}
