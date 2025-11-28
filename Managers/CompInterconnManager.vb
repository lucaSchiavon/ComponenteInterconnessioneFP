Imports NTSInformatica
Imports System.Threading.Thread
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions

Public Class CompInterconnManager

    Public oApp As CLE__APP = Nothing
    Public oMenu As CLE__MENU = Nothing
    Public oCleBoll As CLEVEBOLL = Nothing
    Public oCleAnlo As CLEMGANLO = Nothing
    Dim logger As LogManager = Nothing

    Public Sub Main()



        Try

            'legge tutti i settings dal config ini
            Dim manager As New SettingsManager(Path.Combine(Environment.CurrentDirectory, "") & "\COMPINT_Settings.ini")
            Dim settings As Settings = manager.LoadSettings()

            'istanzia la repository per l'inserimento dei log
            logger = New LogManager(settings)

            Dim MsgInizio As String = "JOB INIZIATO alle " & Date.Now
            Console.WriteLine(MsgInizio)
            logger.LogInfo(MsgInizio)

            'definisce il formato data per la cultura corrente
            General.SetDataFormatForCurrentCulture()

            'avvia l'environment Experience 5
            Dim TipoAccesso As String = settings.ExpAuthType
            AvviaFrameworkBus(settings, TipoAccesso)

            'inizia il job
            ExecuteJob(settings, logger)

            Dim MsgFine As String = "JOB FINITO alle " & Date.Now
            logger.LogInfo(MsgFine)
            Console.WriteLine(MsgFine)

            'se siamo in modalità debug non chiude la console ma la lascia aperta
            If settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG Then
                Console.Read()
            End If

        Catch ex As Exception
            'qui loggo in una tabella tutti gli errori
            'in una gestione centralizzata
            Try

                logger.LogError(ex.Message, ex.StackTrace)

            Catch ex2 As Exception
                'nell'eventualità rara si impianti subito prima ancora di istanziare il logger
                'stampiamo a console quello che è successo e manteniamo aperta la console per poter leggere l'errore
                'a video
                Console.WriteLine("Errore in apertura del file Settings.ini, chiudere la console e risolvere l'errore, il job non partirà fino a che  l'errore non sarà risolto")
                Console.ReadLine()
            End Try

        End Try
    End Sub


