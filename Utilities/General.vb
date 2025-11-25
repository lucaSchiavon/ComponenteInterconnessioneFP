Imports System.Globalization
Imports System.Text.RegularExpressions
Imports System.Threading.Thread

Module General
    'Public Function GetLottoDateString(data As Date) As String

    '    Dim strData As String = "00" & data.ToString("ddMMyyyy")
    '    Return strData
    'End Function

    Public Sub SetDataFormatForCurrentCulture()
        'definisce il formato data per la cultura corrente
        Dim strDateFormat As String = CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern
        If strDateFormat.IndexOf("yyyy") = -1 Then
            strDateFormat = strDateFormat.Replace("yy", "yyyy")
            Dim oCulture As CultureInfo = New CultureInfo(CurrentThread.CurrentCulture.Name)
            oCulture.DateTimeFormat.ShortDatePattern = strDateFormat
            CurrentThread.CurrentCulture = oCulture
        End If
    End Sub

    Public Function FiltraSoloCsvMacchine(filePaths() As String) As String()
        'filtra solo i csv depositati dalle macchine di produzione
        'quelli che vengono generati dal programma vengono ignorati
        ' Regex: verifica che "csv" appaia una sola volta nel nome del file (escludendo il percorso)
        Return filePaths.Where(Function(path)
                                   Dim fileName = System.IO.Path.GetFileName(path)
                                   Dim csvCount = Regex.Matches(fileName.ToLower(), "csv").Count
                                   Return csvCount = 1
                               End Function).ToArray()
    End Function
End Module
