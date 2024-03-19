using System;
using System.Runtime.CompilerServices;

namespace Veldrid.MetalBindings
{
    public readonly unsafe struct Selector
    {
        public readonly IntPtr NativePtr;

        public Selector(IntPtr ptr)
        {
            NativePtr = ptr;
        }

        [SkipLocalsInit]
        [Obsolete(MTLUtil.ObsoleteUtf16Message)]
        public Selector(string name) : this(MTLUtil.GetNullTerminatedUtf8Bytes(name, stackalloc byte[1024]))
        {
        }

        public Selector(ReadOnlySpan<byte> nameUtf8)
        {
            fixed (byte* utf8BytesPtr = nameUtf8)
            {
                NativePtr = ObjectiveCRuntime.sel_registerName(utf8BytesPtr);
            }
        }

        public string Name
        {
            get
            {
                byte* name = ObjectiveCRuntime.sel_getName(NativePtr);
                return MTLUtil.GetUtf8String(name);
            }
        }

        [Obsolete(MTLUtil.ObsoleteUtf16Message)]
        public static implicit operator Selector(string s) => new(s);

        public static implicit operator Selector(ReadOnlySpan<byte> utf8) => new(utf8);
    }
}
