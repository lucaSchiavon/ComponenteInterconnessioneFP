Imports NTSInformatica
Imports System.Threading.Thread
Imports System.Globalization
Imports System.IO

Public Class CompInterconnManager

    Public oApp As CLE__APP = Nothing
    Public oMenu As CLE__MENU = Nothing
    Public oCleBoll As CLEVEBOLL = Nothing
    Public oCleAnlo As CLEMGANLO = Nothing
    Dim logger As LogRep = Nothing
    'Public oCleEsem As CLEHHESEM

    ''autenticazione offline
    'Dim strBusDittaCorrente As String = "PROVA"
    'Dim strBusOperatore As String = "experience@toninnarciso.it"
    'Dim strBusPasswordOperatore As String = "Revihcra2!"
    'Dim strBusDatabase As String = "PROVA"
    'Dim strBusProfilo As String = "Fpbusexpcu5"
    ''Dim strBusProfilo As String = "dsdfsdfs"


    ''autenticazione trusted
    'Dim strBusDittaCorrente As String = "FPSRL"
    ''Dim strBusDittaCorrente As String = "PROVA"
    'Dim strBusOperatore As String = "UTENTE"
    'Dim strBusPasswordOperatore As String = ""
    ''Dim strBusDatabase As String = "PROVA"
    'Dim strBusDatabase As String = "FPSRL"
    'Dim strBusProfilo As String = "Fpbusexpcu5"
    ''Dim strBusProfilo As String = "dsdfsdfs"


    'Public lNumTmpProd As Integer
    'Public strTipoProd As String = "T"
    'Public strSerieProd As String = "A"
    'Public nAnnoProd As Integer = Now.Year
    Public Sub Main()



        Try
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++
            ' Dim fileCsvPaths() As String = Directory.GetFiles("\\192.168.101.111\prova")

            'Dim share As String = "\\192.168.101.111\prova"
            'Dim user As String = "Olivetti"
            'Dim pwd As String = "Oliv1999"

            'Dim ret As Integer = NetworkShareManager.ConnectToShare(share, user, pwd)
            'If ret = 0 Then
            '    Try
            '        Dim files() As String = Directory.GetFiles(share)
            '        For Each f In files
            '            Console.WriteLine(f)
            '        Next
            '    Catch ex As Exception
            '        Console.WriteLine("Errore file: " & ex.Message)
            '    Finally
            '        NetworkShareManager.DisconnectShare(share)
            '    End Try
            'Else
            '    Console.WriteLine("Connessione fallita, codice errore: " & ret)
            'End If
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'legge tutti i settings dal config ini

            Dim manager As New SettingsManager(Path.Combine(Environment.CurrentDirectory, "") & "\FPCOMPINTERCONN_Settings.ini")
            Dim settings As Settings = manager.LoadSettings()

            'istanzia la repository per l'inserimento dei log
            logger = New LogRep(settings.ConnStr)

            Dim MsgInizio As String = "JOB INIZIATO alle " & Date.Now
            logger.LogInfo(MsgInizio)
            Console.WriteLine(MsgInizio)
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

