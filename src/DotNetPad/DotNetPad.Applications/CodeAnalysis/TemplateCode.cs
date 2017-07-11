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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static int StartCaretPositionCSharp { get { return 226; } }

        public static string InitialVisualBasicCode
        {
            get
            {
                return
                    @"Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace Sample
    Module Program
        Sub Main()
            
        End Sub
    End Module
End Namespace";
            }
        }

        public static int StartCaretPositionVisualBasic { get { return 177; } }
    }
}
