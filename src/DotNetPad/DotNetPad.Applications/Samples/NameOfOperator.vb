Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Waf.DotNetPad.Samples
    Module NameOfOperator
        Private list As ObservableCollection(Of String)

        Sub Main()
            ' Use nameof to compare with the property name provided by the event args.
            list = New ObservableCollection(Of String)
            AddHandler CType(list, INotifyPropertyChanged).PropertyChanged, AddressOf ListPropertyChanged
            list.Add("Luke")
            list.Add("Han")

            ' Use nameof with the ArgumentNullException constructor.
            ArgumentNullCheck(Nothing)
        End Sub

        Sub ListPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            If (e.PropertyName = nameof(list.Count)) Then
                Console.WriteLine("Count: " + list.Count.ToString)
            End If
        End Sub

        Sub ArgumentNullCheck(list As IReadOnlyList(Of String))
            If (list Is Nothing) Then
                Throw New ArgumentNullException(nameof(list))
            End If
        End Sub
    End Module
End Namespace