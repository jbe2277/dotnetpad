using System.ComponentModel.Composition;

namespace Waf.DotNetPad.Applications.Services
{
    [Export]
    public class CSharpSampleService
    {
        public Lazy<string> OutVariables => new Lazy<string>(() => GetSampleCode("OutVariables.cs"));

        public Lazy<string> Tuples => new Lazy<string>(() => GetSampleCode("Tuples.cs"));

        public Lazy<string> PatternMatching => new Lazy<string>(() => GetSampleCode("PatternMatching.cs"));

        public Lazy<string> LocalFunctions => new Lazy<string>(() => GetSampleCode("LocalFunctions.cs"));

        public Lazy<string> NullableReferenceTypes => new Lazy<string>(() => GetSampleCode("NullableReferenceTypes.cs"));

        public Lazy<string> NullConditionalOperator => new Lazy<string>(() => GetSampleCode("NullConditionalOperator.cs"));

        public Lazy<string> NameOfOperator => new Lazy<string>(() => GetSampleCode("NameOfOperator.cs"));

        public Lazy<string> AutoPropertyInitializers => new Lazy<string>(() => GetSampleCode("AutoPropertyInitializers.cs"));

        public Lazy<string> StringInterpolation => new Lazy<string>(() => GetSampleCode("StringInterpolation.cs"));

        public Lazy<string> SwitchExpression => new Lazy<string>(() => GetSampleCode("SwitchExpression.cs"));

        public Lazy<string> Record => new Lazy<string>(() => GetSampleCode("Record.cs"));

        internal static string GetSampleCode(string sampleFileName)
        {
            using var stream = typeof(CSharpSampleService).Assembly.GetManifestResourceStream("Waf.DotNetPad.Applications.Samples." + sampleFileName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException("Could not load sample code"));
            return reader.ReadToEnd();
        }
    }
}
