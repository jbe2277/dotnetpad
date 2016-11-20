using System;

namespace Waf.DotNetPad.Applications.Services
{
    public interface ICSharpSampleService
    {
        Lazy<string> OutVariables { get; }

        Lazy<string> Tuples { get; }

        Lazy<string> PatternMatching { get; }

        Lazy<string> LocalFunctions { get; }

        Lazy<string> NullConditionalOperator { get; }
        
        Lazy<string> NameOfOperator { get; }

        Lazy<string> AutoPropertyInitializers { get; }

        Lazy<string> StringInterpolation { get; }
    }
}
