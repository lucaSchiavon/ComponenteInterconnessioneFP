Public Class LogManager
    'questo manager si occupa di loggare l'applicazione
    'e scrive nella tabella di log del database dove traccia le eccezioni più importanti
    'ma può scrivere anche nella console se verbosity level è settato a debug

    Private ReadOnly _settings As Settings
    Private ReadOnly _logRep As LogRep

    Public Sub New(settings As Settings)
        _settings = settings
        _logRep = New LogRep(settings.ConnStr)
    End Sub

    Public Sub LogInfo(messaggio As String, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("INFO", messaggio, Nothing, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogWarning(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("WARNING", messaggio, stackTrace, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogError(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("ERROR", messaggio, stackTrace, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Private Sub PrintConsoleMsg(messaggio As String, macchina As String, codArt As String, fileName As String)
        If _settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG AndAlso Environment.UserInteractive Then
            Console.WriteLine($"{_settings.VerbosityLogLevel} - {messaggio} {Environment.NewLine} Macchina: {macchina} CodArt: {codArt} Nome del file csv: {fileName}")
        End If
    End Sub

End Class
