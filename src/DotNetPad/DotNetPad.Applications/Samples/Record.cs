namespace Waf.DotNetPad.Samples;

internal static class Record
{
    internal static void Main()
    {
        var person = new Person("Obi-Wan", "Kenobi");
        Console.WriteLine(person);

        // Target-typed new expressions
        Student student = new("Luke", "Skywalker") { Style = "Jedi" };
        // student.Style = "Sith";  // not allowed
        Console.WriteLine(student);

        // with ... create clone
        var lukeClone = student with { };
        Console.WriteLine($"Luke and LukeClone: Equals: {student == lukeClone}; ReferenceEquals: {ReferenceEquals(student, lukeClone)}");

        var lea = student with { FirstName = "Lea" };  
        Console.WriteLine(lea);

        // Target typed ?? and ?
        var result = student ?? person;  // Shared base type
        Console.WriteLine(result);

        // "not" pattern
        if (person is not Person) Console.WriteLine("Not of type person");
    }
}

public record Person(string? FirstName, string? LastName);

public record Student(string? FirstName, string? LastName) : Person(FirstName, LastName)
{
    public string? Style { get; init; }
}
