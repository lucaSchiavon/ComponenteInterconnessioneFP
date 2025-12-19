
Public Class EtichCsvDto

    Public Property Data As DateTime
    Public Property Tempo As String
    Public Property Nome_Ricetta As String
    Public Property NomeEtichettaGruppo1 As String
    Public Property EtichetteErogateGruppo1 As Integer
    Public Property NomeEtichettaGruppo2 As String
    Public Property EtichetteErogateGruppo2 As Integer


    Public Property Note As String

    'Public ReadOnly Property CodiceArticolo As String
    '    Get
    '        If String.IsNullOrWhiteSpace(Ricetta) Then
    '            Return String.Empty
    '        End If

    '        ' Prende tutto ciò che c'è prima del primo ' ' (es: "MOUSE" da "MOUSE sturaazzurro02-22")
    '        Dim parts = Ricetta.Split(" "c)
    '        Return parts(0)
    '    End Get
    'End Property



End Class
