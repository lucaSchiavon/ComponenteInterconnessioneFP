Imports System.Text.RegularExpressions

Public Class Picker23017CsvDto

    Public Property DataEOra As DateTime
    Public Property Ricetta As String
    Public Property PezziBuoni As Integer
    Public Property PezziScarto As Integer

    Public Property Note As String

    Public ReadOnly Property CodiceArticolo As String
        Get
            If String.IsNullOrWhiteSpace(Ricetta) Then
                Return String.Empty
            End If

            ' Divide la stringa in parti separate da spazi
            Dim parts = Ricetta.Split(" "c)

            ' Controlla che ci siano almeno 2 elementi
            If parts.Length >= 2 Then
                Return parts(1)   ' Il codice prodotto è la seconda parola
            End If

            Return String.Empty
        End Get
    End Property


End Class
