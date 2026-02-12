Imports NTSInformatica

Public Class LottoManager

    'Dim _OLetteraIdentAnnoRep As LetteraIdentAnnoRep
    'Dim _OGiornoProdConterRep As GiornoProdConterRep
    Dim _OLetteraIdentAnnoRep As ILetteraIdentAnnoRep
    Dim _OGiornoProdConterRep As IGiornoProdConterRep
    Dim _OLottorRep As ILottoRep
    Dim _Settings As Settings
    Public Sub New(Setting As Settings, oCleAnlo As CLEMGANLO)
        'Me._OLetteraIdentAnnoRep = New LetteraIdentAnnoRep(Setting.ConnStr)
        'Me._OGiornoProdConterRep = New GiornoProdConterRep(Setting.ConnStr, Setting)
        'Me._Settings = Setting

        Me.New(Setting,
               New LetteraIdentAnnoRep(Setting.ConnStr),
               New GiornoProdConterRep(Setting.ConnStr, Setting),
               New LottoRep(oCleAnlo))
    End Sub

    'costruttore per unittest
    Public Sub New(setting As Settings,
                   letteraRep As ILetteraIdentAnnoRep,
                   giornoRep As IGiornoProdConterRep,
                   lottorRep As ILottoRep)

        _Settings = setting
        _OLetteraIdentAnnoRep = letteraRep
        _OGiornoProdConterRep = giornoRep
        _OLottorRep = lottorRep
    End Sub

    Public Function GetNomeLotto(CodArt As String, Macchina As String, DataProduzione As DateTime) As String
        Dim NomeLotto As String = ""
        Select Case Macchina.ToUpper()

            Case GlobalConstants.MACHINENAME_MECCANOPLASTICA1
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_MECCANOPLASTICA4
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_DUETTI
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_DUETTI2
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_AXOMATIC
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_ETICH
                NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
            Case GlobalConstants.MACHINENAME_LAY
                NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
            Case GlobalConstants.MACHINENAME_PICKER23017
                NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
            Case GlobalConstants.MACHINENAME_PICKER23018
                NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
            Case GlobalConstants.MACHINENAME_PRONTOWASH1
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
            Case GlobalConstants.MACHINENAME_PRONTOWASH2
                NomeLotto = EstraiNomeLotto(CodArt, DataProduzione)
        End Select

        Return NomeLotto

    End Function

    Public Function IsArtConfForLotto(codditt As String, ar_codart As String) As Boolean
        Return _OLottorRep.IsArtConfForLotto(codditt, ar_codart)
    End Function
    Public Function GetNextLottoNumber(codditt As String) As Integer
        Return _OLottorRep.GetNextLottoNumber(codditt)
    End Function
    Public Function GetLottoProdottoFinito(codditt As String, ar_codart As String, alo_lottox As String) As LottoDto
        Return _OLottorRep.GetLottoProdottoFinito(codditt, ar_codart, alo_lottox)
    End Function

    Public Sub CreaLotto(objLottoDto As LottoDto, Optional DataProdPerGenLottoConter As DateTime = Nothing)
        _OLottorRep.CreaLotto(objLottoDto)
        'solo sei il lotto è di tipo conter si deve aggiornare la tabella dei giorni di produzione conter
        'solo se non esiste un altro
        If objLottoDto.IsLottoConter Then
            _OGiornoProdConterRep.InsertIncrementaleGDiProd(GetGiornoProduzioneFromLotto(objLottoDto.StrLottox), DataProdPerGenLottoConter)
        End If
    End Sub

    Public Function GetDataForLottoName(nomeFile As String) As Date
        'per il nome dei lotti meccanoplastica recuperare la data dal nome del file 
        'e non all'interno del file CSV
        ' Rimuove l'estensione
        Dim senzaExt As String = IO.Path.GetFileNameWithoutExtension(nomeFile)

        ' Divide in parti usando "_" come separatore
        Dim parti() As String = senzaExt.Split("_"c)

        ' La prima parte è la data nel formato yyyy-MM-dd
        Dim dataStr As String = parti(0)

        ' Converte la stringa in un oggetto Date
        Dim dataConvertita As Date = Date.ParseExact(dataStr, "yyyy-MM-dd",
                                                 Globalization.CultureInfo.InvariantCulture)

        Return dataConvertita
    End Function

