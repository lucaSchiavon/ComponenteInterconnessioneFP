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

    Public Function EseguiJobPrincipale() As Integer


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
            If settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG AndAlso Environment.UserInteractive Then
                Console.WriteLine("Premi un tasto per chiudere...")
                Console.Read()
            End If
            Return 0 ' Successo
        Catch ex As Exception
            'qui loggo in una tabella tutti gli errori
            'in una gestione centralizzata
            Try

                logger?.LogError(ex.Message, ex.StackTrace)

            Catch ex2 As Exception
                'nell'eventualità rara si impianti subito prima ancora di istanziare il logger
                'stampiamo a console quello che è successo e manteniamo aperta la console per poter leggere l'errore
                'a video
                Console.WriteLine($"Errore in apertura del file Settings.ini, chiudere la console e risolvere l'errore, il job non partirà fino a che  l'errore non sarà risolto. {ex2.Message}")
                LogTestoManager.LoggaErrore(ex2)
                If Environment.UserInteractive Then Console.ReadLine()
                Return 1 ' Errore
            End Try
            Return 1 ' Errore
        End Try
    End Function


#Region "routine per avvio e gestione framework"

    Private Sub AvviaFrameworkBus(settings As Settings, ByVal TipoAccesso As String)

        Dim bOk As Boolean = False
        Dim strError As String = ""
        'inizializza menu e ci aggancia la gestione degli eventi
        oMenu = New CLE__MENU
        AddHandler oMenu.RemoteEvent, AddressOf GestisciEventiEntity

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
        Dim strNomeFileLog As String = ""
        Dim codart As String = ""
        Try

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
                    'perchè è venuto fuori in produzione
                    logger.LogInfo($"Informazione (MSG_INFO o MSG_INFO_POPUP): {e.Message}", "", codart)
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
        Catch ex As Exception
            'Logga in un file di testo l'errore perchè non riesce a loggare nel db
            LogTestoManager.LoggaErrore(ex)

        End Try
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

        'processa i file csv della cartella della macchina di produzione Meccanoplastica1:
        Meccanoplastica1CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione  Meccanoplastica4:
        Meccanoplastica4CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Duetti:
        DuettiCaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Duetti:
        Duetti2CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione  Axomatic:
        AxomaticCaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione  Layerpack:
        LayScaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        ''processa i file csv della cartella della macchina di produzione  Layerpack:
        EtichScaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione  Layerpack:
        Picker23017ScaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione  Layerpack:
        Picker23018ScaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Prontowash1:
        ICAVL08615CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Prontowash2:
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

                    'se si tratta di macchine meccanoplastica prendo la data dal nome del file
                    Dim Oggi As Date = OLottoManager.GetDataForLottoName(PublicCurrFileName)

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
                        SpostaECopiaFile(FilePath, Oggi, settings.Meccanoplastica1PercorsoOld, OMovimentazioneManager)

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

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_MECCANOPLASTICA1, oMeccanoplastica1CsvDto.CodiceArticolo)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Meccanoplastica1NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Meccanoplastica1NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


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
            r!et_tipobf = settings.TipoBfCaricoScarico
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oMeccanoplastica1CsvDto.CodiceArticolo
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

#Region "Meccanoplastica4"

    Public Overridable Sub Meccanoplastica4CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.Meccanoplastica4Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica4Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica4Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.Meccanoplastica4Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.Meccanoplastica4Percorso, "*.csv")
            End If

            Dim ObjMeccanoplastica4CsvDto As Meccanoplastica4CsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjMeccanoplastica4CsvDto = Meccanoplastica4CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjMeccanoplastica4CsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing

                    'se si tratta di macchine meccanoplastica prendo la data dal nome del file
                    Dim Oggi As Date = OLottoManager.GetDataForLottoName(PublicCurrFileName)

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjMeccanoplastica4CsvDto.CodiceArticolo, settings.Meccanoplastica4NomeMacchina, ObjMeccanoplastica4CsvDto.DataOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjMeccanoplastica4CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjMeccanoplastica4CsvDto.CodiceArticolo),
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
                    Meccanoplastica4ExecCdp(OMovimentazioneManager, OLottoManager, ObjMeccanoplastica4CsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.Meccanoplastica4PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica4NomeMacchina, If(ObjMeccanoplastica4CsvDto IsNot Nothing, ObjMeccanoplastica4CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica4NomeMacchina, If(ObjMeccanoplastica4CsvDto IsNot Nothing, ObjMeccanoplastica4CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.Meccanoplastica4PercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.Meccanoplastica4NomeMacchina, If(ObjMeccanoplastica4CsvDto IsNot Nothing, ObjMeccanoplastica4CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica4NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub Meccanoplastica4ExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oMeccanoplastica4CsvDto As Meccanoplastica4CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_MECCANOPLASTICA4, oMeccanoplastica4CsvDto.CodiceArticolo)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Meccanoplastica4NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Meccanoplastica4NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oMeccanoplastica4CsvDto.DataOra.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oMeccanoplastica4CsvDto.DataOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica4CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica4CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica4CsvDto.PezziBuoni}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica4CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica4CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica4CsvDto.PezziBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oMeccanoplastica4CsvDto.DataOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica4CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica4CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica4CsvDto.PezziBuoni}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oMeccanoplastica4CsvDto.DataOra.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oMeccanoplastica4CsvDto.Note
           r!et_datdoc = oMeccanoplastica4CsvDto.DataOra
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oMeccanoplastica4CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oMeccanoplastica4CsvDto.PezziBuoni

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oMeccanoplastica4CsvDto.DataOra.Year}, serie {Serie}, prodotto {oMeccanoplastica4CsvDto.CodiceArticolo} QtaProdotte {oMeccanoplastica4CsvDto.PezziBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oMeccanoplastica4CsvDto.DataOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oMeccanoplastica4CsvDto.PezziBuoni} per il prodotto {oMeccanoplastica4CsvDto.CodiceArticolo} ", settings.Meccanoplastica4NomeMacchina, oMeccanoplastica4CsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region

