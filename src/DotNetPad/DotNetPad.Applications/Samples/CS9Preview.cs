#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    internal static class CS9Preview
    {
        internal static void Main()
        {
            var person = new Person
            {
                FirstName = "Obi-Wan",
                LastName = "Kenobi"
            };
            // person.FirstName = "Darth";  // not allowed

            var student = new Student { FirstName = "Luke", LastName = "Skywalker" };

            // Target typed ?? and ?
            var result = student ?? person;  // Shared base type

            // "not" pattern
            if (person is not Person) Console.WriteLine("Not of type person");

            // Target-typed new expressions
            Point point = new (2, 3);
        }
    }

    public class Person
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }

    public class Student : Person { }

    public readonly struct Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }
    }
}