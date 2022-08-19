using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.Console;

namespace Waf.DotNetPad.Samples;

internal static class NullableReferenceTypes
{
    private static void Main()
    {
        string notNull = "Hello";
        string? nullable = null;
        notNull = nullable!;   // ! -> null-forgiving operator

        _ = new NullableTest().Name = null;
        _ = new NullableTest().Description = null;
        _ = NullableTest.TryParse("").ToString();
        _ = NullableTest.GetNextElementOrDefault(new[] { "1", "2" }, "5").ToString();
        if (NullableTest.TryGetValue("key", out var result)) { _ = result.ToString(); }
        _ = NullableTest.Truncate("abc", 5).ToString();
    }
}

public class NullableTest
{
    private string? name = null;
    private string description = "";

    [DisallowNull]   // Get might return null but disallow to set null
    public string? Name
    {
        get => name;
        set => name = value ?? throw new ArgumentNullException(nameof(value));
    }

    [AllowNull]   // Get never return null but allow to set null 
    public string Description
    {
        get => description;
        set => description = value ?? "";
    }

    public static string? TryParse(string value) => null;

    [return: MaybeNull]   // Return value may be null if T is a reference type
    // [AllowNull] -> Allow null if T is a reference type 
    public static T GetNextElementOrDefault<T>(IEnumerable<T> items, [AllowNull] T current) => default;

    // [NotNullWhen(true)] -> out value is not null if method returns true
    public static bool TryGetValue(object key, [NotNullWhen(true)] out object? value) { value = null; return false; }

    // [NotNullIfNotNull(parameterName: "value") -> Return value is not null if parameter s is not null
    [return: NotNullIfNotNull(parameterName: "s")]
    public static string? Truncate(string? s, int maxLength) => s;

    [DoesNotReturn]   // This method never returns
    public static void ThrowException() => throw new InvalidOperationException();

    // This method does not return if condition is false
    public static void Assert([DoesNotReturnIf(false)] bool condition) { if (!condition) throw new InvalidOperationException(); }
}
