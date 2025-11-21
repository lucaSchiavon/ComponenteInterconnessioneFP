Imports System.Globalization
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
End Module
