using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Veldrid.Utilities
{
    internal ref struct ReadOnlySpanSplitter<T>
        where T : IEquatable<T>
    {
        private int _offset;

        public ReadOnlySpan<T> Value { get; }
        public ReadOnlySpan<T> Separator { get; }
        public StringSplitOptions SplitOptions { get; }

        public ReadOnlySpan<T> Current { get; private set; }

        public ReadOnlySpanSplitter(
            ReadOnlySpan<T> value,
            ReadOnlySpan<T> separator,
            StringSplitOptions splitOptions)
        {
            if ((splitOptions & StringSplitOptions.TrimEntries) != 0)
            {
                value = Trim(value);
            }

            Value = value;
            Separator = separator;
            SplitOptions = splitOptions;
        }

        public bool MoveNext()
        {
            TryMove:
            ReadOnlySpan<T> span = Value.Slice(_offset);

            // Empty separator in IndexOf always returns 0.
            // Check span length to not loop forever on end.
            int next = span.Length > 0 ? span.IndexOf(Separator) : -1;

            if (next != -1)
            {
                span = span.Slice(0, next);
                _offset += span.Length + Separator.Length;

                if ((SplitOptions & StringSplitOptions.TrimEntries) != 0)
                {
                    span = Trim(span);
                }

                if (span.Length == 0 && (SplitOptions & StringSplitOptions.RemoveEmptyEntries) != 0)
                {
                    goto TryMove;
                }
            }
            else if (span.Length > 0)
            {
                // This is the remainder of Value.
                _offset += span.Length;

                Debug.Assert(_offset == Value.Length);
            }
            else
            {
                Debug.Assert(_offset == Value.Length);

                // We've reached the end of Value.
                Current = default;
                return false;
            }

            Current = span;
            return true;
        }

        public readonly ReadOnlySpanSplitter<T> GetEnumerator()
        {
            return this;
        }
        
        private static ReadOnlySpan<T> Trim(ReadOnlySpan<T> span)
        {
            if (typeof(T) != typeof(char))
            {
                return span;
            }

            ReadOnlySpan<char> charSpan = MemoryMarshal.CreateReadOnlySpan(
                    ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)),
                    span.Length);

            charSpan = charSpan.Trim();

            ReadOnlySpan<T> tSpan = MemoryMarshal.CreateReadOnlySpan(
                ref Unsafe.As<char, T>(ref MemoryMarshal.GetReference(charSpan)),
                charSpan.Length);

            return tSpan;
        }
    }
}
