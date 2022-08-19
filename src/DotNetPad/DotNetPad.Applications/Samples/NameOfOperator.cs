namespace Waf.DotNetPad.Samples;

public static class NameOfOperator
{
    private static readonly ObservableCollection<string> list = new();

    public static void Main()
    {
        // Use nameof to compare with the property name provided by the event args.
        ((INotifyPropertyChanged)list).PropertyChanged += ListPropertyChanged;
        list.Add("Luke");
        list.Add("Han");

        // Use nameof with the ArgumentNullException constructor.
        try
        {
            ArgumentNullCheck(null);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine("Expected exception was thrown: " + ex.Message);
        }
    }

    private static void ListPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(list.Count))
        {
            Console.WriteLine("Count: " + list.Count);
        }
    }

    private static void ArgumentNullCheck(IReadOnlyList<string>? list)
    {
        if (list is null) throw new ArgumentNullException(nameof(list));
    }
}
