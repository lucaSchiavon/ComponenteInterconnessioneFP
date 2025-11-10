Imports NTSInformatica

Public Class LottoRep
    Private _oCleAnlo As CLEMGANLO

    Public Sub New(oCleAnlo As CLEMGANLO)
        Me._oCleAnlo = oCleAnlo
    End Sub
    Public Function IsArtConfForLotto(codditt As String, ar_codart As String) As Boolean
        Dim StrSQL As String = ""
        StrSQL = " SELECT * from artico where codditt='" & CLN__STD.NTSCStr(codditt) & "' and ar_codart='" & ar_codart & " ' and ar_geslotti='S'" ' La query SQL
        ' Chiedo i dati al database
        Dim DsOut As DataSet = Me._oCleAnlo.ocldBase.OpenRecordset(StrSQL, CLE__APP.DBTIPO.DBAZI, "ARTICO“)
        Return DsOut.Tables(0).Rows.Count > 0
    End Function
End Class