#Region "routine per avvio e gestione framework"

    Private Sub AvviaFrameworkBus(settings As Settings, ByVal TipoAccesso As String)

        Dim bOk As Boolean = False
        Dim strError As String = ""
        'inizializza menu e ci aggancia la gestione degli eventi
        oMenu = New CLE__MENU
        AddHandler oMenu.RemoteEvent, AddressOf GestisciEventiEntity
        'todo: qui problema in caso di profilo errato... mostra una msgbox anche se siamo in una console applicatio (app.batch=true non è ancora settato...troppo presto per farlo
        'Dim strBusOperatore As String
        'Dim strBusPasswordOperatore As String
        'If bTrusted Then
        '    '  strBusOperatore = "TRUSTED"
        '    '  strBusPasswordOperatore = "TRUSTED"
        '    bOk = oMenu.Init("TRUSTED" & " " & "TRUSTED" & " " & settings.ExpAuthDatabase & " " & settings.ExpAuthProfile, oApp, Nothing, strError)

        '    'strBusOperatore = oApp.User.Nome
        '    'strBusPasswordOperatore = oApp.User.Pwd
        'Else
        '    bOk = oMenu.Init(strBusOperatore & " " & If(strBusPasswordOperatore = "", ".", strBusPasswordOperatore) & " " & settings.ExpAuthDatabase & " " & settings.ExpAuthProfile, oApp, Nothing, strError)
        'End If

        Select Case TipoAccesso.ToUpper()
            Case "TRUSTED"
                bOk = oMenu.Init("TRUSTED" & " " & "TRUSTED" & " " & settings.ExpAuthDatabase & " " & settings.ExpAuthProfile, oApp, Nothing, strError)
            Case "OFFLINE"
                bOk = oMenu.Init(settings.ExpAuthUserNameOffLineAuth & " " & If(settings.ExpAuthUserNamePwdOffLineAuth = "", ".", settings.ExpAuthUserNamePwdOffLineAuth) & " " & settings.ExpAuthDatabase & " " & settings.ExpAuthProfile, oApp, Nothing, strError)
                'Dim gstrUtente As String = strBusOperatore
                Dim gstrUtente As String = settings.ExpAuthUserNameOffLineAuth
                'in exp sulla funzione CheckOperatore c'è un 4° parametro che va messo a true per fare l'accesso offline
                Select Case CLN__STD.NTSCInt(oMenu.CheckOperatore(settings.ExpAuthUserNameOffLineAuth, settings.ExpAuthUserNamePwdOffLineAuth, gstrUtente, True))
                    Case -1
                        Throw New Exception($"Nome utente {settings.ExpAuthUserNameOffLineAuth} o password non validi.")
                    Case -2
                        Throw New Exception($"L'operatore {settings.ExpAuthUserNameOffLineAuth} è stato disabilitato. Contattare l'amministratore.")
                    Case 1
                        Throw New Exception($"La password per l'utente {settings.ExpAuthUserNameOffLineAuth} è scaduta.")
                    Case 9
                        If settings.ExpAuthUserNamePwdOffLineAuth = "" OrElse settings.ExpAuthUserNamePwdOffLineAuth = "nts" Then
                            'Una password temporanea "" o "nts" può capitare solo con i dati del database che distribuiamo, in questo caso do un messaggio specifico
                            Throw New Exception("Questo è il primo accesso al sistema da parte dell'operatore preconfigurato '|" & gstrUtente & "|'." & vbCrLf &
                                                "A seguito della normativa GDPR (Regolamento Ue 2016/679 in materia di protezione dati personali) occorre generare una nuova password.")
                        Else
                            Throw New Exception($"L'accesso per l'utente {settings.ExpAuthUserNameOffLineAuth} è stato eseguito con una password temporanea.")

                        End If
                End Select
                'per completare l'accesso offline è necessario aggiungere nache questa riga
                oApp.InitParametri(settings.ExpAuthUserNameOffLineAuth, settings.ExpAuthUserNamePwdOffLineAuth, settings.ExpAuthDatabase, settings.ExpAuthProfile, oApp.AvvioProgramma, oApp.AvvioRestrict, oApp.AvvioProgrammaParametri, oApp.Batch, True)

        End Select


        If bOk = False Then
            If strError = "" Then
                strError = "Errore durante l'inizializzazione del menu oMenu.Init"
            End If
            Throw New Exception(strError)
        End If

#Region "accesso offline"
        'per fare su exp l'accesso offline mantenere questa parte ->

        'Dim bAccessoOffline As Boolean = False
        'If bAccessoOffline Then

        '    Dim gstrUtente As String = strBusOperatore
        '    'in exp sulla funzione CheckOperatore c'è un 4° parametro che va messo a true per fare l'accesso offline
        '    Select Case CLN__STD.NTSCInt(oMenu.CheckOperatore(strBusOperatore, strBusPasswordOperatore, gstrUtente, bAccessoOffline))

        '        Case -1
        '            strError = "Nome utente o password non validi."
        '            Throw New Exception(strError)
        '        Case -2
        '            strError = "L'operatore è stato disabilitato. Contattare l'amministratore."
        '            Throw New Exception(strError)
        '        Case 1
        '            strError = "La password è scaduta."
        '            Throw New Exception(strError)

        '        Case 9
        '            If strBusPasswordOperatore = "" OrElse strBusPasswordOperatore = "nts" Then
        '                'Una password temporanea "" o "nts" può capitare solo con i dati del database che distribuiamo, in questo caso do un messaggio specifico
        '                strError = "Questo è il primo accesso al sistema da parte dell'operatore preconfigurato '|" & gstrUtente & "|'." & vbCrLf &
        '                      "A seguito della normativa GDPR (Regolamento Ue 2016/679 in materia di protezione dati personali) occorre generare una nuova password."
        '                Throw New Exception(strError)
        '                'Else
        '                '    strMessaggioVideo = "L'accesso è stato eseguito con una password temporanea."

        '            End If

        '    End Select
        '    'per completare l'accesso offline è necessario aggiungere nache questa riga
        '    oApp.InitParametri(strBusOperatore, strBusPasswordOperatore, strBusDatabase, strBusProfilo, oApp.AvvioProgramma, oApp.AvvioRestrict, oApp.AvvioProgrammaParametri, oApp.Batch, True)

        'End If

