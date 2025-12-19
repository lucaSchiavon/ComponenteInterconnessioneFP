
Imports System.Net
Imports System.Net.Mail


Public Class EmailManager
    Private ReadOnly _settings As Settings
    'Private ReadOnly _logRep As LogRep

    Public Sub New(settings As Settings)
        _settings = settings
        '_logRep = If(logger, New LogManager(settings))
    End Sub

    Public Sub SendEmail(ByVal sender As String, ByVal recipients As List(Of String), ByVal subject As String, ByVal body As String)
        Try
            If String.IsNullOrWhiteSpace(_settings.SmtpServer) Then
                Throw New ArgumentException("SMTP server non configurato")
            End If

            Dim mail As New MailMessage()
            mail.From = New MailAddress(If(String.IsNullOrWhiteSpace(sender), _settings.SmtpFrom, sender))
            For Each rcpt In recipients
                If Not String.IsNullOrWhiteSpace(rcpt) Then
                    mail.To.Add(rcpt)
                End If
            Next

            mail.Subject = subject
            mail.Body = body
            mail.IsBodyHtml = False

            Using smtp As New SmtpClient(_settings.SmtpServer, If(_settings.SmtpPort = 0, 25, _settings.SmtpPort))
                smtp.EnableSsl = _settings.SmtpEnableSsl
                smtp.UseDefaultCredentials = _settings.SmtpUseDefaultCredentials

                If Not _settings.SmtpUseDefaultCredentials Then
                    If Not String.IsNullOrWhiteSpace(_settings.SmtpUser) Then
                        smtp.Credentials = New NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword)
                    End If
                End If

                smtp.Send(mail)
            End Using

            'If logResult Then
            '    '_logRep.InsertLog("INFO", $"Email inviata a: {String.Join(";", recipients)} Oggetto: {subject}", Nothing, "", "", "")

            'End If

        Catch ex As Exception
            'If logResult Then
            '    '_logRep.InsertLog("ERROR", $"Errore invio email: {ex.Message}", ex.StackTrace, "", "", "")

            'End If
            Throw New Exception("Errore invio email: " & ex.Message, ex)
        End Try
    End Sub
End Class
