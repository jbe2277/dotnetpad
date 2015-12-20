using System;
using static System.Console;

namespace Waf.DotNetPad.Applications.Samples
{
    public static class StringInterpolation
    {
        public static void Main()
        {
            var p = new Person() { Name = "Luke", Age = 50 };
            
            WriteLine(string.Format("{0} is {1} year{{s}} old", p.Name, p.Age));
            
            WriteLine($"{p.Name} is {p.Age} year{{s}} old");
			WriteLine($"{p.Name,20} is {p.Age:D3} year{{s}} old");
			WriteLine($"{p.Name} is {p.Age} year{(p.Age == 1 ? "" : "s")} old");
        }
    }

    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }
}