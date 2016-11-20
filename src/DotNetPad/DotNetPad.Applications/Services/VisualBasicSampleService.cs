using System;
using System.ComponentModel.Composition;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(IVisualBasicSampleService))]
    internal class VisualBasicSampleService : IVisualBasicSampleService
    {
        public Lazy<string> Tuples => new Lazy<string>(() => GetSampleCode("Tuples.vb"));

        public Lazy<string> NullConditionalOperator => new Lazy<string>(() => GetSampleCode("NullConditionalOperator.vb"));
        
        public Lazy<string> NameOfOperator => new Lazy<string>(() => GetSampleCode("NameOfOperator.vb"));

        public Lazy<string> AutoPropertyInitializers => new Lazy<string>(() => GetSampleCode("AutoPropertyInitializers.vb"));

        public Lazy<string> StringInterpolation => new Lazy<string>(() => GetSampleCode("StringInterpolation.vb"));


        internal static string GetSampleCode(string sampleFileName)
        {
            return CSharpSampleService.GetSampleCode(sampleFileName);
        }
    }
}
