Imports NTSInformatica

Public Class LottoRep
    Private _oCleAnlo As CLEMGANLO

    Public Sub New(oCleAnlo As CLEMGANLO)
        Me._oCleAnlo = oCleAnlo
    End Sub
    Public Function IsArtConfForLotto(codditt As String, ar_codart As String) As Boolean
        Dim StrSQL As String = ""
        StrSQL = " SELECT * from artico where codditt='" & CLN__STD.NTSCStr(codditt) & "' and ar_codart='" & CLN__STD.NTSCStr(ar_codart) & "' and ar_geslotti='S'"
        ' Chiedo i dati al database
        Dim DsOut As DataSet = Me._oCleAnlo.ocldBase.OpenRecordset(StrSQL, CLE__APP.DBTIPO.DBAZI, "ARTICO“)
        Return DsOut.Tables(0).Rows.Count > 0
    End Function

    Public Function GetNextLottoNumber(codditt As String) As Integer
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

    Public Function GetLottoProdottoFinito(codditt As String, ar_codart As String, alo_lottox As String) As LottoDto

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
End Class