#End Region


        bOk = oMenu.InitEx(strError, Nothing)

        CLN__STD.bNoLoadPers = True 'non carica le perosnalizzazioni


        If bOk = False Then
            If strError = "" Then strError = "Errore su oMenu.InitEx"
            Throw New Exception(strError)
        End If

        bOk = oMenu.SetAziendaDitta(settings.ExpAuthDatabase, "", strError)


        If bOk = False Then
            If strError = "" Then strError = "Errore su settaggio dell'azienda"
            Throw New Exception(strError)
        End If

        'SetLookAndFeel()
        bOk = oMenu.Start(strError)


        If bOk = False Then
            If strError = "" Then strError = "Errore sullo start del menu"
            Throw New Exception(strError)
        End If

        oApp.SetDittaCorrente(settings.ExpAuthDittaCorrente)
        oApp.DbAp.QueryTimeOut = 36000
        oApp.bWriteQueryInLogFile = False : oApp.bNoWriteQueryInLog = True
        oApp.Batch = True

    End Sub

    Public Overridable Sub GestisciEventiEntity(ByVal sender As Object, ByRef e As NTSEventArgs)
        Dim fErr As StreamWriter = Nothing
        Dim fiErr As FileInfo = Nothing
        Dim strNomeFileLog As String = ""
        Dim codart As String = ""

        'If oCleBoll.dttEC.Rows.Count > 0 Then
        '    codart = CStr(oCleBoll.dttEC.Rows(oCleBoll.dttEC.Rows.Count).Item("ec_codart"))
        'End If

        'forzare la data della distinta base a oggi
        If e.TipoEvento = "DataValDB.:" Then
            'Questa gestione serve per creare i carichi di produzione
            e.RetValue = CLN__STD.NTSCDate(oCleBoll.dttET.Rows(0)!et_datdoc.ToString).ToShortDateString
        Else
            'magari poi inserire anche l'istruzione sotto, così un volta loggati li giriamo comunque alla UI
            'todo:qui cosa inserire??
        End If

        If e.TipoEvento = "" Then
            'guardo se message contiene erroroccorred ed in caso affermativo loggo
            If e.Message.ToUpper.Contains("ERROR OCCURED:") Then
                logger.LogError(e.Message, Environment.StackTrace, "", codart)
            End If
        End If

        'todo:questo il codice che metterei al posto di quello che c'è sotto
        If e.TipoEvento = CLN__STD.ThMsg.MSG_ERROR Then
            'loggare sempre e comunque ERROR
            'logger.LogError(e.TipoEvento, "GestisciEventiEntity")
            logger.LogError(e.Message, Environment.StackTrace, "", codart)
            'oCleBoll.dttEC.Rows.Count

            'cstr(oCleBoll.dttET.Rows(0).Item("et_conto"))
            'logger.LogError(Environment.StackTrace, "GestisciEventiEntity")
            'e.Message è vuoto mettere errore se message è vuto tendenzialmente ci sono altre info su e ma tendenzialmente e.Message non succede mai in errore
            'Else

        End If
        If e.TipoEvento = CLN__STD.ThMsg.MSG_INFO Or e.TipoEvento = CLN__STD.ThMsg.MSG_INFO_POPUP Then
            If e.Message <> "" Then
                logger.LogInfo("Errore non specificato", "", codart)
            End If
            'quando non ho a che fare con un errore se ho un messaggio loggo INFO
            'talvolta qui, a differenza di error, sono frequenti i messaggi vuoti
            'in questo caso non loggo nulla
        End If


        'If e.Message.ToUpper.Contains("ERROR OCCURED:") Then
        '    'Throw New Exception(e.Message)
        '    ErroreInGestisciEventiEntity = True
        '    StrErroreInGestisciEventiEntity = e.Message
        'End If

    End Sub

#End Region