#Region "Axomatic"

    Public Overridable Sub AxomaticCaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.AxomaticPercorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.AxomaticPercorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.AxomaticPercorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.AxomaticPercorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.AxomaticPercorso, "*.csv")
            End If

            Dim ObjAxomaticCsvDto As AxomaticCsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjAxomaticCsvDto = AxomaticCsvParser.ParseProduzioneCsvTollerante(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjAxomaticCsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing

                    Dim Oggi As Date = ObjAxomaticCsvDto.Data_Ora

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjAxomaticCsvDto.CodiceArticolo, settings.Meccanoplastica4NomeMacchina, ObjAxomaticCsvDto.Data_Ora)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjAxomaticCsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjAxomaticCsvDto.CodiceArticolo),
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
                    AxomaticExecCdp(OMovimentazioneManager, OLottoManager, ObjAxomaticCsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.AxomaticPercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.AxomaticNomeMacchina, If(ObjAxomaticCsvDto IsNot Nothing, ObjAxomaticCsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.AxomaticNomeMacchina, If(ObjAxomaticCsvDto IsNot Nothing, ObjAxomaticCsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.AxomaticPercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.AxomaticNomeMacchina, If(ObjAxomaticCsvDto IsNot Nothing, ObjAxomaticCsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.AxomaticNomeMacchina)
        End Try
    End Sub

    Public Overridable Sub AxomaticExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oAxomaticCsvDto As AxomaticCsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_AXOMATIC, oAxomaticCsvDto.CodiceArticolo)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.AxomaticNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.AxomaticNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oAxomaticCsvDto.Data_Ora.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oAxomaticCsvDto.Data_Ora.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oAxomaticCsvDto.Data_Ora.Year}, serie {Serie}, prodotto {oAxomaticCsvDto.CodiceArticolo} QtaProdotte {oAxomaticCsvDto.CartoniBuoni}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oAxomaticCsvDto.Data_Ora.Year}, serie {Serie}, prodotto {oAxomaticCsvDto.CodiceArticolo} QtaProdotte {oAxomaticCsvDto.CartoniBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oAxomaticCsvDto.Data_Ora.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oAxomaticCsvDto.Data_Ora.Year}, serie {Serie}, prodotto {oAxomaticCsvDto.CodiceArticolo} QtaProdotte {oAxomaticCsvDto.CartoniBuoni}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oAxomaticCsvDto.Data_Ora.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oAxomaticCsvDto.Note
           r!et_datdoc = oAxomaticCsvDto.Data_Ora
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oAxomaticCsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oAxomaticCsvDto.CartoniBuoni

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oAxomaticCsvDto.Data_Ora.Year}, serie {Serie}, prodotto {oAxomaticCsvDto.CodiceArticolo} QtaProdotte {oAxomaticCsvDto.CartoniBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oAxomaticCsvDto.Data_Ora.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oAxomaticCsvDto.CartoniBuoni} per il prodotto {oAxomaticCsvDto.CodiceArticolo} ", settings.AxomaticNomeMacchina, oAxomaticCsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region

#Region "Layerpak"

    Public Overridable Sub LayScaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.LayPercorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.LayPercorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.LayPercorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.LayPercorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.LayPercorso, "*.csv")
            End If

            Dim ObjLayCsvDto As LayCsvDto = Nothing

            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjLayCsvDto = LayCsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjLayCsvDto.CodiceArticolo)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing

                    Dim Oggi As Date = ObjLayCsvDto.DataOra

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjLayCsvDto.CodiceArticolo, settings.LayNomeMacchina, ObjLayCsvDto.DataOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjLayCsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjLayCsvDto.CodiceArticolo),
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
                    LayExecScdp(OMovimentazioneManager, OLottoManager, ObjLayCsvDto, oLottoDto, settings, logger, PublicCurrFileName)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        SpostaECopiaFile(FilePath, Oggi, settings.LayPercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.LayNomeMacchina, If(ObjLayCsvDto IsNot Nothing, ObjLayCsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.LayPercorsoErrori)

                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try
                End Try

            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.LayNomeMacchina)
        End Try
    End Sub

    Public Overridable Sub LayExecScdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oLayCsvDto As LayCsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_LAY, oLayCsvDto.CodiceArticolo)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.LayNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.LayNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("Z", Serie, oLayCsvDto.DataOra.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "Z", oLayCsvDto.DataOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oLayCsvDto.DataOra.Year}, serie {Serie}, prodotto {oLayCsvDto.CodiceArticolo} QtaProdotte {oLayCsvDto.PalletCompleti}")
        End If

        OMovimentazioneManager.SetApriDocSilent(True)

        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di Scarico. Dettagli: numero documento {lNumTmpProd} data documento {oLayCsvDto.DataOra.Year}, serie {Serie}, prodotto {oLayCsvDto.CodiceArticolo} QtaProdotte {oLayCsvDto.PalletCompleti}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "Z", oLayCsvDto.DataOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oLayCsvDto.DataOra.Year}, serie {Serie}, prodotto {oLayCsvDto.CodiceArticolo} QtaProdotte {oLayCsvDto.PalletCompleti}")
        End If
        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "Z"
            r!et_anno = oLayCsvDto.DataOra.Year
            r!et_serie = Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oLayCsvDto.Note
            r!et_datdoc = oLayCsvDto.DataOra
            r!et_tipobf = settings.TipoBfScarico
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oLayCsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oLayCsvDto.PalletCompleti

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        OMovimentazioneManager.SettaPiedeProd()

        OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di Scarico.! Dettagli: numero documento {lNumTmpProd} data documento {oLayCsvDto.DataOra.Year}, serie {Serie}, prodotto {oLayCsvDto.CodiceArticolo} QtaProdotte {oLayCsvDto.PalletCompleti}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie
        logger.LogInfo($"Scarico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oLayCsvDto.DataOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oLayCsvDto.PalletCompleti} per il prodotto {oLayCsvDto.CodiceArticolo} ", settings.LayNomeMacchina, oLayCsvDto.CodiceArticolo, PublicCurrFileName)
    End Sub

