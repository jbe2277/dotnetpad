using System;
using static System.Console;

namespace Waf.DotNetPad.Samples
{
    internal static class LocalFunctions
    {
        internal static void Main()
        {
            Write(Fibonacci(6));
        }

        public static int Fibonacci(int x)
        {
            if (x < 0) { throw new ArgumentException("Negative values are not allowed.", nameof(x)); }
            return Fib(x).current;

            // The local function is designed to be used just from the enclosing method.
            (int current, int previous) Fib(int i)

            {
                if (i == 0) { return (1, 0); }
                var(c, p) = Fib(i - 1);
                return (c + p, c);
            }
        }
    }
}