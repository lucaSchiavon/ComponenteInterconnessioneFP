Public Class DuettiCsvDto

    Public Property DataOra As DateTime

    Public Property Ricetta As String
    Public Property CartoniBuoni As Integer
    Public Property CartoniScarto As Integer

    Public Property Note As String
    Public ReadOnly Property CodiceArticolo As String
        Get
            If String.IsNullOrWhiteSpace(Ricetta) Then
                Return String.Empty
            End If

            ' Prende tutto ciò che c'è prima del primo spazio (es: "MOUSE" da "MOUSE-sturaazzurro02-22")
            Dim parts = Ricetta.Split(" "c)
            Return parts(0)
        End Get
    End Property


End Class
