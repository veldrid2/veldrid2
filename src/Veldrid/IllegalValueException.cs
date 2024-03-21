using System.Diagnostics.CodeAnalysis;

namespace Veldrid
{
    internal static class Illegal
    {
        [DoesNotReturn]
        internal static void Value<T>()
        {
            throw new IllegalValueException<T>();
        }

        [DoesNotReturn]
        internal static R Value<T, R>()
        {
            throw new IllegalValueException<T, R>();
        }

        internal class IllegalValueException<T> : VeldridException
        {
        }

        internal class IllegalValueException<T, R> : IllegalValueException<T>
        {
        }
    }
}