#Region "metodi privati"
    Private Function GetGiornoProduzioneFromLotto(nomeLotto As String) As Integer
        If String.IsNullOrEmpty(nomeLotto) OrElse nomeLotto.Length <> 12 Then
            Throw New ArgumentException("Nome lotto non valido")
        End If

        Dim giornoProduzioneStr As String = nomeLotto.Substring(7, 4)
        Dim giornoProduzione As Integer

        If Integer.TryParse(giornoProduzioneStr, giornoProduzione) Then
            Return giornoProduzione
        Else
            Throw New FormatException("Il giorno di produzione non è valido nel nome lotto")
        End If
    End Function
    Private Function EstraiNomeLotto(CodArt As String, DataProduzione As DateTime) As String
        Dim NomeLotto As String = ""
        Dim StrErr As String = $"Prodotto con Codice articolo ({CodArt}) non riconosciuto durante la generazione del nome lotto prodotto finito"
        ' Primo carattere
        Dim primoCar As Char = CodArt(0)

        If primoCar = "P"c Then
            'Se inizia con P il nome lotto sarà costituito ad xxxAFP dove le tre lettere
            'sono l'incrementale calendario giuliano, la A la lettera identificatìva dell'anno
            NomeLotto = GetNomeLottoCalGiuliano(DataProduzione)
        ElseIf primoCar = "M"c Then
            'Se inizia con M è prodotto della Bolton, il nome del lotto sarà costituito da Uggmmaa
            NomeLotto = GetNomeLottoConData(DataProduzione)
        ElseIf primoCar = "C"c Then
            'Se inizia con C in tutti i casi il lotto non viene applicato
            NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
        ElseIf primoCar = "F"c Then
            If CodArt.Length >= 3 AndAlso CodArt.Substring(0, 3) = "FAA" Then
                'il nome lotto va generato secondo le specifiche conter
                'AASSGLLBBBBF
                NomeLotto = GetNomeLottoConter(DataProduzione)
            Else
                Throw New Exception(StrErr)
            End If
        ElseIf Char.IsDigit(primoCar) Then
            If primoCar = "5"c Then
                NomeLotto = GetNomeLottoConDataProdConSuff5(DataProduzione)
            ElseIf primoCar = "2"c Then
                NomeLotto = GlobalConstants.LOTTO_NONAPPLICATO
            Else
                Throw New Exception(StrErr)
            End If


        Else
            Throw New Exception(StrErr)
        End If

        Return NomeLotto
    End Function
    Private Function GetNomeLottoConData(data As Date) As String

        'Dim strData As String = "00" & data.ToString("ddMMyy")
        Dim strData As String = data.ToString("ddMMyy")
        Return String.Concat("U", strData)
    End Function
    Private Function GetNomeLottoConDataProdConSuff5(data As Date) As String

        'Dim strData As String = "00" & data.ToString("ddMMyy")
        Dim strData As String = data.ToString("dd/MM/yyyy")
        Return strData
    End Function
    Private Function GetGiornoGiuliano(ByVal dataInput As Date) As Integer
        ' Estraggo giorno, mese e anno
        Dim giorno As Integer = Day(dataInput)
        Dim mese As Integer = Month(dataInput)
        Dim anno As Integer = Year(dataInput)

        ' Verifico se l'anno è bisestile secondo il calendario giuliano
        ' (ogni anno divisibile per 4 è bisestile)
        Dim bisestile As Boolean = (anno Mod 4 = 0)

        ' Array con i giorni per mese (anno normale)
        Dim giorniMese() As Integer = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31}

        ' Se bisestile, febbraio ha 29 giorni
        If bisestile Then
            giorniMese(1) = 29
        End If

        ' Calcolo il numero progressivo
        Dim giornoAnno As Integer = giorno
        For i As Integer = 0 To mese - 2
            giornoAnno += giorniMese(i)
        Next

        Return giornoAnno
    End Function
    Private Function GetNomeLottoCalGiuliano(ByVal DataProduzione As Date) As String

        Dim GiornoGiuliano As Int32 = GetGiornoGiuliano(DataProduzione)
        Dim LetteraIdentAnno As String = _OLetteraIdentAnnoRep.GetLetteraIdentAnno(DataProduzione)
        Dim CampoFissoFp As String = _Settings.NomeLottoPagCampoFissoFp
        Return String.Concat(GiornoGiuliano.ToString("000"), LetteraIdentAnno, CampoFissoFp)

    End Function
    Private Function GetNomeLottoConter(ByVal DataProduzione As Date) As String
        'il nome dovrà avere questo formato:
        'AASSGLLBBBBF
        'AA= anno di confezionamento
        'SS= Numero settimana dell'anno
        'G= giorno della settimana
        'LL= Numero linea di riempimento
        'BBBB giorno di produzione per conter da inizio anno
        'F= lettera identif. FP

        Dim AnnoDiConfezionamento As String = DataProduzione.ToString("yy")
        Dim NumeroSettimanaDellAnno As String = GetIsoWeek(DataProduzione)
        Dim GiornoDellaSettimana As String = CInt(DataProduzione.DayOfWeek).ToString()
        Dim NumeroLineaDiRiempimento As String = _Settings.NumeroLineaDiRiempimento
        Dim GiornoDiProdPerContDaInizioAnno As String = _OGiornoProdConterRep.GetNextProdCounter(DataProduzione)
        Dim LetteraIdentifFp As String = _Settings.LetteraIdentifFp

        Return String.Concat(AnnoDiConfezionamento, NumeroSettimanaDellAnno, GiornoDellaSettimana, NumeroLineaDiRiempimento, GiornoDiProdPerContDaInizioAnno, LetteraIdentifFp)

    End Function
    Private Function GetIsoWeek(ByVal dataRif As Date) As String
        'la prima settimana dell’anno è quella che contiene almeno 4 giorni del nuovo anno (regola ISO-8601).
        'la settimana inizia di lunedì (regola ISO-8601).
        Return Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
            dataRif,
            Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday
        ).ToString("00")
    End Function
#End Region

End Class
