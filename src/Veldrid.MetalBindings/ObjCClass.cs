using System;
using System.Runtime.CompilerServices;

namespace Veldrid.MetalBindings
{
    public unsafe struct ObjCClass
    {
        public readonly IntPtr NativePtr;

        public static implicit operator IntPtr(ObjCClass c) => c.NativePtr;

        [SkipLocalsInit]
        [Obsolete(MTLUtil.ObsoleteUtf16Message)]
        public ObjCClass(string name) : this(MTLUtil.GetNullTerminatedUtf8Bytes(name, stackalloc byte[1024]))
        {
        }

        public ObjCClass(ReadOnlySpan<byte> nameUtf8)
        {
            fixed (byte* utf8BytesPtr = nameUtf8)
            {
                NativePtr = ObjectiveCRuntime.objc_getClass(utf8BytesPtr);
            }
        }

        [SkipLocalsInit]
        [Obsolete(MTLUtil.ObsoleteUtf16Message)]
        public IntPtr GetProperty(string propertyName)
        {
            Span<byte> utf8Bytes = MTLUtil.GetNullTerminatedUtf8Bytes(propertyName, stackalloc byte[1024]);
            return GetProperty(utf8Bytes);
        }

        public IntPtr GetProperty(ReadOnlySpan<byte> propertyNameUtf8)
        {
            fixed (byte* utf8BytesPtr = propertyNameUtf8)
            {
                return ObjectiveCRuntime.class_getProperty(this, utf8BytesPtr);
            }
        }

        public string Name => MTLUtil.GetUtf8String(ObjectiveCRuntime.class_getName(this));

        public T Alloc<T>() where T : unmanaged
        {
            IntPtr value = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, Selectors.alloc);
            return Unsafe.BitCast<IntPtr, T>(value);
        }

        public T AllocInit<T>() where T : unmanaged
        {
            IntPtr value = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, Selectors.alloc);
            ObjectiveCRuntime.objc_msgSend(value, Selectors.init);
            return Unsafe.BitCast<IntPtr, T>(value);
        }

        public ObjectiveCMethod* class_copyMethodList(out uint count)
        {
            uint result = 0;
            ObjectiveCMethod* ret = ObjectiveCRuntime.class_copyMethodList(this, &result);
            count = result;
            return ret;
        }
    }
}