#Region "Inizializzazioni dll Experience"

    Public Overridable Sub InizializzaBeveboll()

        If Not oCleBoll Is Nothing Then Exit Sub
        'inizializzo BEVEBOLL per carico produzione
        Dim strErr As String = ""
        Dim oTmp As Object = Nothing
        If CLN__STD.NTSIstanziaDll(oApp.ServerDir, oApp.NetDir, "BNHHESEM", "BEVEBOLL", oTmp, strErr, False, "", "") = False Then
            Throw New Exception("ERRORE in fase di creazione Entity:" & vbCrLf & "|" & strErr & "|")
        End If

        oCleBoll = CType(oTmp, CLEVEBOLL)

        AddHandler oCleBoll.RemoteEvent, AddressOf GestisciEventiEntity
        If oCleBoll.Init(oApp, Nothing, oMenu.oCleComm, "", False, "", "") = False Then Throw New Exception("Entity oCleBoll non è stata inizializzata")
        If Not oCleBoll.InitExt() Then Throw New Exception("Metodo oCleBoll.InitExt fallito")
        oCleBoll.bModuloCRM = False
        oCleBoll.bIsCRMUser = False

    End Sub
    Public Overridable Sub InizializzaBemganlo()

        If Not oCleAnlo Is Nothing Then Exit Sub
        'inizializzo BEMGANLO per gestione lotti
        Dim strErr As String = ""
        Dim oTmp As Object = Nothing
        If CLN__STD.NTSIstanziaDll(oApp.ServerDir, oApp.NetDir, "BNMGANLO", "BEMGANLO", oTmp, strErr, False, "", "") = False Then
            Throw New Exception("ERRORE in fase di creazione Entity:" & vbCrLf & "|" & strErr & "|")
        End If
        oCleAnlo = CType(oTmp, CLEMGANLO)

        AddHandler oCleAnlo.RemoteEvent, AddressOf GestisciEventiEntity
        If oCleAnlo.Init(oApp, Nothing, CType(oApp.oMenu, CLE__MENU).oCleComm, "", False, "", "") = False Then Throw New Exception("Entity oCleAnlo non è stata inizializzata")
    End Sub

#End Region



    Public Overridable Sub ExecuteJob(settings As Settings, logger As LogManager)

        'inizializzo le entities di experience che mi serviranno per inserire lotti
        'e carichi di produzione
        InizializzaBeveboll()
        InizializzaBemganlo()

        'processa i file csv della cartella della macchina di produzione Ica2:
        'Meccanoplastica1CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Prontowash1:
        ICAVL08615CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Prontowash1:
        ICAVL08616CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)



    End Sub



#Region "Meccanoplastica1"

    Public Overridable Sub Meccanoplastica1CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.Meccanoplastica1Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica1Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica1Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.Meccanoplastica1Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica1Percorso, "*.csv")
            End If

            Dim ObjMeccanoplastica1CsvDto As Meccanoplastica1CsvDto = Nothing

            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjMeccanoplastica1CsvDto = Meccanoplastica1CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjMeccanoplastica1CsvDto.CodiceArticolo)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjMeccanoplastica1CsvDto.DataOra


                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjMeccanoplastica1CsvDto.CodiceArticolo, settings.Meccanoplastica1NomeMacchina, ObjMeccanoplastica1CsvDto.DataOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjMeccanoplastica1CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjMeccanoplastica1CsvDto.CodiceArticolo),
                          .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(NextLottoNumber),
                          .StrLottox = NomeLotto,
                          .DataCreazione = CLN__STD.NTSCDate(Oggi),
                          .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                          .LottoGiaPresente = False
                      }
                        End If

                    End If

                    'crea il lotto e un carico di produzione per l'articolo del csv
                    Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
                    Meccanoplastica1ExecCdp(OMovimentazioneManager, OLottoManager, ObjMeccanoplastica1CsvDto, oLottoDto, settings, logger, PublicCurrFileName)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        SpostaECopiaFile(FilePath, Oggi, settings.Meccanoplastica1Percorso, settings.Meccanoplastica1PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica1NomeMacchina, If(ObjMeccanoplastica1CsvDto IsNot Nothing, ObjMeccanoplastica1CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.Meccanoplastica1PercorsoErrori)

                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try

                    'LogManager.ScriviLog($"Errore durante l'elaborazione di {file} meccanoplastica: {ex.Message}{Environment.NewLine}{ex.StackTrace}")
                End Try

            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica1NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub Meccanoplastica1ExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oMeccanoplastica1CsvDto As Meccanoplastica1CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        'Dim OMovimentazioneManager As New MovimentazioneManager()
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente Then
            'creo il lotto
            oLottoManager.CreaLotto(oLottoDto)
        End If

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_MECCANOPLASTICA1, oMeccanoplastica1CsvDto.CodiceArticolo)
        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oMeccanoplastica1CsvDto.DataOra.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oMeccanoplastica1CsvDto.DataOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica1CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica1CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica1CsvDto.PezziBuoni}")
        End If

        OMovimentazioneManager.SetApriDocSilent(True)

        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica1CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica1CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica1CsvDto.PezziBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oMeccanoplastica1CsvDto.DataOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica1CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica1CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica1CsvDto.PezziBuoni}")
        End If
        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "T"
            r!et_anno = oMeccanoplastica1CsvDto.DataOra.Year
            r!et_serie = Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oMeccanoplastica1CsvDto.Note
            r!et_datdoc = oMeccanoplastica1CsvDto.DataOra
            r!et_tipobf = settings.TipoBf
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.Codditt = oMeccanoplastica1CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oMeccanoplastica1CsvDto.PezziBuoni

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        OMovimentazioneManager.SettaPiedeProd()

        OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica1CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica1CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica1CsvDto.PezziBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie
        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oMeccanoplastica1CsvDto.DataOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oMeccanoplastica1CsvDto.PezziBuoni} per il prodotto {oMeccanoplastica1CsvDto.CodiceArticolo} ", settings.Meccanoplastica1NomeMacchina, oMeccanoplastica1CsvDto.CodiceArticolo, PublicCurrFileName)
    End Sub

