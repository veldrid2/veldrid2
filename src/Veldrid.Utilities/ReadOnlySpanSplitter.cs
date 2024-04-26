using System;
using System.Diagnostics;

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
                span = span.Slice(0, Math.Max(1, next));
                _offset += span.Length + Separator.Length;

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
    }
}
