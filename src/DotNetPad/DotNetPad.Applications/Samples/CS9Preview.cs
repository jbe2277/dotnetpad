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
            var person = new Person("Obi-Wan", "Kenobi");
            Console.WriteLine(person.ToString());

            // Target-typed new expressions
            Student student = new("Luke", "Skywalker") { Style = "Jedi" };
            // student.Style = "Sith";  // not allowed
            Console.WriteLine(student.ToString());

            // Target typed ?? and ?
            var result = student ?? person;  // Shared base type
            Console.WriteLine(result.ToString());

            // "not" pattern
            if (person is not Person) Console.WriteLine("Not of type person");
        }
    }

    public record Person(string? FirstName, string? LastName);

    public record Student(string? FirstName, string? LastName) : Person(FirstName, LastName)
    {
        public string? Style { get; init; }
    }
}