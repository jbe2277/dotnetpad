namespace Waf.DotNetPad.Applications.CodeAnalysis
{
    internal static class TemplateCode
    {
        public static string InitialCSharpCode =>
            /*lang=c#-test*/ """
            namespace Sample;

            internal static class Program
            {
                private static void Main()
                {
        
                }
            }
            """;

        public static int StartCaretPositionCSharp => 102;

        public static string InitialVisualBasicCode =>
            /*lang=vb.net-test*/ """
            Imports System
            Imports System.Collections.Generic
            Imports System.Linq
            Imports System.Threading.Tasks

            Namespace Sample
                Module Program
                    Sub Main()
            
                    End Sub
                End Module
            End Namespace
            """;

        public static int StartCaretPositionVisualBasic => 177;
    }
}
