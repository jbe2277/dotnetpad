namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal static class TemplateCode
    {
        public static string InitialCSharpCode
        {
            get
            {
                return
                    @"using System;
using System.Linq;

namespace Sample
{
    internal static class Program
    {
        internal static void Main()
        {
            
        }
    }
}";
            }
        }

        public static int StartCaretPositionCSharp { get { return 160; } }

        public static string InitialVisualBasicCode
        {
            get
            {
                return
                    @"Imports System
Imports System.Linq

Namespace Sample
    Module Program
        Sub Main()
            
        End Sub
    End Module
End Namespace";
            }
        }

        public static int StartCaretPositionVisualBasic { get { return 110; } }
    }
}