#End Region

#Region "Etich"

    'Public Overridable Sub EtichScaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
    '    'si scorre i file csv

    '    Dim AllFileCsvPaths() As String = Nothing
    '    Try
    '        'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
    '        'perchè la cartella condivisa è protetta da nome utente e pwd
    '        If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
    '            Try
    '                NetworkShareManager.ConnectToShare(settings.EtichPercorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
    '                AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
    '            Catch ex As Exception
    '                'si è verificato un errore nell'accesso con password
    '                'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
    '                'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
    '                Try
    '                    AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
    '                Catch exInn As Exception
    '                    'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
    '                    Throw New Exception($"Apertura della cartella {settings.EtichPercorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
    '                End Try
    '            End Try
    '        Else
    '            'le cartelle non sono protette da password leggo direttamente i contenuti
    '            AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
    '        End If

    '        Dim ObjEtichCsvDto As EtichCsvDto = Nothing

    '        Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

    '        Dim PublicCurrFileName As String = ""

    '        For Each FilePath As String In CsvFilePaths


    '            Try
    '                'espongo il nome del file per loggarlo in caso d'errore
    '                PublicCurrFileName = Path.GetFileName(FilePath)
    '                'verifica la validità del csv
    '                ObjEtichCsvDto = EtichCsvParser.ParseProduzioneCsv(FilePath, settings)

    '                'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
    '                'l'articolo ha gestione lotti?
    '                Dim OLottoManager As New LottoManager(settings, oCleAnlo)
    '                'Dim OLottoRep = New LottoRep(oCleAnlo)
    '                Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjEtichCsvDto.CodiceArticolo)

    '                'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
    '                'preparo i dati da inserire nel lotto...
    '                Dim oLottoDto As LottoDto = Nothing

    '                Dim Oggi As Date = ObjEtichCsvDto.Data

    '                'Il lotto esiste già?
    '                If IsArtConfForLotto Then
    '                    Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjEtichCsvDto.CodiceArticolo, settings.EtichNomeMacchina, ObjEtichCsvDto.Data)
    '                    oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjEtichCsvDto.CodiceArticolo, NomeLotto)
    '                    'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
    '                    'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

    '                    If oLottoDto Is Nothing Then
    '                        Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
    '                        oLottoDto = New LottoDto() With {
    '                      .StrCodart = CLN__STD.NTSCStr(ObjEtichCsvDto.CodiceArticolo),
    '                      .StrDescodart = CLN__STD.NTSCStr(""),
    '                        .LLotto = CLN__STD.NTSCInt(NextLottoNumber),
    '                      .StrLottox = NomeLotto,
    '                      .DataCreazione = CLN__STD.NTSCDate(Oggi),
    '                      .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
    '                      .LottoGiaPresente = False
    '                  }
    '                    End If

    '                End If

    '                'crea il lotto e un carico di produzione per l'articolo del csv
    '                Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
    '                LayExecScdp(OMovimentazioneManager, OLottoManager, ObjEtichCsvDto, oLottoDto, settings, logger, PublicCurrFileName)
    '                'sposta il file in old dopo averlo elaborato
    '                'crea la directory se ancora non c'è
    '                Try
    '                    SpostaECopiaFile(FilePath, Oggi, settings.EtichPercorsoOld, OMovimentazioneManager)

    '                Catch ex As Exception
    '                    ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
    '                End Try


    '            Catch ex As Exception
    '                'logga sposta in errore il file csv e va avanti
    '                logger.LogError(ex.Message, ex.StackTrace, settings.EtichNomeMacchina, If(ObjEtichCsvDto IsNot Nothing, ObjEtichCsvDto.CodiceArticolo, ""), PublicCurrFileName)

    '                Try
    '                    SpostaNellaCartellaErrori(FilePath, settings.EtichPercorsoErrori)

    '                Catch
    '                    ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
    '                End Try
    '            End Try

    '        Next
    '    Catch ex As Exception
    '        'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
    '        logger.LogError(ex.Message, ex.StackTrace, settings.EtichNomeMacchina)
    '    End Try
    'End Sub

    'Public Overridable Sub EtichExecScdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oEtichCsvDto As EtichCsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

    '    Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_ETICH, oEtichCsvDto.CodiceArticolo)

    '    'esegue effettivamente il carico in experience
    '    If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
    '        'creo il lotto solo se si tratta di un articolo con gestione lotti
    '        'e solo se il lotto non è ancora presente a db
    '        If Serie = "CON" Then
    '            'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
    '            'l'insert del contatore conteggio giorni di produzione in crea lotto
    '            oLottoDto.IsLottoConter = True
    '        End If
    '        oLottoManager.CreaLotto(oLottoDto)

    '        logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.EtichNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
    '    Else
    '        If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
    '            logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.EtichNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
    '        End If
    '    End If


    '    '--- Legge il progressivo in TABNUMA
    '    Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("Z", Serie, oEtichCsvDto.Data.Year)

    '    'preparo l'ambiente

    '    Dim ds As New DataSet
    '    If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "Z", oEtichCsvDto.Data.Year, Serie, lNumTmpProd, ds) Then
    '        Throw New Exception($"Apertura del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotto {oEtichCsvDto.CodiceArticolo} QtaProdotte {oEtichCsvDto.PalletCompleti}")
    '    End If

    '    OMovimentazioneManager.SetApriDocSilent(True)

    '    If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
    '        Throw New Exception($"Errore nella numerazione del documento di Scarico. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotto {oEtichCsvDto.CodiceArticolo} QtaProdotte {oEtichCsvDto.PalletCompleti}")
    '    End If
    '    OMovimentazioneManager.ResetVar()
    '    OMovimentazioneManager.StrVisNoteConto = "N"

    '    If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "Z", oEtichCsvDto.Data.Year, Serie, lNumTmpProd, "") Then
    '        Throw New Exception($"Creazione del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotto {oEtichCsvDto.CodiceArticolo} QtaProdotte {oEtichCsvDto.PalletCompleti}")
    '    End If
    '    'oCleBoll.bInNuovoDocSilent = True
    '    OMovimentazioneManager.SetNuovoDocSilent(True)

    '    Dim OTestataCaricoDiProd As Action(Of DataRow) =
    '    Sub(r As DataRow)
    '        r!codditt = oApp.Ditta
    '        r!et_conto = settings.Fornitore
    '        r!et_tipork = "Z"
    '        r!et_anno = oEtichCsvDto.Data.Year
    '        r!et_serie = Serie
    '        r!et_numdoc = lNumTmpProd
    '        r!et_note = oEtichCsvDto.Note
    '        r!et_datdoc = oEtichCsvDto.Data
    '        r!et_tipobf = settings.TipoBfScarico
    '    End Sub

    '    OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


    '    'setta il carico di produzione
    '    Dim OCorpoCaricoProd As New CorpoCaricoProd()
    '    OCorpoCaricoProd.CodArt = oEtichCsvDto.CodiceArticolo
    '    OCorpoCaricoProd.ec_colli = oEtichCsvDto.PalletCompleti

    '    OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

    '    OMovimentazioneManager.SettaPiedeProd()

    '    OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

    '    If Not OMovimentazioneManager.SalvaDocumento("N") Then
    '        Throw New Exception($"Errore al salvataggio del documento di Scarico.! Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotto {oEtichCsvDto.CodiceArticolo} QtaProdotte {oEtichCsvDto.PalletCompleti}")
    '    End If


    '    OMovimentazioneManager.ProgProd = lNumTmpProd
    '    OMovimentazioneManager.Serie = Serie
    '    logger.LogInfo($"Scarico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oEtichCsvDto.Data.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oEtichCsvDto.PalletCompleti} per il prodotto {oEtichCsvDto.CodiceArticolo} ", settings.EtichNomeMacchina, oEtichCsvDto.CodiceArticolo, PublicCurrFileName)
    'End Sub

    '-------------------------------------------------------------------

    'qui di seguito la versione corretta per etich
    Public Overridable Sub EtichScaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.EtichPercorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.EtichPercorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.EtichPercorso, "*.csv")
            End If

            Dim ObjEtichCsvDto As EtichCsvDto = Nothing

            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjEtichCsvDto = EtichCsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArt1ConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjEtichCsvDto.NomeEtichettaGruppo1)
                    Dim IsArt2ConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjEtichCsvDto.NomeEtichettaGruppo2)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto1 As LottoDto = Nothing
                    Dim oLottoDto2 As LottoDto = Nothing

                    Dim Oggi As Date = ObjEtichCsvDto.Data

                    Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                    'Il lotto esiste già per il primo articolo?
                    If IsArt1ConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjEtichCsvDto.NomeEtichettaGruppo1, settings.EtichNomeMacchina, ObjEtichCsvDto.Data)
                        oLottoDto1 = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjEtichCsvDto.NomeEtichettaGruppo1, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto1 Is Nothing Then
                            oLottoDto1 = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjEtichCsvDto.NomeEtichettaGruppo1),
                          .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(NextLottoNumber),
                          .StrLottox = NomeLotto,
                          .DataCreazione = CLN__STD.NTSCDate(Oggi),
                          .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                          .LottoGiaPresente = False
                      }
                        End If

                    End If

                    'Il lotto esiste già per il secondo articolo?
                    If IsArt2ConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjEtichCsvDto.NomeEtichettaGruppo2, settings.EtichNomeMacchina, ObjEtichCsvDto.Data)
                        oLottoDto1 = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjEtichCsvDto.NomeEtichettaGruppo2, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto2 Is Nothing Then
                            'Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto2 = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjEtichCsvDto.NomeEtichettaGruppo2),
                          .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(NextLottoNumber + 1),
                          .StrLottox = NomeLotto,
                          .DataCreazione = CLN__STD.NTSCDate(Oggi),
                          .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                          .LottoGiaPresente = False
                      }
                        End If

                    End If

                    'crea il lotto e un carico di produzione per l'articolo del csv
                    Dim OMovimentazioneManager As New MovimentazioneManager(settings, oCleBoll)
                    'todo:creare due routine per gestire i carichi
                    EtichExecScdp(OMovimentazioneManager, OLottoManager, ObjEtichCsvDto, oLottoDto1, settings, logger, PublicCurrFileName)
                    'EtichExecScdp(OMovimentazioneManager, OLottoManager, ObjEtichCsvDto, oLottoDto2, settings, logger, PublicCurrFileName)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        SpostaECopiaFile(FilePath, Oggi, settings.EtichPercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.EtichNomeMacchina, If(ObjEtichCsvDto IsNot Nothing, ObjEtichCsvDto.NomeEtichettaGruppo1 + " " + ObjEtichCsvDto.NomeEtichettaGruppo2, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.EtichPercorsoErrori)

                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try
                End Try

            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.EtichNomeMacchina)
        End Try
    End Sub

    Public Overridable Sub EtichExecScdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oEtichCsvDto As EtichCsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_ETICH, oEtichCsvDto.NomeEtichettaGruppo1)

        'non deve creare nessun lotto etich...
        ''esegue effettivamente il carico in experience
        'If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
        '    'creo il lotto solo se si tratta di un articolo con gestione lotti
        '    'e solo se il lotto non è ancora presente a db
        '    If Serie1 = "CON" Then
        '        'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
        '        'l'insert del contatore conteggio giorni di produzione in crea lotto
        '        oLottoDto.IsLottoConter = True
        '    End If
        '    oLottoManager.CreaLotto(oLottoDto)

        '    logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.EtichNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        'Else
        '    If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
        '        logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.EtichNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        '    End If
        'End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("Z", Serie, oEtichCsvDto.Data.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "Z", oEtichCsvDto.Data.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotti {oEtichCsvDto.NomeEtichettaGruppo1},{oEtichCsvDto.NomeEtichettaGruppo2} QtaProdotte {oEtichCsvDto.EtichetteErogateGruppo1},{oEtichCsvDto.EtichetteErogateGruppo2}")
        End If

        OMovimentazioneManager.SetApriDocSilent(True)

        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di Scarico. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotti {oEtichCsvDto.NomeEtichettaGruppo1},{oEtichCsvDto.NomeEtichettaGruppo2} QtaProdotte {oEtichCsvDto.EtichetteErogateGruppo1},{oEtichCsvDto.EtichetteErogateGruppo2}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "Z", oEtichCsvDto.Data.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotti {oEtichCsvDto.NomeEtichettaGruppo1},{oEtichCsvDto.NomeEtichettaGruppo2} QtaProdotte {oEtichCsvDto.EtichetteErogateGruppo1},{oEtichCsvDto.EtichetteErogateGruppo2}")
        End If
        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "Z"
            r!et_anno = oEtichCsvDto.Data.Year
            r!et_serie = Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oEtichCsvDto.Note
            r!et_datdoc = oEtichCsvDto.Data
            r!et_tipobf = settings.TipoBfScarico
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd1 As New CorpoCaricoProd()
        OCorpoCaricoProd1.CodArt = oEtichCsvDto.NomeEtichettaGruppo1
        OCorpoCaricoProd1.ec_colli = oEtichCsvDto.EtichetteErogateGruppo1

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd1, oLottoDto)

        Dim OCorpoCaricoProd2 As New CorpoCaricoProd()
        OCorpoCaricoProd2.CodArt = oEtichCsvDto.NomeEtichettaGruppo2
        OCorpoCaricoProd2.ec_colli = oEtichCsvDto.EtichetteErogateGruppo2

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd2, oLottoDto)

        OMovimentazioneManager.SettaPiedeProd()

        OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di Scarico.! Dettagli: numero documento {lNumTmpProd} data documento {oEtichCsvDto.Data.Year}, serie {Serie}, prodotti {oEtichCsvDto.NomeEtichettaGruppo1},{oEtichCsvDto.NomeEtichettaGruppo2} QtaProdotte {oEtichCsvDto.EtichetteErogateGruppo1},{oEtichCsvDto.EtichetteErogateGruppo2}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie
        logger.LogInfo($"Scarico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oEtichCsvDto.Data.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di QtaProdotte {oEtichCsvDto.EtichetteErogateGruppo1},{oEtichCsvDto.EtichetteErogateGruppo2} per i prodotti {oEtichCsvDto.NomeEtichettaGruppo1},{oEtichCsvDto.NomeEtichettaGruppo2} ", settings.EtichNomeMacchina, oEtichCsvDto.NomeEtichettaGruppo1, PublicCurrFileName)
    End Sub