#Region "Routines del job di carico produzione"

    Public Overridable Sub ExecuteJob(settings As Settings, logger As LogRep)

        'inizializzo le entities di experience che mi serviranno per inserire lotti
        'e carichi di produzione
        InizializzaBeveboll()
        InizializzaBemganlo()

        'processa i file csv della cartella della macchina di produzione Ica2:
        Meccanoplastica1CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione Meccanoplastica1:
        ICAVL08615CaricoDiProduzione(oCleBoll, oCleAnlo, settings, logger)

        'processa i file csv della cartella della macchina di produzione xxx:
        '.......

    End Sub
    Public Overridable Sub CreaTestataProd(
    lNumTmpProd As Integer,
    oCleBoll As CLEVEBOLL,
    settings As Settings,
    setupRow As Action(Of DataRow)   ' <--- delegato passato dall’esterno
)

        Dim row As DataRow = oCleBoll.dttET.Rows(0)

        ' eseguo il codice esterno
        setupRow(row)

        ' validazione interna
        If Not oCleBoll.OkTestata Then
            Throw New Exception("Errore nella creazione della testata del documento")
        End If

    End Sub
    Public Overridable Sub CreaRigaProd(oCleBoll As CLEVEBOLL, oCorpoCaricoProd As CorpoCaricoProd, settings As Settings, oLottoDto As LottoDto)

        'Creo una nuova riga di corpo setto i principali campi poi setto tutti gli altri
        'todo:ma qui la casuale ed il magazzino va messo??  

        If Not oCleBoll.AggiungiRigaCorpo(False, CLN__STD.NTSCStr(oCorpoCaricoProd.Codditt), 0, 0) Then
            Throw New Exception("Errore nella creazione del corpo del documento")
        End If

        With oCleBoll.dttEC.Rows(oCleBoll.dttEC.Rows.Count - 1)
            !ec_colli = oCorpoCaricoProd.ec_colli
            If Not oLottoDto Is Nothing Then
                !ec_lotto = oLottoDto.LLotto
            End If
        End With

        If Not oCleBoll.RecordSalva(oCleBoll.dttEC.Rows.Count - 1, False, Nothing) Then
            oCleBoll.dttEC.Rows(oCleBoll.dttEC.Rows.Count - 1).Delete()
            Throw New Exception("Errore nel salvataggio del corpo del documento")
        End If

        oCleBoll.dtrHT = Nothing

    End Sub
    Public Sub SettaPiedeProd(oCleBoll As CLEVEBOLL)
        If oCleBoll.CalcolaTotali() = False Then
            Throw New Exception("Errore nel calcolo dei totali del piede corpo")
        End If
    End Sub
    Public Overridable Sub CreaLotto(oCleAnlo As CLEMGANLO, objLottoDto As LottoDto)

        Dim bOk As Boolean
        Dim strNomeTabella As String = "ANALOTTI"

        oCleAnlo.strCodart = CLN__STD.NTSCStr(objLottoDto.StrCodart)
        'oCleAnlo.strDescodart = 'AUTO FINITA MODELLO 'b' (NEUTRO )'
        oCleAnlo.lLotto = CLN__STD.NTSCInt(objLottoDto.LLotto) '1112025
        oCleAnlo.strLottox = CLN__STD.NTSCStr(objLottoDto.StrLottox)

        Dim dsAnlo As DataSet = Nothing
        bOk = oCleAnlo.Nuovo(oCleAnlo.strCodart, oCleAnlo.lLotto, dsAnlo)

        If bOk = False Then
            Throw New Exception("Errore nella creazione di un nuovo lotto")
        End If

        oCleAnlo.dsShared = dsAnlo

        oCleAnlo.dsShared.Tables("ANALOTTI").Rows.Add(oCleAnlo.dsShared.Tables("ANALOTTI").NewRow)
        With oCleAnlo.dsShared.Tables("ANALOTTI").Rows(oCleAnlo.dsShared.Tables("ANALOTTI").Rows.Count - 1)
            !alo_lotto = oCleAnlo.lLotto
            !alo_lottox = oCleAnlo.strLottox
            !alo_dtscad = objLottoDto.DataScadenza
        End With

        bOk = oCleAnlo.ocldBase.ScriviTabellaSemplice(oCleAnlo.strDittaCorrente, strNomeTabella, oCleAnlo.dsShared.Tables("ANALOTTI"), "", "", "")

        If bOk Then
            oCleAnlo.dsShared.Tables("ANALOTTI").AcceptChanges()
            oCleAnlo.bHasChanges = False
        Else
            Throw New Exception("Errore durante il salvataggio di un nuovo lotto")
        End If


    End Sub
#End Region

