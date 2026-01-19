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
        SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogWarning(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("WARNING", messaggio, stackTrace, macchina, codArt, fileName)
        SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogError(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        _logRep.InsertLog("ERROR", messaggio, stackTrace, macchina, codArt, fileName)
        SpedisciEmailNotifica(messaggio)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogDebug(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        If _settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG Then
            _logRep.InsertLog("DEBUG", messaggio, stackTrace, macchina, codArt, fileName)
        End If
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Private Sub PrintConsoleMsg(messaggio As String, macchina As String, codArt As String, fileName As String)
        If _settings.VerbosityLogLevel.ToUpper() = VERBOSITYLOGLEVEL_DEBUG AndAlso Environment.UserInteractive Then
            Console.WriteLine($"{_settings.VerbosityLogLevel} - {messaggio} {Environment.NewLine} Macchina: {macchina} CodArt: {codArt} Nome del file csv: {fileName}")
        End If
    End Sub


    Public Sub SpedisciEmailNotifica(messaggio As String)
        Try
            Dim repo As New ErrorsUsedForNotificationsRep(_settings.ConnStr)
            Dim rules As List(Of ErrorUsedForNotificationDto) = repo.GetAll()

            If rules Is Nothing OrElse rules.Count = 0 Then
                Return
            End If

            Dim emailManager As New EmailManager(_settings)

            For Each rule As ErrorUsedForNotificationDto In rules
                Try
                    Dim genericPattern As String = rule.ErrorsToCheckForNotifications
                    If String.IsNullOrWhiteSpace(genericPattern) Then Continue For

                    Dim regexPattern As String = WildcardPatternToRegex(genericPattern)

                    Dim match As Boolean = False
                    If Not String.IsNullOrWhiteSpace(regexPattern) Then
                        'considera il testo come una unica riga, anche se ne ha due e il testo va quindi a capo
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
                            emailManager.SendEmail(sender, addresses, subject, messaggio)
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


    Private Function WildcardPatternToRegex(pattern As String) As String
        If String.IsNullOrWhiteSpace(pattern) Then Return String.Empty

        ' Suddividi il pattern sulle sequenze [...]
        Dim parts As String() = pattern.Split(New String() {"[...]"}, StringSplitOptions.None)

        ' Ricostruisci la regex: ogni parte fissa viene escappata, tra una parte e l'altra metti .*?
        Dim sb As New System.Text.StringBuilder()
        For i As Integer = 0 To parts.Length - 1
            'fa precedere ogni cartattere speciale con backslash in modo che i caratteri speciali vengano considerati normali e non come operatori regex
            sb.Append(Regex.Escape(parts(i)))
            If i < parts.Length - 1 Then
                sb.Append(".*?") ' corrisponde a qualsiasi cosa (non greedy)
            End If
        Next

        ' Aggiungi ancore per matchare l'intera stringa
        Return "^\s*" & sb.ToString() & "\s*$"
    End Function

End Class
