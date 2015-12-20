Imports System

Namespace Waf.DotNetPad.Samples
    Module AutoPropertyInitializers
        Sub Main()
            Dim customer = New Customer(3)
            Console.WriteLine("Customer {0}: {1}, {2}", customer.Id, customer.First, customer.Last)
        End Sub
    End Module

    Class Customer
        Sub New(id As Integer)
            Me.Id = id
        End Sub

        Public ReadOnly Property Id As Integer

        Public Property First As String = "Luke"

        Public Property Last As String = "Skywalker"
    End Class
End Namespace