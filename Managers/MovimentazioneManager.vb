Imports NTSInformatica

Public Class MovimentazioneManager


    Dim _Settings As Settings
    Private _oCleBoll As CLEVEBOLL
    Public Sub New(Settings As Settings, oCleBoll As CLEVEBOLL)
        _Settings = Settings
        _oCleBoll = oCleBoll
    End Sub

    Public Function GetSerie(Macchina As String, CodArt As String) As String

        Dim PrimoCar As Char = CodArt(0)
        Dim Serie As String = ""
        Select Case Macchina
            Case GlobalConstants.MACHINENAME_MECCANOPLASTICA1, GlobalConstants.MACHINENAME_MECCANOPLASTICA4
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                    Case "5"c
                        Serie = "/P"
                End Select
            Case GlobalConstants.MACHINENAME_DUETTI, GlobalConstants.MACHINENAME_DUETTI2
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                    Case "F"c
                        If CodArt.Length >= 3 AndAlso CodArt.Substring(0, 3) = "FAA" Then
                            Serie = "CON"
                        Else
                            Throw New Exception($"Articolo {CodArt} processato dalla macchina {Macchina} non riconosciuto per ricavarne la serie")
                        End If
                    Case "P"c
                        Serie = "PAG"
                End Select
            Case GlobalConstants.MACHINENAME_AXOMATIC
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                End Select
            Case GlobalConstants.MACHINENAME_ETICH
                Select Case PrimoCar
                    Case "M"c, "C"c, "P"c
                        Serie = "ET"
                End Select
            Case GlobalConstants.MACHINENAME_LAY
                Select Case PrimoCar
                    Case "2"c
                        Serie = "LY"
                End Select
            Case GlobalConstants.MACHINENAME_PICKER2317
                Select Case PrimoCar
                    Case "5"c, "M"c, "P"c
                        Serie = "FL"
                End Select
            Case GlobalConstants.MACHINENAME_PICKER2318
                Select Case PrimoCar
                    Case "5"c, "C"c, "M"c, "P"c
                        Serie = "FL"
                End Select
            Case GlobalConstants.MACHINENAME_PRONTOWASH1, GlobalConstants.MACHINENAME_PRONTOWASH2

                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                End Select
            Case Else
                Throw New Exception($"Articolo {CodArt} processato dalla macchina {Macchina} non riconosciuto per ricavarne la serie")
        End Select

        Return Serie

    End Function

    Public Function LegNuma(Tipo As String, Serie As String, Anno As Integer) As Integer
        Return _oCleBoll.LegNuma(Tipo, Serie, Anno)
    End Function

    Public Function ApriDoc(Ditta As String, Nuovo As Boolean, Tipo As String, Anno As Integer, Serie As String, NumDoc As Integer, Ds As DataSet) As Boolean
        Return _oCleBoll.ApriDoc(Ditta, Nuovo, Tipo, Anno, Serie, NumDoc, Ds)
    End Function

    Public Function NuovoDocumento(Ditta As String, Tipo As String, Anno As Integer, Serie As String, NumDoc As Integer, Modello As String)
        Return _oCleBoll.NuovoDocumento(Ditta, Tipo, Anno, Serie, NumDoc, Modello)
    End Function

    Public Function SalvaDocumento(flagPick As String) As Boolean
        Return _oCleBoll.SalvaDocumento(flagPick)
    End Function

    Public Sub ResetVar()
        _oCleBoll.ResetVar()
    End Sub

    Public Sub SetApriDocSilent(apri As Boolean)
        _oCleBoll.bInApriDocSilent = apri
    End Sub
    Public Sub SetNuovoDocSilent(nuovo As Boolean)
        _oCleBoll.bInNuovoDocSilent = nuovo
    End Sub

    Public ReadOnly Property DsShared As DataSet
        Get
            Return _oCleBoll.dsShared
        End Get
    End Property

    Public Property StrVisNoteConto As String
        Get
            Return _oCleBoll.strVisNoteConto
        End Get
        Set(value As String)
            _oCleBoll.strVisNoteConto = value
        End Set

    End Property

    Public Property BCreaFilePick As Boolean
        Get
            Return _oCleBoll.bCreaFilePick
        End Get
        Set(value As Boolean)
            _oCleBoll.bCreaFilePick = value
        End Set
    End Property

    Dim _ProgProd As Integer
    Public Property ProgProd As Integer
        Get
            Return _ProgProd
        End Get
        Set(value As Integer)
            _ProgProd = value
        End Set
    End Property

    Dim _Serie As String
    Public Property Serie As String
        Get
            Return _Serie
        End Get
        Set(value As String)
            _Serie = value
        End Set
    End Property

    Public Sub CreaTestataProd(
   setupRow As Action(Of DataRow)   ' <--- delegato passato dall’esterno
)

        Dim row As DataRow = _oCleBoll.dttET.Rows(0)

        ' eseguo il codice esterno
        setupRow(row)

        ' validazione interna
        If Not _oCleBoll.OkTestata Then
            Throw New Exception($"Errore nella creazione della testata del documento. Dettagli:num doc {row("et_numdoc")} serie {row("et_serie")} anno {row("et_anno")}")
        End If


    End Sub

    Public Sub CreaRigaProd(oCorpoCaricoProd As CorpoCaricoProd, oLottoDto As LottoDto)

        'Creo una nuova riga di corpo setto i principali campi poi setto tutti gli altri
        'todo:ma qui la casuale ed il magazzino va messo??  

        If Not _oCleBoll.AggiungiRigaCorpo(False, CLN__STD.NTSCStr(oCorpoCaricoProd.Codditt), 0, 0) Then
            Throw New Exception($"Errore nella creazione del corpo del documento. Dettagli: articolo {oLottoDto.StrCodart} qta prodotte {oCorpoCaricoProd.ec_colli} lotto {oLottoDto.StrLottox}")
        End If

        With _oCleBoll.dttEC.Rows(_oCleBoll.dttEC.Rows.Count - 1)
            !ec_colli = oCorpoCaricoProd.ec_colli
            If Not oLottoDto Is Nothing Then
                !ec_lotto = oLottoDto.LLotto
            End If
        End With

        If Not _oCleBoll.RecordSalva(_oCleBoll.dttEC.Rows.Count - 1, False, Nothing) Then
            _oCleBoll.dttEC.Rows(_oCleBoll.dttEC.Rows.Count - 1).Delete()
            Throw New Exception($"Errore nel salvataggio del corpo del documento. Dettagli: articolo {oLottoDto.StrCodart} qta prodotte {oCorpoCaricoProd.ec_colli} lotto {oLottoDto.StrLottox}")
        End If

        _oCleBoll.dtrHT = Nothing

    End Sub
    Public Sub SettaPiedeProd()
        If _oCleBoll.CalcolaTotali() = False Then
            Throw New Exception("Errore nel calcolo dei totali del piede corpo")
        End If
    End Sub
End Class