#End Region

#Region "PRONTOWASH1"

    Public Overridable Sub ICAVL08615CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.ICAVL08615Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08615Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08615Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.ICAVL08615Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08615Percorso, "*.csv")
            End If

            Dim ObjIca1CsvDto As ICAVL08615CsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjIca1CsvDto = ICAVL08615CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjIca1CsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjIca1CsvDto.FineTurno

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjIca1CsvDto.CodiceArticolo, settings.ICAVL08615NomeMacchina, ObjIca1CsvDto.FineTurno)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjIca1CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjIca1CsvDto.CodiceArticolo),
                            .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(NextLottoNumber),
                            .StrLottox = NomeLotto,
                            .DataCreazione = CLN__STD.NTSCDate(Oggi),
                            .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                            .LottoGiaPresente = False
                        }

                        End If


                    End If

                    Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
                    ICAVL08615ExecCdp(OMovimentazioneManager, OLottoManager, ObjIca1CsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.ICAVL08615Percorso, settings.ICAVL08615PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08615NomeMacchina, If(ObjIca1CsvDto IsNot Nothing, ObjIca1CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08615NomeMacchina, If(ObjIca1CsvDto IsNot Nothing, ObjIca1CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.ICAVL08615PercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.ICAVL08615NomeMacchina, If(ObjIca1CsvDto IsNot Nothing, ObjIca1CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08615NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub ICAVL08615ExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oIca1CsvDto As ICAVL08615CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        'Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            oLottoManager.CreaLotto(oLottoDto)
        End If

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PRONTOWASH1, oIca1CsvDto.CodiceArticolo)
        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oIca1CsvDto.FineTurno.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oIca1CsvDto.InizioTurno.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oIca1CsvDto.InizioTurno.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oIca1CsvDto.FineTurno.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oIca1CsvDto.Note
           r!et_datdoc = oIca1CsvDto.FineTurno
           r!et_tipobf = settings.TipoBf
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.Codditt = oIca1CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oIca1CsvDto.ScatoleTeoricheProdotte

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oIca1CsvDto.FineTurno.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oIca1CsvDto.ScatoleTeoricheProdotte} per il prodotto {oIca1CsvDto.CodiceArticolo} ", settings.ICAVL08615NomeMacchina, oIca1CsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region

#Region "PRONTOWASH2"

    Public Overridable Sub ICAVL08616CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.ICAVL08616Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08616Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08616Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.ICAVL08616Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.ICAVL08616Percorso, "*.csv")
            End If

            Dim ObjIca2CsvDto As ICAVL08616CsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjIca2CsvDto = ICAVL08616CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjIca2CsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjIca2CsvDto.FineTurno

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjIca2CsvDto.CodiceArticolo, settings.ICAVL08616NomeMacchina, ObjIca2CsvDto.FineTurno)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjIca2CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjIca2CsvDto.CodiceArticolo),
                            .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(NextLottoNumber),
                            .StrLottox = NomeLotto,
                            .DataCreazione = CLN__STD.NTSCDate(Oggi),
                            .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                            .LottoGiaPresente = False
                        }

                        End If


                    End If

                    Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
                    ICAVL08616ExecCdp(OMovimentazioneManager, OLottoManager, ObjIca2CsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.ICAVL08616Percorso, settings.ICAVL08616PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08616NomeMacchina, If(ObjIca2CsvDto IsNot Nothing, ObjIca2CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08616NomeMacchina, If(ObjIca2CsvDto IsNot Nothing, ObjIca2CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.ICAVL08616PercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.ICAVL08616NomeMacchina, If(ObjIca2CsvDto IsNot Nothing, ObjIca2CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08616NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub ICAVL08616ExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oIca2CsvDto As ICAVL08616CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        'Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            oLottoManager.CreaLotto(oLottoDto)
        End If

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PRONTOWASH1, oIca2CsvDto.CodiceArticolo)
        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oIca2CsvDto.FineTurno.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oIca2CsvDto.InizioTurno.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oIca2CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca2CsvDto.CodiceArticolo} QtaProdotte {oIca2CsvDto.ScatoleTeoricheProdotte}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca2CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca2CsvDto.CodiceArticolo} QtaProdotte {oIca2CsvDto.ScatoleTeoricheProdotte}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oIca2CsvDto.InizioTurno.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oIca2CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca2CsvDto.CodiceArticolo} QtaProdotte {oIca2CsvDto.ScatoleTeoricheProdotte}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oIca2CsvDto.FineTurno.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oIca2CsvDto.Note
           r!et_datdoc = oIca2CsvDto.FineTurno
           r!et_tipobf = settings.TipoBf
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.Codditt = oIca2CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oIca2CsvDto.ScatoleTeoricheProdotte

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oIca2CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca2CsvDto.CodiceArticolo} QtaProdotte {oIca2CsvDto.ScatoleTeoricheProdotte}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oIca2CsvDto.FineTurno.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oIca2CsvDto.ScatoleTeoricheProdotte} per il prodotto {oIca2CsvDto.CodiceArticolo} ", settings.ICAVL08616NomeMacchina, oIca2CsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region