#End Region


#Region "Picker23017"

    Public Overridable Sub Picker23017ScaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.Picker23017Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.Picker23017Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.Picker23017Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.Picker23017Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.Picker23017Percorso, "*.csv")
            End If

            Dim ObjPicker23017CsvDto As Picker23017CsvDto = Nothing

            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjPicker23017CsvDto = Picker23017CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjPicker23017CsvDto.CodiceArticolo)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing

                    Dim Oggi As Date = ObjPicker23017CsvDto.DataEOra

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjPicker23017CsvDto.CodiceArticolo, settings.Picker23017NomeMacchina, ObjPicker23017CsvDto.DataEOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjPicker23017CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjPicker23017CsvDto.CodiceArticolo),
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
                    Picker23017ExecScdp(OMovimentazioneManager, OLottoManager, ObjPicker23017CsvDto, oLottoDto, settings, logger, PublicCurrFileName)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        SpostaECopiaFile(FilePath, Oggi, settings.Picker23017PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Picker23017NomeMacchina, If(ObjPicker23017CsvDto IsNot Nothing, ObjPicker23017CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.Picker23017PercorsoErrori)

                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try
                End Try

            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.Picker23017NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub Picker23017ExecScdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oPicker23017CsvDto As Picker23017CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PICKER23017, oPicker23017CsvDto.CodiceArticolo)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Picker23017NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Picker23017NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("Z", Serie, oPicker23017CsvDto.DataEOra.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "Z", oPicker23017CsvDto.DataEOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23017CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23017CsvDto.CodiceArticolo} QtaProdotte {oPicker23017CsvDto.PezziBuoni}")
        End If

        OMovimentazioneManager.SetApriDocSilent(True)

        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di Scarico. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23017CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23017CsvDto.CodiceArticolo} QtaProdotte {oPicker23017CsvDto.PezziBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "Z", oPicker23017CsvDto.DataEOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23017CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23017CsvDto.CodiceArticolo} QtaProdotte {oPicker23017CsvDto.PezziBuoni}")
        End If
        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "Z"
            r!et_anno = oPicker23017CsvDto.DataEOra.Year
            r!et_serie = Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oPicker23017CsvDto.Note
            r!et_datdoc = oPicker23017CsvDto.DataEOra
            r!et_tipobf = settings.TipoBfScarico
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oPicker23017CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oPicker23017CsvDto.PezziBuoni

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        OMovimentazioneManager.SettaPiedeProd()

        OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di Scarico.! Dettagli: numero documento {lNumTmpProd} data documento {oPicker23017CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23017CsvDto.CodiceArticolo} QtaProdotte {oPicker23017CsvDto.PezziBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie
        logger.LogInfo($"Scarico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oPicker23017CsvDto.DataEOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oPicker23017CsvDto.PezziBuoni} per il prodotto {oPicker23017CsvDto.CodiceArticolo} ", settings.Picker23017NomeMacchina, oPicker23017CsvDto.CodiceArticolo, PublicCurrFileName)
    End Sub

#End Region


#Region "Picker23018"

    Public Overridable Sub Picker23018ScaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.Picker23018Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.Picker23018Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.Picker23018Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.Picker23018Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.Picker23018Percorso, "*.csv")
            End If

            Dim ObjPicker23018CsvDto As Picker23018CsvDto = Nothing

            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjPicker23018CsvDto = Picker23018CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjPicker23018CsvDto.CodiceArticolo)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing

                    Dim Oggi As Date = ObjPicker23018CsvDto.DataEOra

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjPicker23018CsvDto.CodiceArticolo, settings.Picker23018NomeMacchina, ObjPicker23018CsvDto.DataEOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjPicker23018CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjPicker23018CsvDto.CodiceArticolo),
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
                    Picker23018ExecScdp(OMovimentazioneManager, OLottoManager, ObjPicker23018CsvDto, oLottoDto, settings, logger, PublicCurrFileName)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        SpostaECopiaFile(FilePath, Oggi, settings.Picker23018PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Picker23018NomeMacchina, If(ObjPicker23018CsvDto IsNot Nothing, ObjPicker23018CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.Picker23018PercorsoErrori)

                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try
                End Try

            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.Picker23018NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub Picker23018ExecScdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oPicker23018CsvDto As Picker23018CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PICKER23018, oPicker23018CsvDto.CodiceArticolo)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Picker23018NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Picker23018NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("Z", Serie, oPicker23018CsvDto.DataEOra.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "Z", oPicker23018CsvDto.DataEOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23018CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23018CsvDto.CodiceArticolo} QtaProdotte {oPicker23018CsvDto.PezziBuoni}")
        End If

        OMovimentazioneManager.SetApriDocSilent(True)

        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di Scarico. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23018CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23018CsvDto.CodiceArticolo} QtaProdotte {oPicker23018CsvDto.PezziBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "Z", oPicker23018CsvDto.DataEOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di Scarico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oPicker23018CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23018CsvDto.CodiceArticolo} QtaProdotte {oPicker23018CsvDto.PezziBuoni}")
        End If
        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "Z"
            r!et_anno = oPicker23018CsvDto.DataEOra.Year
            r!et_serie = Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oPicker23018CsvDto.Note
            r!et_datdoc = oPicker23018CsvDto.DataEOra
            r!et_tipobf = settings.TipoBfScarico
        End Sub

        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oPicker23018CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oPicker23018CsvDto.PezziBuoni

        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        OMovimentazioneManager.SettaPiedeProd()

        OMovimentazioneManager.BCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di Scarico.! Dettagli: numero documento {lNumTmpProd} data documento {oPicker23018CsvDto.DataEOra.Year}, serie {Serie}, prodotto {oPicker23018CsvDto.CodiceArticolo} QtaProdotte {oPicker23018CsvDto.PezziBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie
        logger.LogInfo($"Scarico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oPicker23018CsvDto.DataEOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oPicker23018CsvDto.PezziBuoni} per il prodotto {oPicker23018CsvDto.CodiceArticolo} ", settings.Picker23018NomeMacchina, oPicker23018CsvDto.CodiceArticolo, PublicCurrFileName)
    End Sub

#End Region

#Region "Duetti"

    Public Overridable Sub DuettiCaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.DuettiPercorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.DuettiPercorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.DuettiPercorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.DuettiPercorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.DuettiPercorso, "*.csv")
            End If

            Dim ObjDuettiCsvDto As DuettiCsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjDuettiCsvDto = DuettiCsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjDuettiCsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjDuettiCsvDto.DataOra

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjDuettiCsvDto.CodiceArticolo, settings.DuettiNomeMacchina, ObjDuettiCsvDto.DataOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjDuettiCsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjDuettiCsvDto.CodiceArticolo),
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
                    DuettiExecCdp(OMovimentazioneManager, OLottoManager, ObjDuettiCsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.DuettiPercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.DuettiNomeMacchina, If(ObjDuettiCsvDto IsNot Nothing, ObjDuettiCsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.DuettiNomeMacchina, If(ObjDuettiCsvDto IsNot Nothing, ObjDuettiCsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.DuettiPercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.DuettiNomeMacchina, If(ObjDuettiCsvDto IsNot Nothing, ObjDuettiCsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.DuettiNomeMacchina)
        End Try
    End Sub

    Public Overridable Sub DuettiExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oDuettiCsvDto As DuettiCsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_DUETTI, oDuettiCsvDto.CodiceArticolo)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.DuettiNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.DuettiNomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oDuettiCsvDto.DataOra.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oDuettiCsvDto.DataOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oDuettiCsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuettiCsvDto.CodiceArticolo} QtaProdotte {oDuettiCsvDto.CartoniBuoni}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oDuettiCsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuettiCsvDto.CodiceArticolo} QtaProdotte {oDuettiCsvDto.CartoniBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oDuettiCsvDto.DataOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oDuettiCsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuettiCsvDto.CodiceArticolo} QtaProdotte {oDuettiCsvDto.CartoniBuoni}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oDuettiCsvDto.DataOra.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oDuettiCsvDto.Note
           r!et_datdoc = oDuettiCsvDto.DataOra
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oDuettiCsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oDuettiCsvDto.CartoniBuoni

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oDuettiCsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuettiCsvDto.CodiceArticolo} QtaProdotte {oDuettiCsvDto.CartoniBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oDuettiCsvDto.DataOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oDuettiCsvDto.CartoniBuoni} per il prodotto {oDuettiCsvDto.CodiceArticolo} ", settings.DuettiNomeMacchina, oDuettiCsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region

