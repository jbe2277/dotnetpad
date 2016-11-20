using System;

namespace Sample
{
    internal static class Program
    {
        internal static void Main()
        {
            if (int.TryParse("42", out var result))  // declare out variables in the argument list of a method call
            {
                Console.WriteLine($"Result: {result}");
            }
        }
    }
}