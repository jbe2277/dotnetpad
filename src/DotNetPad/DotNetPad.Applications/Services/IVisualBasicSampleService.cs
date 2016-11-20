using System;

namespace Waf.DotNetPad.Applications.Services
{
    public interface IVisualBasicSampleService
    {
        Lazy<string> Tuples { get; }

        Lazy<string> NullConditionalOperator { get; }
        
        Lazy<string> NameOfOperator { get; }

        Lazy<string> AutoPropertyInitializers { get; }

        Lazy<string> StringInterpolation { get; }
    }
}
