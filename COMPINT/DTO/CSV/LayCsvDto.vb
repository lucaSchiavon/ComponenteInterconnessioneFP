Public Class LayCsvDto

    Public Property DataOra As DateTime
    Public Property Ricetta As String
    Public Property PalletCompleti As Integer
    Public Property PalletIncompleti As Integer
    Public Property PalletVuoti As Integer

    Public Property Note As String
    Public ReadOnly Property CodiceArticolo As String
        Get
            If String.IsNullOrWhiteSpace(Ricetta) Then
                Return String.Empty
            End If

            ' Prende tutto ciò che c'è prima del primo ' ' (es: "MOUSE" da "MOUSE-sturaazzurro02-22")
            Dim parts = Ricetta.Split(" "c)
            Return parts(0)
        End Get
    End Property


End Class
