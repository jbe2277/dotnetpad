using System;

namespace Waf.DotNetPad.Applications.Services
{
    public interface ICSharpSampleService
    {
        Lazy<string> NullConditionalOperator { get; }
        
        Lazy<string> NameOfOperator { get; }

        Lazy<string> AutoPropertyInitializers { get; }

        Lazy<string> StringInterpolation { get; }
    }
}
