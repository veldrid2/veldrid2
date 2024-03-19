using System;
using System.Runtime.InteropServices;
using static Veldrid.MetalBindings.ObjectiveCRuntime;

namespace Veldrid.MetalBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MTLCompileOptions
    {
        public readonly IntPtr NativePtr;

        public static implicit operator IntPtr(MTLCompileOptions mco) => mco.NativePtr;

        public static MTLCompileOptions New()
        {
            return s_class.AllocInit<MTLCompileOptions>();
        }

        public Bool8 fastMathEnabled
        {
            get => bool8_objc_msgSend(NativePtr, sel_fastMathEnabled);
            set => objc_msgSend(NativePtr, sel_setFastMathEnabled, value);
        }

        public MTLLanguageVersion languageVersion
        {
            get => (MTLLanguageVersion)uint_objc_msgSend(NativePtr, sel_languageVersion);
            set => objc_msgSend(NativePtr, sel_setLanguageVersion, (uint)value);
        }

        private static readonly ObjCClass s_class = new("MTLCompileOptions"u8);
        private static readonly Selector sel_fastMathEnabled = "fastMathEnabled"u8;
        private static readonly Selector sel_setFastMathEnabled = "setFastMathEnabled:"u8;
        private static readonly Selector sel_languageVersion = "languageVersion"u8;
        private static readonly Selector sel_setLanguageVersion = "setLanguageVersion:"u8;
    }
}
