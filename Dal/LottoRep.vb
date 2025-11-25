Imports NTSInformatica

Public Class LottoRep
    Implements ILottoRep

    Private _oCleAnlo As CLEMGANLO

    Public Sub New(oCleAnlo As CLEMGANLO)
        Me._oCleAnlo = oCleAnlo
    End Sub
    Public Function IsArtConfForLotto(codditt As String, ar_codart As String) As Boolean Implements ILottoRep.IsArtConfForLotto
        Dim StrSQL As String = ""
        StrSQL = " SELECT * from artico where codditt='" & CLN__STD.NTSCStr(codditt) & "' and ar_codart='" & CLN__STD.NTSCStr(ar_codart) & "' and ar_geslotti='S'"
        ' Chiedo i dati al database
        Dim DsOut As DataSet = Me._oCleAnlo.ocldBase.OpenRecordset(StrSQL, CLE__APP.DBTIPO.DBAZI, "ARTICO“)
        Return DsOut.Tables(0).Rows.Count > 0
    End Function

    Public Function GetNextLottoNumber(codditt As String) As Integer Implements ILottoRep.GetNextLottoNumber
        Dim ProgNumerazioneLotto As Integer = 1
        Dim StrSQL As String = ""
        StrSQL = " SELECT MAX(alo_lotto) + 1 as ProgNumerazioneLotto from analotti where codditt='" & CLN__STD.NTSCStr(codditt) & "'"
        ' Chiedo i dati al database
        Dim DsOut As DataSet = Me._oCleAnlo.ocldBase.OpenRecordset(StrSQL, CLE__APP.DBTIPO.DBAZI, "ANALOTTI“)

        If DsOut.Tables(0).Rows.Count > 0 Then
            Dim val = DsOut.Tables(0).Rows(0)("ProgNumerazioneLotto")
            If val IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(val.ToString()) Then
                ProgNumerazioneLotto = CLN__STD.NTSCInt(val)
            End If
            'ProgNumerazioneLotto = CLN__STD.NTSCInt(DsOut.Tables(0).Rows("ProgNumerazioneLotto"))
        End If

        Return ProgNumerazioneLotto
    End Function

    Public Function GetLottoProdottoFinito(codditt As String, ar_codart As String, alo_lottox As String) As LottoDto Implements ILottoRep.GetLottoProdottoFinito
        Dim OLottoDto As LottoDto = Nothing

        Dim StrSQL As String = ""
        StrSQL = " SELECT * from analotti where codditt='" & CLN__STD.NTSCStr(codditt) & "' and alo_codart='" & CLN__STD.NTSCStr(ar_codart) & "' and alo_lottox='" & CLN__STD.NTSCStr(alo_lottox) & "'"
        ' Chiedo i dati al database
        Dim DsOut As DataSet = Me._oCleAnlo.ocldBase.OpenRecordset(StrSQL, CLE__APP.DBTIPO.DBAZI, "ANALOTTI“)
        Dim TblLottoProdFinito As DataTable = DsOut.Tables(0)

        If TblLottoProdFinito.Rows.Count > 0 Then
            Dim RowLottoProdFinito As DataRow = TblLottoProdFinito.Rows(0)
            OLottoDto = New LottoDto() With {
                            .StrCodart = CLN__STD.NTSCStr(RowLottoProdFinito("alo_codart")),
                            .StrDescodart = CLN__STD.NTSCStr(""),
                            .LLotto = CLN__STD.NTSCInt(RowLottoProdFinito("alo_lotto")),
                            .StrLottox = CLN__STD.NTSCStr(RowLottoProdFinito("alo_lottox")),
                            .DataScadenza = CLN__STD.NTSCStr(RowLottoProdFinito("alo_dtscad")),
                            .LottoGiaPresente = True
                        }

        End If

        Return OLottoDto
    End Function


    Public Sub CreaLotto(objLottoDto As LottoDto) Implements ILottoRep.CreaLotto

        Dim bOk As Boolean
        Dim strNomeTabella As String = "ANALOTTI"

        _oCleAnlo.strCodart = CLN__STD.NTSCStr(objLottoDto.StrCodart)
        'oCleAnlo.strDescodart = 'AUTO FINITA MODELLO 'b' (NEUTRO )'
        _oCleAnlo.lLotto = CLN__STD.NTSCInt(objLottoDto.LLotto) '1112025
        _oCleAnlo.strLottox = CLN__STD.NTSCStr(objLottoDto.StrLottox)

        Dim dsAnlo As DataSet = Nothing
        bOk = _oCleAnlo.Nuovo(_oCleAnlo.strCodart, _oCleAnlo.lLotto, dsAnlo)

        If bOk = False Then
            Throw New Exception("Errore nella creazione di un nuovo lotto")
        End If

        _oCleAnlo.dsShared = dsAnlo

        _oCleAnlo.dsShared.Tables("ANALOTTI").Rows.Add(_oCleAnlo.dsShared.Tables("ANALOTTI").NewRow)
        With _oCleAnlo.dsShared.Tables("ANALOTTI").Rows(_oCleAnlo.dsShared.Tables("ANALOTTI").Rows.Count - 1)
            !alo_lotto = _oCleAnlo.lLotto
            !alo_lottox = _oCleAnlo.strLottox
            !alo_dtscad = objLottoDto.DataScadenza
            !alo_dtprep = objLottoDto.DataCreazione
        End With

        bOk = _oCleAnlo.ocldBase.ScriviTabellaSemplice(_oCleAnlo.strDittaCorrente, strNomeTabella, _oCleAnlo.dsShared.Tables("ANALOTTI"), "", "", "")

        If bOk Then
            _oCleAnlo.dsShared.Tables("ANALOTTI").AcceptChanges()
            _oCleAnlo.bHasChanges = False
        Else
            Throw New Exception("Errore durante il salvataggio di un nuovo lotto")
        End If


    End Sub
End Class
