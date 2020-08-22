using System;
using static System.Console;

namespace Waf.DotNetPad.Applications.Samples
{
    internal static class SwitchExpression
    {
        public enum Orientation { West, North, East, South }

        internal static void Main()
        {
            int input = 3;
            var orientation = input switch
            {
                1 => Orientation.West,
                2 => Orientation.North,
                3 => Orientation.East,
                4 => Orientation.South,
                _ => throw new NotSupportedException($"Orientation value {input} is not supported.")
            };
            WriteLine($"Orientation: {orientation}");
        }
    }
}
