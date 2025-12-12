Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Mail

Public Class EmailManager
    Private ReadOnly _settings As Settings
    Private ReadOnly _logger As LogManager

    Public Sub New(settings As Settings, Optional logger As LogManager = Nothing)
        _settings = settings
        _logger = If(logger, New LogManager(settings))
    End Sub

    Public Function SendEmail(ByVal sender As String, ByVal recipients As List(Of String), ByVal subject As String, ByVal body As String, Optional ByVal logResult As Boolean = True) As Boolean
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

            If logResult Then
                _logger.LogInfo($"Email inviata a: {String.Join(";", recipients)} Oggetto: {subject}")
            End If
            Return True
        Catch ex As Exception
            If logResult Then
                _logger.LogError($"Errore invio email: {ex.Message}", ex.StackTrace)
            End If
            Return False
        End Try
    End Function
End Class
