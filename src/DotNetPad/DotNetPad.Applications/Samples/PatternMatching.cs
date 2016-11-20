using System;
using static System.Console;

namespace Waf.DotNetPad.Samples
{
    internal static class PatternMatching
    {
        internal static void Main()
        {
            // Use 'is' pattern expression with variable initialization
            Shape shape = new Circle { Radius = 2 };
            if (shape is Circle c)
            {
                WriteLine($"circle with radius {c.Radius}");
            }

            PrintShape(new Circle { Radius = 2 });
            PrintShape(new Rectangle { Length = 3, Height = 3 });
            PrintShape(new Rectangle { Length = 5, Height = 3 });
        }

        private static void PrintShape(Shape shape)
        {
            // Use 'case' with pattern expressions
            switch (shape)
            {
                case Circle c:
                    WriteLine($"circle with radius {c.Radius}");
                    break;
                case Rectangle s when (s.Length == s.Height):
			        WriteLine($"{s.Length} x {s.Height} square");
                    break;
                case Rectangle r:
                    WriteLine($"{r.Length} x {r.Height} rectangle");
                    break;
                case null:
                    throw new ArgumentNullException(nameof(shape));
                default:
                    WriteLine("<unknown shape>");
                    break;
            }
        }
    }

    public abstract class Shape
    {
    }

    public class Circle : Shape
    {
        public double Radius { get; set; }
    }

    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Height { get; set; }
    }
}