#Region "Duetti2"

    Public Overridable Sub Duetti2CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogManager)
        'si scorre i file csv

        Dim AllFileCsvPaths() As String = Nothing
        Try
            'prima di accedere alla cartella si logga (solo se la sicurezza è configurata nel config.ini)
            'perchè la cartella condivisa è protetta da nome utente e pwd
            If settings.MachineFoldersSecurity.ToUpper() = GlobalConstants.MACHINE_FOLDERS_SECURITY_ON Then
                Try
                    NetworkShareManager.ConnectToShare(settings.Duetti2Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
                    AllFileCsvPaths = Directory.GetFiles(settings.Duetti2Percorso, "*.csv")
                Catch ex As Exception
                    'si è verificato un errore nell'accesso con password
                    'verifico che l'accesso non sia già stato fatto precedentemente perchè nel qual caso andrebbe in crash
                    'se cerca di accedere ad una cartella a cui si può già accedere fornendo le credenziali
                    Try
                        AllFileCsvPaths = Directory.GetFiles(settings.Duetti2Percorso, "*.csv")
                    Catch exInn As Exception
                        'se anche il tentativo di accedere senza credenziali fallisce allora il problema potrebbe essere utente o pwd errata
                        Throw New Exception($"Apertura della cartella {settings.Duetti2Percorso} fallita, utilizzando l'utente {settings.MachineFoldersUserName} probabilmente il nome utente o password non sono corretti")
                    End Try
                End Try
            Else
                'le cartelle non sono protette da password leggo direttamente i contenuti
                AllFileCsvPaths = Directory.GetFiles(settings.Duetti2Percorso, "*.csv")
            End If

            Dim ObjDuetti2CsvDto As Duetti2CsvDto = Nothing


            Dim CsvFilePaths = FiltraSoloCsvMacchine(AllFileCsvPaths)

            Dim PublicCurrFileName As String = ""

            For Each FilePath As String In CsvFilePaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(FilePath)
                    'verifica la validità del csv
                    ObjDuetti2CsvDto = Duetti2CsvParser.ParseProduzioneCsv(FilePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoManager As New LottoManager(settings, oCleAnlo)
                    'Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoManager.IsArtConfForLotto(oApp.Ditta, ObjDuetti2CsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjDuetti2CsvDto.DataOra

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjDuetti2CsvDto.CodiceArticolo, settings.Duetti2NomeMacchina, ObjDuetti2CsvDto.DataOra)
                        oLottoDto = OLottoManager.GetLottoProdottoFinito(oApp.Ditta, ObjDuetti2CsvDto.CodiceArticolo, NomeLotto)
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            Dim NextLottoNumber As Integer = OLottoManager.GetNextLottoNumber(oApp.Ditta)
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjDuetti2CsvDto.CodiceArticolo),
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
                    Duetti2ExecCdp(OMovimentazioneManager, OLottoManager, ObjDuetti2CsvDto, oLottoDto, settings, logger, PublicCurrFileName)

                    Try
                        'fa una copia del file con questa logica es:
                        '2025-09-11_23.09.28_ExpFile.csv_1026_PB_12_09_2025.csv dove 2025-09-11_23.09.28_ExpFile.csv= il nome del file che viene spostato in old
                        '1026= progressivo di produzione, PB= serie, 12_09_2025 la data del carico nel gestionale ERP
                        'sposta il file in old dopo averlo elaborato, crea la directory se ancora non c'è
                        SpostaECopiaFile(FilePath, Oggi, settings.Duetti2PercorsoOld, OMovimentazioneManager)

                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(ex.Message, ex.StackTrace, settings.Duetti2NomeMacchina, If(ObjDuetti2CsvDto IsNot Nothing, ObjDuetti2CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Duetti2NomeMacchina, If(ObjDuetti2CsvDto IsNot Nothing, ObjDuetti2CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        SpostaNellaCartellaErrori(FilePath, settings.Duetti2PercorsoErrori)
                    Catch exInn As Exception
                        ' Se fallisce lo spostamento in err qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                        logger.LogError(exInn.Message, exInn.StackTrace, settings.Duetti2NomeMacchina, If(ObjDuetti2CsvDto IsNot Nothing, ObjDuetti2CsvDto.CodiceArticolo, ""), PublicCurrFileName)
                    End Try

                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.Duetti2NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub Duetti2ExecCdp(OMovimentazioneManager As MovimentazioneManager, oLottoManager As LottoManager, oDuetti2CsvDto As Duetti2CsvDto, oLottoDto As LottoDto, settings As Settings, logger As LogManager, PublicCurrFileName As String)

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_DUETTI2, oDuetti2CsvDto.CodiceArticolo)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

                logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Duetti2NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.Duetti2NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oDuetti2CsvDto.DataOra.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not OMovimentazioneManager.ApriDoc(oApp.Ditta, False, "T", oDuetti2CsvDto.DataOra.Year, Serie, lNumTmpProd, ds) Then
            Throw New Exception($"Apertura del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oDuetti2CsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuetti2CsvDto.CodiceArticolo} QtaProdotte {oDuetti2CsvDto.CartoniBuoni}")
        End If
        'oCleBoll.bInApriDocSilent = True
        OMovimentazioneManager.SetApriDocSilent(True)
        'If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
        '    Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oIca1CsvDto.InizioTurno.Year}, serie {Serie}, prodotto {oIca1CsvDto.CodiceArticolo} QtaProdotte {oIca1CsvDto.ScatoleTeoricheProdotte}")
        'End If
        If OMovimentazioneManager.DsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception($"Errore nella numerazione del documento di carico. Dettagli: numero documento {lNumTmpProd} data documento {oDuetti2CsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuetti2CsvDto.CodiceArticolo} QtaProdotte {oDuetti2CsvDto.CartoniBuoni}")
        End If
        OMovimentazioneManager.ResetVar()
        OMovimentazioneManager.StrVisNoteConto = "N"

        If Not OMovimentazioneManager.NuovoDocumento(oApp.Ditta, "T", oDuetti2CsvDto.DataOra.Year, Serie, lNumTmpProd, "") Then
            Throw New Exception($"Creazione del documento di carico fallita. Dettagli: numero documento {lNumTmpProd} data documento {oDuetti2CsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuetti2CsvDto.CodiceArticolo} QtaProdotte {oDuetti2CsvDto.CartoniBuoni}")
        End If

        'oCleBoll.bInNuovoDocSilent = True
        OMovimentazioneManager.SetNuovoDocSilent(True)

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oDuetti2CsvDto.DataOra.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oDuetti2CsvDto.Note
           r!et_datdoc = oDuetti2CsvDto.DataOra
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oDuetti2CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oDuetti2CsvDto.CartoniBuoni

        'CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)
        OMovimentazioneManager.CreaRigaProd(OCorpoCaricoProd, oLottoDto)

        'SettaPiedeProd(oCleBoll)
        OMovimentazioneManager.SettaPiedeProd()

        'oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        OMovimentazioneManager.BCreaFilePick = False

        If Not OMovimentazioneManager.SalvaDocumento("N") Then
            Throw New Exception($"Errore al salvataggio del documento di carico.! Dettagli: numero documento {lNumTmpProd} data documento {oDuetti2CsvDto.DataOra.Year}, serie {Serie}, prodotto {oDuetti2CsvDto.CodiceArticolo} QtaProdotte {oDuetti2CsvDto.CartoniBuoni}")
        End If


        OMovimentazioneManager.ProgProd = lNumTmpProd
        OMovimentazioneManager.Serie = Serie

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oDuetti2CsvDto.DataOra.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oDuetti2CsvDto.CartoniBuoni} per il prodotto {oDuetti2CsvDto.CodiceArticolo} ", settings.Duetti2NomeMacchina, oDuetti2CsvDto.CodiceArticolo, PublicCurrFileName)

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
                    Dim Oggi As Date = ObjIca1CsvDto.InizioTurno

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjIca1CsvDto.CodiceArticolo, settings.ICAVL08615NomeMacchina, ObjIca1CsvDto.InizioTurno)
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
                        SpostaECopiaFile(FilePath, Oggi, settings.ICAVL08615PercorsoOld, OMovimentazioneManager)

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

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PRONTOWASH1, oIca1CsvDto.CodiceArticolo)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.ICAVL08615NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.ICAVL08615NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oIca1CsvDto.InizioTurno.Year)


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
           r!et_anno = oIca1CsvDto.InizioTurno.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oIca1CsvDto.Note
           r!et_datdoc = oIca1CsvDto.InizioTurno
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oIca1CsvDto.CodiceArticolo
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

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oIca1CsvDto.InizioTurno.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oIca1CsvDto.ScatoleTeoricheProdotte} per il prodotto {oIca1CsvDto.CodiceArticolo} ", settings.ICAVL08615NomeMacchina, oIca1CsvDto.CodiceArticolo, PublicCurrFileName)

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
                    Dim Oggi As Date = ObjIca2CsvDto.InizioTurno

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then
                        Dim NomeLotto As String = OLottoManager.GetNomeLotto(ObjIca2CsvDto.CodiceArticolo, settings.ICAVL08616NomeMacchina, ObjIca2CsvDto.InizioTurno)
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
                        SpostaECopiaFile(FilePath, Oggi, settings.ICAVL08616PercorsoOld, OMovimentazioneManager)

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

        Dim Serie As String = OMovimentazioneManager.GetSerie(GlobalConstants.MACHINENAME_PRONTOWASH1, oIca2CsvDto.CodiceArticolo)
        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            If Serie = "CON" Then
                'se la serie è CON allora il lotto è di tipo conter e quindi devo fare 
                'l'insert del contatore conteggio giorni di produzione in crea lotto
                oLottoDto.IsLottoConter = True
            End If
            oLottoManager.CreaLotto(oLottoDto)

            logger.LogInfo($"Lotto creato per l'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.ICAVL08616NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
        Else
            If oLottoDto IsNot Nothing AndAlso oLottoDto.LottoGiaPresente AndAlso oLottoDto.StrLottox <> GlobalConstants.LOTTO_NONAPPLICATO Then
                logger.LogInfo($"Lotto associato all'articolo {oLottoDto.StrCodart}, nome lotto: {oLottoDto.StrLottox}", settings.ICAVL08616NomeMacchina, oLottoDto.StrCodart, PublicCurrFileName)
            End If
        End If


        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = OMovimentazioneManager.LegNuma("T", Serie, oIca2CsvDto.InizioTurno.Year)


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
           r!et_anno = oIca2CsvDto.InizioTurno.Year
           r!et_serie = Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oIca2CsvDto.Note
           r!et_datdoc = oIca2CsvDto.InizioTurno
           r!et_tipobf = settings.TipoBfCaricoScarico
           r!et_hhdescampolibero2 = "1" 'serve per segnalare che il carico è stato inserito automaticamente dal componente di interconnessione
       End Sub

        'CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)
        OMovimentazioneManager.CreaTestataProd(OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.CodArt = oIca2CsvDto.CodiceArticolo
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

        logger.LogInfo($"Carico effettuato con successo (num. produzione {OMovimentazioneManager.ProgProd} serie {OMovimentazioneManager.Serie} il {oIca2CsvDto.InizioTurno.ToString("dddd dd/MM/yyyy", New CultureInfo("it-IT"))} di qta {oIca2CsvDto.ScatoleTeoricheProdotte} per il prodotto {oIca2CsvDto.CodiceArticolo} ", settings.ICAVL08616NomeMacchina, oIca2CsvDto.CodiceArticolo, PublicCurrFileName)

    End Sub

#End Region


#Region "Routines private"

    Public Sub SpostaECopiaFile(FilePath As String, Oggi As DateTime, PercorsoOld As String, OMovimentazioneManager As MovimentazioneManager)
        ' Verifica ed eventualmente crea la cartella "Old"
        If Not Directory.Exists(PercorsoOld) Then
            Directory.CreateDirectory(PercorsoOld)
        End If

        ' Percorso del file da spostare in "Old"
        Dim OldFilePath As String = Path.Combine(PercorsoOld, Path.GetFileName(FilePath))

        ' Data in formato richiesto
        Dim DataCaricoERP As String = Oggi.ToString("dd_MM_yyyy")

        'toglie il segno / dalla serie per evitare problemi nel nome del file in quanto
        '/ non è un carattere valido nei nomi dei file nella creazione di percorsi
        Dim SerieForPath As String = OMovimentazioneManager.Serie.Replace("/", "")
        ' Nome e percorso della copia
        Dim FileCopiaNome As String = $"{Path.GetFileName(FilePath)}_{OMovimentazioneManager.ProgProd}_{SerieForPath}_{DataCaricoERP}.csv"
        Dim FileCopiaPath As String = Path.Combine(PercorsoOld, FileCopiaNome)

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
