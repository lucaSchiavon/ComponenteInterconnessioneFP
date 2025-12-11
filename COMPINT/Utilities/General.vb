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


    'Public Function ConvertiDataCustomForMeccanoplastica(dataStr As String) As Date
    '    Const fmt As String = "yyyy-MM-dd_HH.mm.ss"

    '    Return DateTime.ParseExact(
    '    dataStr,
    '    fmt,
    '    Globalization.CultureInfo.InvariantCulture
    ')
    'End Function
    Public Function ConvertiDataCustom(dataStr As String, NomeMacchina As String, CodArt As String) As Date
        Dim fmt As String = ""

        'Const fmt As String = "dd/MM/yyyy HH.mm.ss"

        Select Case NomeMacchina.ToUpperInvariant()
            Case GlobalConstants.MACHINENAME_MECCANOPLASTICA1,
                 GlobalConstants.MACHINENAME_MECCANOPLASTICA4
                fmt = "yyyy-MM-dd_HH.mm.ss"
                'Case GlobalConstants.MACHINENAME_DUETTI,
                '     GlobalConstants.MACHINENAME_DUETTI2
                '    Select Case CodArt(0)
                '        Case "P"c, "F"c
                '            fmt = "dd/MM/yyyy HH.mm.ss"
                '        Case "M"c
                '            fmt = "dd.MM.yyyy HH:mm:ss"

                '    End Select
            Case GlobalConstants.MACHINENAME_DUETTI
                fmt = "dd/MM/yyyy HH.mm.ss"
            Case GlobalConstants.MACHINENAME_DUETTI2, GlobalConstants.MACHINENAME_LAY
                fmt = "dd.MM.yyyy HH:mm:ss"
            Case GlobalConstants.MACHINENAME_AXOMATIC
                fmt = "yyyy-MM-dd_HH.mm.ss"

            Case Else
                Throw New ArgumentException($"Nome macchina {NomeMacchina} non riconosciuto durante la conversione in data del valore nel csv di produzione")
        End Select

        Return DateTime.ParseExact(
            dataStr,
            fmt,
            Globalization.CultureInfo.InvariantCulture
        )
    End Function
End Module