#Region "Meccanoplastica1"

    Public Overridable Sub Meccanoplastica1CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogRep)
        'si scorre i file csv
        Try
            'prima di accedere alla cartella si logga perchè la cartella condivisa è protetta da nome utente e pwd
            NetworkShareManager.ConnectToShare(settings.ICAVL08615Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
            Dim ObjMeccanoplastica1CsvDto As Meccanoplastica1CsvDto = Nothing
            Dim fileCsvPaths() As String = Directory.GetFiles(settings.Meccanoplastica1Percorso, "*.csv")
            Dim PublicCurrFileName As String

            For Each filePath As String In fileCsvPaths


                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(filePath)
                    'verifica la validità del csv
                    ObjMeccanoplastica1CsvDto = Meccanoplastica1CsvParser.ParseProduzioneCsv(filePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoRep.IsArtConfForLotto(oApp.Ditta, ObjMeccanoplastica1CsvDto.CodiceArticolo)

                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjMeccanoplastica1CsvDto.DataOra
                    Dim strOggi As String = GetLottoDateString(Oggi)

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        oLottoDto = OLottoRep.GetLottoProdottoFinito(oApp.Ditta, ObjMeccanoplastica1CsvDto.CodiceArticolo, strOggi, CLN__STD.NTSCInt(strOggi))
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            oLottoDto = New LottoDto() With {
                          .StrCodart = CLN__STD.NTSCStr(ObjMeccanoplastica1CsvDto.CodiceArticolo),
                          .StrDescodart = CLN__STD.NTSCStr(""),
                          .LLotto = CLN__STD.NTSCInt(strOggi),
                          .StrLottox = strOggi,
                          .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                          .LottoGiaPresente = False
                      }
                        End If

                    End If

                    'crea il lotto e un carico di produzione per l'articolo del csv
                    Meccanoplastica1ExecCdp(oCleBoll, oCleAnlo, ObjMeccanoplastica1CsvDto, oLottoDto, settings)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        If Not Directory.Exists(settings.Meccanoplastica1PercorsoOld) Then
                            Directory.CreateDirectory(settings.Meccanoplastica1PercorsoOld)
                        End If

                        Dim oldFile As String = Path.Combine(settings.Meccanoplastica1PercorsoOld, Path.GetFileName(filePath))
                        File.Move(filePath, oldFile)
                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try


                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.Meccanoplastica1NomeMacchina, If(ObjMeccanoplastica1CsvDto IsNot Nothing, ObjMeccanoplastica1CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        'crea la directory se ancora non c'è
                        If Not Directory.Exists(settings.Meccanoplastica1PercorsoErrori) Then
                            Directory.CreateDirectory(settings.Meccanoplastica1PercorsoErrori)
                        End If

                        Dim errorFile As String = Path.Combine(settings.Meccanoplastica1PercorsoErrori, Path.GetFileName(filePath))
                        File.Move(filePath, errorFile)
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

    Public Overridable Sub Meccanoplastica1ExecCdp(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, oMeccanoplastica1CsvDto As Meccanoplastica1CsvDto, oLottoDto As LottoDto, settings As Settings)

        'esegue effettivamente il carico in experience
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente Then
            'creo il lotto
            CreaLotto(oCleAnlo, oLottoDto)
        End If

        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = oCleBoll.LegNuma("T", settings.Meccanoplastica1Serie, oMeccanoplastica1CsvDto.DataOra.Year)

        'preparo l'ambiente

        Dim ds As New DataSet
        'todo:aggiungere serie
        Dim A As String = settings.Meccanoplastica1Serie
        If Not oCleBoll.ApriDoc(oApp.Ditta, False, "T", oMeccanoplastica1CsvDto.DataOra.Year, settings.Meccanoplastica1Serie, lNumTmpProd, ds) Then
            Throw New Exception("Apertura del documento di carico fallita")
        End If
        oCleBoll.bInApriDocSilent = True
        If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception("Errore nella numerazione del documento di carico.")
        End If
        oCleBoll.ResetVar()
        oCleBoll.strVisNoteConto = "N"

        If Not oCleBoll.NuovoDocumento(oApp.Ditta, "T", oMeccanoplastica1CsvDto.DataOra.Year, settings.Meccanoplastica1Serie, lNumTmpProd, "") Then
            Throw New Exception("Creazione del documento di carico fallita")
        End If
        oCleBoll.bInNuovoDocSilent = True


        Dim OTestataCaricoDiProd As Action(Of DataRow) =
        Sub(r As DataRow)
            r!codditt = oApp.Ditta
            r!et_conto = settings.Fornitore
            r!et_tipork = "T"
            r!et_anno = oMeccanoplastica1CsvDto.DataOra.Year
            r!et_serie = settings.Meccanoplastica1Serie
            r!et_numdoc = lNumTmpProd
            r!et_note = oMeccanoplastica1CsvDto.Note
            r!et_datdoc = oMeccanoplastica1CsvDto.DataOra
            r!et_tipobf = settings.tipobf
        End Sub

        CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)


        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.Codditt = oMeccanoplastica1CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oMeccanoplastica1CsvDto.PezziBuoni

        CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)

        SettaPiedeProd(oCleBoll)

        oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        If Not oCleBoll.SalvaDocumento("N") Then
            Throw New Exception("Errore al salvataggio del documento di carico.!")
        End If

        'todo:qui logga una info con che dice che il documento è stato creato con successo

    End Sub

#End Region

#Region "ICA1"

    Public Overridable Sub ICAVL08615CaricoDiProduzione(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, settings As Settings, logger As LogRep)
        'si scorre i file csv
        Try
            'prima di accedere alla cartella si logga perchè la cartella condivisa è protetta da nome utente e pwd
            NetworkShareManager.ConnectToShare(settings.ICAVL08615Percorso, settings.MachineFoldersUserName, settings.MachineFoldersPassword)
            Dim ObjIca1CsvDto As ICAVL08615CsvDto = Nothing
            Dim fileCsvPaths() As String = Directory.GetFiles(settings.ICAVL08615Percorso, "*.csv")
            Dim PublicCurrFileName As String

            For Each filePath As String In fileCsvPaths

                Try
                    'espongo il nome del file per loggarlo in caso d'errore
                    PublicCurrFileName = Path.GetFileName(filePath)
                    'verifica la validità del csv
                    ObjIca1CsvDto = ICAVL08615CsvParser.ParseProduzioneCsv(filePath, settings)

                    'se l'articolo è configurato per avere lotti crea il lotto altrimenti passa nothing
                    'l'articolo ha gestione lotti?
                    Dim OLottoRep = New LottoRep(oCleAnlo)
                    Dim IsArtConfForLotto As Boolean = OLottoRep.IsArtConfForLotto(oApp.Ditta, ObjIca1CsvDto.CodiceArticolo)



                    'se l'articolo è configurato per gestire lotto allora occorre creare un lotto per il prodotto finito
                    'preparo i dati da inserire nel lotto...
                    Dim oLottoDto As LottoDto = Nothing
                    Dim Oggi As Date = ObjIca1CsvDto.FineTurno
                    Dim strOggi As String = GetLottoDateString(Oggi)

                    'Il lotto esiste già?
                    If IsArtConfForLotto Then

                        oLottoDto = OLottoRep.GetLottoProdottoFinito(oApp.Ditta, ObjIca1CsvDto.CodiceArticolo, strOggi, CLN__STD.NTSCInt(strOggi))
                        'solo se l'articolo è configurato per le gestione lotti e non esiste già un lotto prodotto finito con lo stesso nome
                        'valorizza un oggetto LottoDto che poi servirà per inserire un nuovo lotto

                        If oLottoDto Is Nothing Then
                            oLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(ObjIca1CsvDto.CodiceArticolo),
                            .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(strOggi),
                            .StrLottox = strOggi,
                            .DataScadenza = CLN__STD.NTSCDate(Oggi.AddYears(3)),
                            .LottoGiaPresente = False
                        }

                        End If


                    End If

                    ICAVL08615ExecCdp(oCleBoll, oCleAnlo, ObjIca1CsvDto, oLottoDto, settings)
                    'sposta il file in old dopo averlo elaborato
                    'crea la directory se ancora non c'è
                    Try
                        If Not Directory.Exists(settings.ICAVL08615PercorsoOld) Then
                            Directory.CreateDirectory(settings.ICAVL08615PercorsoOld)
                        End If

                        Dim oldFile As String = Path.Combine(settings.ICAVL08615PercorsoOld, Path.GetFileName(filePath))
                        File.Move(filePath, oldFile)
                    Catch ex As Exception
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try



                Catch ex As Exception
                    'logga sposta in errore il file csv e va avanti
                    logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08615NomeMacchina, If(ObjIca1CsvDto IsNot Nothing, ObjIca1CsvDto.CodiceArticolo, ""), PublicCurrFileName)

                    Try
                        'crea la directory se ancora non c'è
                        If Not Directory.Exists(settings.ICAVL08615PercorsoErrori) Then
                            Directory.CreateDirectory(settings.ICAVL08615PercorsoErrori)
                        End If

                        Dim errorFile As String = Path.Combine(settings.ICAVL08615PercorsoErrori, Path.GetFileName(filePath))
                        File.Move(filePath, errorFile)
                    Catch
                        ' Se fallisce qui occorrerebbe fare il rollback dell'inserimento movimento di carico e del lotto del prodotto finito (se è stato inserito)
                    End Try

                    'LogManager.ScriviLog($"Errore durante l'elaborazione di {file} meccanoplastica: {ex.Message}{Environment.NewLine}{ex.StackTrace}")
                End Try
            Next
        Catch ex As Exception
            'logga in caso non riesca ad aprire il path della cartella dei file csv...e poi passa alla elaborazione dei files della prossima macchina
            logger.LogError(ex.Message, ex.StackTrace, settings.ICAVL08615NomeMacchina)
        End Try
    End Sub

    Public Overridable Sub ICAVL08615ExecCdp(oCleBoll As CLEVEBOLL, oCleAnlo As CLEMGANLO, oIca1CsvDto As ICAVL08615CsvDto, oLottoDto As LottoDto, settings As Settings)

        'esegue effettivamente il carico in experience
        'If Not oLottoDto.LottoGiaPresente Then
        If oLottoDto IsNot Nothing AndAlso Not oLottoDto.LottoGiaPresente Then
            'creo il lotto solo se si tratta di un articolo con gestione lotti
            'e solo se il lotto non è ancora presente a db
            CreaLotto(oCleAnlo, oLottoDto)
        End If

        '--- Legge il progressivo in TABNUMA
        Dim lNumTmpProd As Integer = oCleBoll.LegNuma("T", settings.ICAVL08615Serie, oIca1CsvDto.FineTurno.Year)


        'preparo l'ambiente

        Dim ds As New DataSet
        If Not oCleBoll.ApriDoc(oApp.Ditta, False, "T", oIca1CsvDto.InizioTurno.Year, settings.ICAVL08615Serie, lNumTmpProd, ds) Then
            Throw New Exception("Apertura del documento di carico fallita")
        End If
        oCleBoll.bInApriDocSilent = True
        If oCleBoll.dsShared.Tables("TESTA").Rows.Count > 0 Then
            Throw New Exception("Errore nella numerazione del documento di carico.")
        End If
        oCleBoll.ResetVar()
        oCleBoll.strVisNoteConto = "N"

        If Not oCleBoll.NuovoDocumento(oApp.Ditta, "T", oIca1CsvDto.InizioTurno.Year, settings.ICAVL08615Serie, lNumTmpProd, "") Then
            Throw New Exception("Creazione del documento di carico fallita")
        End If
        oCleBoll.bInNuovoDocSilent = True

        Dim OTestataCaricoDiProd As Action(Of DataRow) =
       Sub(r As DataRow)
           r!codditt = oApp.Ditta
           r!et_conto = settings.Fornitore
           r!et_tipork = "T"
           r!et_anno = oIca1CsvDto.InizioTurno.Year
           r!et_serie = settings.ICAVL08615Serie
           r!et_numdoc = lNumTmpProd
           r!et_note = oIca1CsvDto.Note
           r!et_datdoc = oIca1CsvDto.FineTurno
           r!et_tipobf = settings.tipobf
       End Sub

        CreaTestataProd(lNumTmpProd, oCleBoll, settings, OTestataCaricoDiProd)

        'setta il carico di produzione
        Dim OCorpoCaricoProd As New CorpoCaricoProd()
        OCorpoCaricoProd.Codditt = oIca1CsvDto.CodiceArticolo
        OCorpoCaricoProd.ec_colli = oIca1CsvDto.ScatoleTeoricheProdotte

        CreaRigaProd(oCleBoll, OCorpoCaricoProd, settings, oLottoDto)

        SettaPiedeProd(oCleBoll)

        oCleBoll.bCreaFilePick = False 'non faccio generare il piking dal salvataggio del documento
        If Not oCleBoll.SalvaDocumento("N") Then
            Throw New Exception("Errore al salvataggio del documento di carico.!")
        End If

        'todo:qui logga una info con che dice che il documento è stato creato con successo

    End Sub

#End Region






End Class
