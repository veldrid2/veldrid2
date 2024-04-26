using System;
using System.Runtime.CompilerServices;

namespace Veldrid.Utilities
{
    internal static class FastParse
    {
        private static readonly double[] _doubleExpLookup = GetDoubleExponents();

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static bool TryParseInt(ReadOnlySpan<char> s, out int result)
        {
            int r = 0;
            int sign;
            int i = 0;

            char c = s[i];
            if (c == '-')
            {
                sign = -1;
                i = 1;
            }
            else if (c > '9' || c < '0')
            {
                goto Fail;
            }
            else
            {
                i = 1;
                r = 10 * r + (c - '0');
                sign = 1;
            }

            for (; i < s.Length; i++)
            {
                c = s[i];
                if (c > '9' || c < '0')
                {
                    goto Fail;
                }

                r = 10 * r + (c - '0');
            }

            result = r * sign;
            return true;

            Fail:
            result = i;
            return false;
        }

        public static bool TryParseDouble(ReadOnlySpan<char> s, out double result, out bool hasFraction, char decimalSeparator = '.')
        {
            hasFraction = false;

            double r = 0;
            int sign;
            int i = 0;

            char c = s[i];
            if (c == '-')
            {
                sign = -1;
                i = 1;
            }
            else if (c > '9' || c < '0')
            {
                goto Fail;
            }
            else
            {
                i = 1;
                r = 10 * r + (c - '0');
                sign = 1;
            }

            for (; i < s.Length; i++)
            {
                c = s[i];
                if (c > '9' || c < '0')
                {
                    if (c == decimalSeparator)
                    {
                        i++;
                        goto DecimalPoint;
                    }
                    else if (c == 'e' || c == 'E')
                    {
                        goto DecimalPoint;
                    }
                    else
                    {
                        goto Fail;
                    }
                }

                r = 10 * r + (c - '0');
            }

            r *= sign;
            goto Finish;

            DecimalPoint:
            double tmp = 0;
            int length = i;
            double exponent = 0;
            hasFraction = true;

            for (; i < s.Length; i++)
            {
                c = s[i];
                if (c > '9' || c < '0')
                {
                    if (c == 'e' || c == 'E')
                    {
                        exponent = ProcessExponent(s, i);
                        break;
                    }

                    goto Fail;
                }
                tmp = 10 * tmp + (c - '0');
            }
            length = i - length;

            double fraction = tmp * GetInversedBaseTen(length);
            r += fraction;
            r *= sign;

            if (exponent > 0)
                r /= exponent;
            else if (exponent < 0)
                r *= -exponent;

            Finish:
            result = r;
            return true;

            Fail:
            result = i;
            return false;
        }

        private static double ProcessExponent(ReadOnlySpan<char> s, int i)
        {
            int expSign = 1;
            int exp = 0;

            for (i++; i < s.Length; i++)
            {
                char c = s[i];
                if (c > '9' || c < '0')
                {
                    if (c == '-')
                    {
                        expSign = -1;
                        continue;
                    }
                }

                exp = 10 * exp + (c - '0');
            }

            double exponent = GetInversedBaseTen(exp) * expSign;
            return exponent;
        }

        private static double GetInversedBaseTen(int index)
        {
            double[] array = _doubleExpLookup;
            if ((uint)index < (uint)array.Length)
                return array[index];
            else
                return Math.Pow(10, -index);
        }

        private static double[] GetDoubleExponents()
        {
            double[] exps = new double[309];

            for (int i = 0; i < exps.Length; i++)
            {
                exps[i] = Math.Pow(10, -i);
            }

            return exps;
        }
    }
}
