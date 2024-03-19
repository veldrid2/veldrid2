using System;

namespace Veldrid.MetalBindings
{
    public readonly struct NSDictionary
    {
        public readonly IntPtr NativePtr;

        public UIntPtr count => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, "count"u8);
    }
}
