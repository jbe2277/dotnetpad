namespace Waf.DotNetPad.Samples;

internal static class AutoPropertyInitializers
{
    private static void Main()
    {
        var customer = new Customer(3);

        Console.WriteLine("Customer {0}: {1}, {2}", customer.Id, customer.First, customer.Last);
    }
}

public class Customer
{
    public Customer(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public string First { get; set; } = "Luke";

    public string Last { get; set; } = "Skywalker";
}
