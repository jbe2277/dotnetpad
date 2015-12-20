using System;
using System.ComponentModel.Composition;

namespace Waf.DotNetPad.Applications.Services
{
    [Export(typeof(IVisualBasicSampleService))]
    internal class VisualBasicSampleService : IVisualBasicSampleService
    {
        public Lazy<string> NullConditionalOperator
        {
            get { return new Lazy<string>(() => GetSampleCode("NullConditionalOperator.vb")); }
        }
        
        public Lazy<string> NameOfOperator
        {
            get { return new Lazy<string>(() => GetSampleCode("NameOfOperator.vb")); }
        }

        public Lazy<string> AutoPropertyInitializers
        {
            get { return new Lazy<string>(() => GetSampleCode("AutoPropertyInitializers.vb")); }
        }

        public Lazy<string> StringInterpolation
        {
            get { return new Lazy<string>(() => GetSampleCode("StringInterpolation.vb")); }
        }


        internal static string GetSampleCode(string sampleFileName)
        {
            return CSharpSampleService.GetSampleCode(sampleFileName);
        }
    }
}
