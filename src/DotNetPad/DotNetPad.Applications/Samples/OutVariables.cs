namespace Waf.DotNetPad.Samples;

internal static class OutVariables
{
    private static void Main()
    {
        // declare out variables in the argument list of a method call
        if (int.TryParse("42", out var result))
        {
            Console.WriteLine($"Result: {result}");
        }

        // ignore value with '_'
        if (int.TryParse("42", out _))  
        {
            Console.WriteLine("This is a number");
        }
    }
}