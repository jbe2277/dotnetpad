using System.ComponentModel.Composition;

namespace Waf.DotNetPad.Applications.Services;

[Export]
public class VisualBasicSampleService
{
    public Lazy<string> Tuples => new(() => GetSampleCode("Tuples.vb"));

    public Lazy<string> NullConditionalOperator => new(() => GetSampleCode("NullConditionalOperator.vb"));

    public Lazy<string> NameOfOperator => new(() => GetSampleCode("NameOfOperator.vb"));

    public Lazy<string> AutoPropertyInitializers => new(() => GetSampleCode("AutoPropertyInitializers.vb"));

    public Lazy<string> StringInterpolation => new(() => GetSampleCode("StringInterpolation.vb"));

    internal static string GetSampleCode(string sampleFileName) => CSharpSampleService.GetSampleCode(sampleFileName);
}
