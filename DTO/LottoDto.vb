Public Class LottoDto
    ' Codice articolo
    Public Property StrCodart As String

    ' Descrizione codice articolo
    Public Property StrDescodart As String

    ' Lotto numerico
    Public Property LLotto As Long

    ' Lotto esteso come stringa
    Public Property StrLottox As String

    ' Data di scadenza del lotto
    Public Property DataScadenza As Date

    Public Property DataCreazione As Date

    Public Property LottoGiaPresente As Boolean

    ' Costruttore opzionale per inizializzare il DTO
    Public Sub New(Optional codart As String = "",
                   Optional descodart As String = "",
                   Optional lotto As Long = 0,
                   Optional lottox As String = "",
                   Optional dataCreazione As Date = Nothing,
                   Optional dataScadenza As Date = Nothing)
        Me.StrCodart = codart
        Me.StrDescodart = descodart
        Me.LLotto = lotto
        Me.StrLottox = lottox

        If dataCreazione = Nothing Then
            Me.DataCreazione = Date.MinValue
        Else
            Me.DataCreazione = dataCreazione
        End If
        ' Se non viene passata una data, la inizializziamo con la data minima
        If dataScadenza = Nothing Then
            Me.DataScadenza = Date.MinValue
        Else
            Me.DataScadenza = dataScadenza
        End If


    End Sub
End Class
