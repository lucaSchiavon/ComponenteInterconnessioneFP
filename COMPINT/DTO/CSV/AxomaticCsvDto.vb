Public Class AxomaticCsvDto

    Public Property Data_Ora As DateTime
    Public Property NomeRicetta As String
    Public Property PezziBuoni As Integer
    Public Property PezziScartati As Integer
    Public Property PezziCartone As Integer
    Public Property CartoniBuoni As Integer

    Public Property Note As String
    Public ReadOnly Property CodiceArticolo As String
        Get
            If String.IsNullOrWhiteSpace(NomeRicetta) Then
                Return String.Empty
            End If

            ' Prende tutto ciò che c'è prima del primo ' ' (es: "MOUSE" da "MOUSE sturaazzurro02 22")
            Dim parts = NomeRicetta.Split(" "c)
            Return parts(0)
        End Get
    End Property

End Class
