using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace Veldrid.MetalBindings
{
    public static class MTLUtil
    {
        internal const string ObsoleteUtf16Message = "Prefer the UTF8 overload to skip conversion costs.";

        public static Encoding UTF8 { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static unsafe string GetUtf8String(byte* stringStart)
        {
            return Marshal.PtrToStringUTF8((IntPtr) stringStart) ?? string.Empty;
        }

        public static Span<byte> GetNullTerminatedUtf8Bytes(ReadOnlySpan<char> value, Span<byte> buffer)
        {
            OperationStatus status = Utf8.FromUtf16(value, buffer, out _, out int bytesWritten);
            if (status == OperationStatus.Done && bytesWritten < buffer.Length)
            {
                buffer[bytesWritten] = 0;
                return buffer.Slice(0, bytesWritten + 1);
            }
            return UTF8.GetBytes($"{value}\0");
        }

        [Obsolete(ObsoleteUtf16Message)]
        public static T AllocInit<T>(string typeName) where T : unmanaged
        {
            ObjCClass cls = new(typeName);
            return cls.AllocInit<T>();
        }

        public static T AllocInit<T>(ReadOnlySpan<byte> utf8TypeName) where T : unmanaged
        {
            ObjCClass cls = new(utf8TypeName);
            return cls.AllocInit<T>();
        }
    }
}
