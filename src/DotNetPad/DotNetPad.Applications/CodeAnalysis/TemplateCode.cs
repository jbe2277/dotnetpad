namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal static class TemplateCode
    {
        public static string StartCaretIndicator
        {
            get
            {
                return "@StartCaretPosition@";
            }
        }

        public static string InitialCSharpCode
        {
            get
            {
                return
                    @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sample
{
    internal static class Program
    {
        internal static void Main()
        {
            @StartCaretPosition@
        }
    }
}";
            }
        }

        public static string InitialVisualBasicCode
        {
            get
            {
                return
                    @"Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.IO

Namespace Sample
    Module Program
        Sub Main()
            @StartCaretPosition@
        End Sub
    End Module
End Namespace";
            }
        }
    }
}
