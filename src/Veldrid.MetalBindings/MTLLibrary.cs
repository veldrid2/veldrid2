using System;
using System.Runtime.InteropServices;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MTLLibrary
    {
        public readonly IntPtr NativePtr;
        public MTLLibrary(IntPtr ptr) => NativePtr = ptr;

        public MTLFunction newFunctionWithName(string name)
        {
            NSString nameNSS = NSString.New(name);
            IntPtr function = IntPtr_objc_msgSend(NativePtr, sel_newFunctionWithName, nameNSS);
            release(nameNSS.NativePtr);
            return new MTLFunction(function);
        }

        public unsafe MTLFunction newFunctionWithNameConstantValues(string name, MTLFunctionConstantValues constantValues)
        {
            NSString nameNSS = NSString.New(name);
            NSError error;
            IntPtr function = IntPtr_objc_msgSend(
                NativePtr,
                sel_newFunctionWithNameConstantValues,
                nameNSS.NativePtr,
                constantValues.NativePtr,
                &error);
            release(nameNSS.NativePtr);

            if (function == IntPtr.Zero)
            {
                throw new Exception($"Failed to create MTLFunction: {error.localizedDescription}");
            }

            return new MTLFunction(function);
        }

        private static readonly Selector sel_newFunctionWithName = "newFunctionWithName:"u8;
        private static readonly Selector sel_newFunctionWithNameConstantValues = "newFunctionWithName:constantValues:error:"u8;
    }
}