#Region "Routines private"

    Public Sub SpostaECopiaFile(FilePath As String, Oggi As DateTime, PercorsoCorrente As String, PercorsoOld As String, OMovimentazioneManager As MovimentazioneManager)
        ' Verifica ed eventualmente crea la cartella "Old"
        If Not Directory.Exists(PercorsoOld) Then
            Directory.CreateDirectory(PercorsoOld)
        End If

        ' Percorso del file da spostare in "Old"
        Dim OldFilePath As String = Path.Combine(PercorsoOld, Path.GetFileName(FilePath))

        ' Data in formato richiesto
        Dim DataCaricoERP As String = Oggi.ToString("dd_MM_yyyy")

        ' Nome e percorso della copia
        Dim FileCopiaNome As String = $"{Path.GetFileName(FilePath)}_{OMovimentazioneManager.ProgProd}_{OMovimentazioneManager.Serie}_{DataCaricoERP}.csv"
        Dim FileCopiaPath As String = Path.Combine(PercorsoCorrente, FileCopiaNome)

        ' Copia del file
        File.Copy(FilePath, FileCopiaPath, overwrite:=True)

        ' Spostamento del file originale in "Old"
        File.Move(FilePath, OldFilePath)
    End Sub

    Public Sub SpostaNellaCartellaErrori(FilePath As String, PercorsoErrori As String)
        ' Crea la directory Errori se non esiste
        If Not Directory.Exists(PercorsoErrori) Then
            Directory.CreateDirectory(PercorsoErrori)
        End If

        ' Percorso del file nella cartella Errori
        Dim errorFile As String = Path.Combine(PercorsoErrori, Path.GetFileName(FilePath))

        ' Sposta il file nella cartella Errori
        File.Move(FilePath, errorFile)
    End Sub
#End Region



End Class
