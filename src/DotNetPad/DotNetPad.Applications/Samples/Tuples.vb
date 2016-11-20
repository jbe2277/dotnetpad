Imports System.Collections.Generic
Imports System.Console

Namespace Waf.DotNetPad.Samples
    Module Tuples
        Sub Main()
            ' Create a tuple with semantic names
            Dim namedLetters1 As (alpha As String, beta As String) = ("a", "b")
            Dim namedLetters2 = (alpha:="a", beta:="b")
            WriteLine($"{namedLetters1.alpha} == {namedLetters2.alpha}")

            ' Use method that returns a tuple
            Dim rangeValue = Range(New List(Of Integer) From {8, 2, 4})
            WriteLine($"Min: {rangeValue.min}; Max: {rangeValue.max}")
        End Sub

        Function Range(numbers As IEnumerable(Of Integer)) As (min As Integer, max As Integer)
            Dim min = Integer.MaxValue
            Dim max = Integer.MinValue
            For Each n In numbers
                min = If(n < min, n, min)
                max = If(n > max, n, max)
            Next
            Range = (min, max)			
        End Function
    End Module
End Namespace