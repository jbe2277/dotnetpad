using System;
using System.Collections.Generic;
using static System.Console;

namespace Waf.DotNetPad.Samples
{
    internal static class Tuples
    {
        internal static void Main()
        {
            // Create a tuple with semantic names
            (string alpha, string beta) namedLetters1 = ("a", "b");
            var namedLetters2 = (alpha: "a", beta: "b");
            WriteLine($"{namedLetters1.alpha} == {namedLetters2.alpha}");

            // Use method that returns a tuple
            var range = Range(new[] { 8, 2, 4 });
            WriteLine($"Min: {range.min}; Max: {range.max}");
            (int min, int max) = Range(new[] { 8, 2, 4 });
            WriteLine($"Min: {min}; Max: {max}");

            // Use deconstruct
            var point1 = new Point(1.1, 1.2);
            (double a, double b) = point1;
            WriteLine($"X: {a}; Y: {b}");
        }

        private static (int min, int max) Range(IEnumerable<int> numbers)
        {
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (var n in numbers)
            {
                min = (n < min) ? n : min;
                max = (n > max) ? n : max;
            }
            return (min, max);
        }

        public struct Point
        {
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }

            public void Deconstruct(out double x, out double y)
            {
                x = X;
                y = Y;
            }
        }
    }
}