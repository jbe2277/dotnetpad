Imports System
Imports System.Collections.Generic

Namespace Waf.DotNetPad.Samples
    Module NullConditionalOperator
        Sub Main()
            ' Use ?. to access the Name property
            Dim person As Person = Nothing
            Console.WriteLine("person?.Name: {0}", If(person?.Name, "Nothing"))
            Dim persons As List(Of Person) = Nothing
			Console.WriteLine("persons?(0).Name: {0}", If(persons?(0).Name, "Nothing"))

            person = New Person With {.Name = "Luke"}
            Console.WriteLine("person?.Name: {0}", If(person?.Name, "Nothing"))
            persons = New List(Of Person) From {person}
            Console.WriteLine("persons?(0).Name: {0}", persons?(0).Name, "Nothing")
        End Sub
    End Module

    Class Person
        Public Property Name As String
    End Class
End Namespace