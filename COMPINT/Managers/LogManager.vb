Imports System.Text.RegularExpressions

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
        'todo:invia una mail se l'errore o la info è presente nella tabella TblErrorsUsedForNotifications
        'SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogWarning(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("WARNING", messaggio, stackTrace, macchina, codArt, fileName)
        'todo:invia una mail se l'errore o la info è presente nella tabella TblErrorsUsedForNotifications
        'SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogError(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("ERROR", messaggio, stackTrace, macchina, codArt, fileName)
        'todo:invia una mail se l'errore o la info è presente nella tabella TblErrorsUsedForNotifications
        'SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Private Sub PrintConsoleMsg(messaggio As String, macchina As String, codArt As String, fileName As String)
        If _settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG AndAlso Environment.UserInteractive Then
            Console.WriteLine($"{_settings.VerbosityLogLevel} - {messaggio} {Environment.NewLine} Macchina: {macchina} CodArt: {codArt} Nome del file csv: {fileName}")
        End If
    End Sub

    Private Function WildcardPatternToRegex(pattern As String) As String
        ' Trasforma un pattern che usa [...] per parti variabili in una regex.
        ' Esempio: "L'articolo [...] nella riga [...] degli SCARICHI" -> "L'articolo \[.*?\] nella riga \[.*?\] degli SCARICHI"
        If String.IsNullOrWhiteSpace(pattern) Then Return String.Empty

        ' Escape regex metacharacters first, then replace escaped \[\.\.\.\] with a capture of any content
        Dim escaped As String = Regex.Escape(pattern)
        ' l'escape rende \[ \.\.\. \] per la sequenza [...]
        Dim regexPattern As String = Regex.Replace(escaped, Regex.Escape("[...]"), ".*?", RegexOptions.IgnoreCase)

        ' Allow whitespace and minor differences: normalize spaces
        Return "^\s*" & regexPattern & "\s*$"
    End Function

    Public Sub SpedisciEmailNotifica(messaggio As String)
        Try
            Dim repo As New ErrorsUsedForNotificationsRep(_settings.ConnStr)
            Dim rules As List(Of ErrorUsedForNotificationDto) = repo.GetAll()

            If rules Is Nothing OrElse rules.Count = 0 Then
                Return
            End If

            Dim emailManager As New EmailManager(_settings, Me)

            For Each rule As ErrorUsedForNotificationDto In rules
                Try
                    Dim genericPattern As String = rule.ErrorsToCheckForNotifications
                    If String.IsNullOrWhiteSpace(genericPattern) Then Continue For

                    Dim regexPattern As String = WildcardPatternToRegex(genericPattern)

                    Dim match As Boolean = False
                    If Not String.IsNullOrWhiteSpace(regexPattern) Then
                        match = Regex.IsMatch(messaggio, regexPattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
                    End If

                    If match Then
                        ' Parse addresses separated by ; or ,
                        Dim addresses As New List(Of String)()
                        If Not String.IsNullOrWhiteSpace(rule.Addresses) Then
                            For Each addr As String In rule.Addresses.Split({";", ","}, StringSplitOptions.RemoveEmptyEntries)
                                Dim trimmed = addr.Trim()
                                If Not String.IsNullOrWhiteSpace(trimmed) Then addresses.Add(trimmed)
                            Next
                        End If

                        If addresses.Count = 0 Then
                            ' Nothing to send to
                            Continue For
                        End If

                        Dim sender As String = If(String.IsNullOrWhiteSpace(rule.Sender), _settings.SmtpFrom, rule.Sender)
                        Dim subject As String = If(String.IsNullOrWhiteSpace(rule.SubjectOfEmail), "Notifica da sistema", rule.SubjectOfEmail)

                        Try
                            ' Evitare logging ricorsivo durante l'invio
                            emailManager.SendEmail(sender, addresses, subject, messaggio, logResult:=False)
                            ' Log a livello informativo dell'azione di notifica
                            _logRep.InsertLog("INFO", $"Invio notifica email a: {String.Join(";", addresses)} soggetto: {subject}", Nothing, "", "", "")
                        Catch ex As Exception
                            _logRep.InsertLog("ERROR", $"Errore invio notifica email: {ex.Message}", ex.StackTrace, "", "", "")
                        End Try
                    End If

                Catch exRule As Exception
                    _logRep.InsertLog("ERROR", $"Errore durante l'elaborazione della regola di notifica: {exRule.Message}", exRule.StackTrace, "", "", "")
                End Try
            Next

        Catch ex As Exception
            _logRep.InsertLog("ERROR", $"Errore SpedisciEmailNotifica: {ex.Message}", ex.StackTrace, "", "", "")
        End Try
    End Sub

End Class
