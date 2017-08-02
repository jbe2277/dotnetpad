using System;

namespace Waf.DotNetPad.Samples
{
    internal static class OutVariables
    {
        internal static void Main()
        {
            if (int.TryParse("42", out var result))  // declare out variables in the argument list of a method call
            {
                Console.WriteLine($"Result: {result}");
            }

            if (int.TryParse("42", out _))  // ignore value with '_'
            {
                Console.WriteLine("This is a number");
            }
        }
    }
}