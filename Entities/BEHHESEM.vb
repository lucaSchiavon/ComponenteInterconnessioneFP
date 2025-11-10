Imports NTSInformatica.CLN__STD
Imports NTSInformatica

Public Class CLEHHESEM
    Inherits CLE__BASE
    Public Overridable Function CreaLotto(objLottoDto As LottoDto) As Boolean
        Try


            'Throw New Exception("test eccezione")
            '----------------------------------------------------------------------------------------
            '--- Inizializzo BEVEBOLL
            '----------------------------------------------------------------------------------------
            Dim bResult As Boolean
            Dim strNomeTabella As String = "ANALOTTI"
            If Not InizializzaBemganlo() Then Return False

            'oCleAnlo.strCodart = NTSCStr("CP2PF1")
            ''oCleAnlo.strDescodart = 'AUTO FINITA MODELLO 'b' (NEUTRO )'
            'oCleAnlo.lLotto = NTSCInt("002112025") '1112025
            'oCleAnlo.strLottox = NTSCStr("002112025")

            oCleAnlo.strCodart = NTSCStr(objLottoDto.StrCodart)
            'oCleAnlo.strDescodart = 'AUTO FINITA MODELLO 'b' (NEUTRO )'
            oCleAnlo.lLotto = NTSCInt(objLottoDto.LLotto) '1112025
            oCleAnlo.strLottox = NTSCStr(objLottoDto.StrLottox)

            Dim dsAnlo As DataSet
            oCleAnlo.Nuovo(oCleAnlo.strCodart, oCleAnlo.lLotto, dsAnlo)

            dsShared = dsAnlo

            'Dim dttLotto As System.Data.DataTable = dsAnlo.Tables("ANALOTTI")
            'Dim dtrNewRow As System.Data.DataRow = dttLotto.NewRow()
            dsShared.Tables("ANALOTTI").Rows.Add(dsShared.Tables("ANALOTTI").NewRow)
            With dsShared.Tables("ANALOTTI").Rows(dsShared.Tables("ANALOTTI").Rows.Count - 1)
                !alo_lotto = oCleAnlo.lLotto
                !alo_lottox = oCleAnlo.strLottox
                !alo_dtscad = objLottoDto.DataScadenza ' Today.AddDays(30)
            End With

            bResult = ocldBase.ScriviTabellaSemplice(strDittaCorrente, strNomeTabella, dsShared.Tables("ANALOTTI"), "", "", "")

            If bResult Then
                dsShared.Tables("ANALOTTI").AcceptChanges()
                bHasChanges = False
            End If

            Return True
        Catch ex As Exception
            CLN__STD.GestErr(ex, Me, "")
        End Try
    End Function
    Public oCleAnlo As CLEMGANLO = Nothing
    Public Overridable Function InizializzaBemganlo() As Boolean
        Try
            If Not oCleAnlo Is Nothing Then Return True
            '------------------------
            'inizializzo BEVEBOLL
            Dim strErr As String = ""
            Dim oTmp As Object = Nothing
            If CLN__STD.NTSIstanziaDll(oApp.ServerDir, oApp.NetDir, "BNMGANLO", "BEMGANLO", oTmp, strErr, False, "", "") = False Then
                'Throw New NTSException(oApp.Tr(Me, 128607611686875006, "ERRORE in fase di creazione Entity:" & vbCrLf & "|" & strErr & "|"))
                Return False
            End If
            oCleAnlo = CType(oTmp, CLEMGANLO)
            '------------------------------------------------
            'AddHandler oCleMganlo.RemoteEvent, AddressOf GestisciEventiEntityMYBOLL2
            If oCleAnlo.Init(oApp, oScript, CType(oApp.oMenu, CLE__MENU).oCleComm, "", False, "", "") = False Then Return False
            'If Not oCleMganlo.InitExt() Then Return False
            'oCleBoll2.bModuloCRM = False
            'oCleBoll2.bIsCRMUser = False
            Return True
        Catch ex As Exception
            CLN__STD.GestErr(ex, Me, "")
        End Try
    End Function
End Class
