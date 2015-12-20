Imports System
Imports System.Console

Namespace Waf.DotNetPad.Samples
    Module StringInterpolation
        Sub Main()
            Dim p = New Person With {.Name = "Luke", .Age = 50}

            WriteLine(String.Format("{0} is {1} year{{s}} old", p.Name, p.Age))

            WriteLine($"{p.Name} is {p.Age} year{{s}} old")
			WriteLine($"{p.Name,20} is {p.Age:D3} year{{s}} old")
			WriteLine($"{p.Name} is {p.Age} year{If (p.Age = 1, "", "s")} old")
        End Sub
    End Module

    Class Person
        Public Property Name As String

        Public Property Age As Integer
    End Class
End Namespace