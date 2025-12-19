Imports System.IO
Imports System.Text.RegularExpressions

Public Class LogTestoManager

    Public Shared Sub LoggaErrore(ex As Exception)
        Dim fErr As StreamWriter = Nothing
        Try
            Dim fiErr As FileInfo = Nothing
            'Dim strNomeFileLog As String = "COMPINT_" & Date.Now.ToString("yyyy_MM_dd") & ".log"
            Dim strNomeFileLog As String = "COMPINT.log"
            fiErr = New FileInfo(strNomeFileLog)

            ' Soglia di dimensione: 50 MB
            Dim sogliaDimensione As Long = 500000 * 100

            ' Se il file esiste ed è troppo grande, crea un nuovo file con suffisso progressivo
            If fiErr.Exists AndAlso fiErr.Length > sogliaDimensione Then
                Dim i As Integer = 1
                Dim strNuovoNomeFileLog As String = ""
                Do
                    'strNuovoNomeFileLog = "COMPINT_" & Date.Now.ToString("yyyy_MM_dd") & "_" & i.ToString() & ".log"
                    strNuovoNomeFileLog = "COMPINT(" & i.ToString() & ").log"
                    fiErr = New FileInfo(strNuovoNomeFileLog)
                    i += 1
                Loop While fiErr.Exists AndAlso fiErr.Length > sogliaDimensione
                strNomeFileLog = strNuovoNomeFileLog
            End If

            ' Scrittura nel file di log
            fErr = New StreamWriter(strNomeFileLog, True)
            fErr.WriteLine("Errore: " & ex.Message)
            fErr.WriteLine("StackTrace: " & ex.StackTrace)
            fErr.WriteLine("Data/Ora: " & Date.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            fErr.WriteLine("--------------------------------------------------")
            fErr.Close()

        Catch innerEx As Exception
            ' Non posso fare nulla qui...
        Finally
            If fErr IsNot Nothing Then
                fErr.Close()
            End If
        End Try
    End Sub


End Class
