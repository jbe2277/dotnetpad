using System.ComponentModel.Composition;

namespace Waf.DotNetPad.Applications.Services;

[Export]
public class CSharpSampleService
{
    public Lazy<string> OutVariables => new(() => GetSampleCode("OutVariables.cs"));

    public Lazy<string> Tuples => new(() => GetSampleCode("Tuples.cs"));

    public Lazy<string> PatternMatching => new(() => GetSampleCode("PatternMatching.cs"));

    public Lazy<string> LocalFunctions => new(() => GetSampleCode("LocalFunctions.cs"));

    public Lazy<string> NullableReferenceTypes => new(() => GetSampleCode("NullableReferenceTypes.cs"));

    public Lazy<string> NullConditionalOperator => new(() => GetSampleCode("NullConditionalOperator.cs"));

    public Lazy<string> NameOfOperator => new(() => GetSampleCode("NameOfOperator.cs"));

    public Lazy<string> AutoPropertyInitializers => new(() => GetSampleCode("AutoPropertyInitializers.cs"));

    public Lazy<string> StringInterpolation => new(() => GetSampleCode("StringInterpolation.cs"));

    public Lazy<string> SwitchExpression => new(() => GetSampleCode("SwitchExpression.cs"));

    public Lazy<string> Record => new(() => GetSampleCode("Record.cs"));

    internal static string GetSampleCode(string sampleFileName)
    {
        using var stream = typeof(CSharpSampleService).Assembly.GetManifestResourceStream("Waf.DotNetPad.Applications.Samples." + sampleFileName);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException("Could not load sample code"));
        return reader.ReadToEnd();
    }
}
