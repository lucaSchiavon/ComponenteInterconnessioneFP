Imports System.IO

Public Class SettingsManager
    Private parser As IniParser

    Public Sub New(iniFilePath As String)
        parser = New IniParser(iniFilePath)
    End Sub

    Public Function LoadSettings() As Settings
        Dim settings As New Settings()

        ' ROOT
        settings.ConnStr = parser.GetSetting("ROOT", "ConnStr")

        settings.VerbosityLogLevel = parser.GetSetting("ROOT", "VerbosityLogLevel")

        settings.MachineFoldersSecurity = parser.GetSetting("MACHINEFOLDERSECURITY", "MachineFoldersSecurity")
        settings.MachineFoldersUserName = parser.GetSetting("MACHINEFOLDERSECURITY", "MachineFoldersUserName")
        settings.MachineFoldersPassword = parser.GetSetting("MACHINEFOLDERSECURITY", "MachineFoldersPassword")

        settings.Fornitore = parser.GetSetting("ROOT", "Fornitore")
        settings.TipoBfCaricoScarico = parser.GetSetting("ROOT", "TipoBfCaricoScarico")
        settings.TipoBfScarico = parser.GetSetting("ROOT", "TipoBfScarico")

        ' SMTP
        settings.SmtpServer = parser.GetSetting("SMTP", "Server")
        Integer.TryParse(parser.GetSetting("SMTP", "Port"), settings.SmtpPort)
        settings.SmtpUser = parser.GetSetting("SMTP", "User")
        settings.SmtpPassword = parser.GetSetting("SMTP", "Password")
        Boolean.TryParse(parser.GetSetting("SMTP", "EnableSsl"), settings.SmtpEnableSsl)
        Boolean.TryParse(parser.GetSetting("SMTP", "UseDefaultCredentials"), settings.SmtpUseDefaultCredentials)
        settings.SmtpFrom = parser.GetSetting("SMTP", "From")

        'LOTTOPAGLIERI
        settings.NomeLottoPagCampoFissoFp = parser.GetSetting("LOTTOPAGLIERI", "NomeLottoPagCampoFissoFp")

        'LOTTOCONTER
        settings.NumeroLineaDiRiempimento = parser.GetSetting("LOTTOCONTER", "NumeroLineaDiRiempimento")
        settings.LetteraIdentifFp = parser.GetSetting("LOTTOCONTER", "LetteraIdentifFp")



        'EXPERIENCEAUTH
        settings.ExpAuthType = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthType")
        settings.ExpAuthDittaCorrente = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthDittaCorrente")
        settings.ExpAuthUserNameTrust = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNameTrust")
        settings.ExpAuthUserNameOffLineAuth = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNameOffLineAuth")
        settings.ExpAuthUserNamePwdOffLineAuth = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNamePwdOffLineAuth")
        settings.ExpAuthDatabase = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthDatabase")
        settings.ExpAuthProfile = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthProfile")

        ' ICAVL08615
        settings.ICAVL08615Percorso = parser.GetSetting("ICAVL08615", "Percorso")
        settings.ICAVL08615PercorsoErrori = Path.Combine(settings.ICAVL08615Percorso, "Errori")
        settings.ICAVL08615PercorsoOld = Path.Combine(settings.ICAVL08615Percorso, "Old")
        settings.ICAVL08615NomeMacchina = parser.GetSetting("ICAVL08615", "NomeMacchina")
        settings.ICAVL08615Magazzino = parser.GetSetting("ICAVL08615", "Magazzino")

        ' ICAVL08616
        settings.ICAVL08616Percorso = parser.GetSetting("ICAVL08616", "Percorso")
        settings.ICAVL08616PercorsoErrori = Path.Combine(settings.ICAVL08616Percorso, "Errori")
        settings.ICAVL08616PercorsoOld = Path.Combine(settings.ICAVL08616Percorso, "Old")
        settings.ICAVL08616NomeMacchina = parser.GetSetting("ICAVL08616", "NomeMacchina")
        settings.ICAVL08616Magazzino = parser.GetSetting("ICAVL08616", "Magazzino")

        ' MECCANOPLASTICA1
        settings.Meccanoplastica1Percorso = parser.GetSetting("MECCANOPLASTICA1", "Percorso")
        settings.Meccanoplastica1PercorsoErrori = Path.Combine(settings.Meccanoplastica1Percorso, "Errori")
        settings.Meccanoplastica1PercorsoOld = Path.Combine(settings.Meccanoplastica1Percorso, "Old")
        settings.Meccanoplastica1NomeMacchina = parser.GetSetting("MECCANOPLASTICA1", "NomeMacchina")
        settings.Meccanoplastica1Magazzino = parser.GetSetting("MECCANOPLASTICA1", "Magazzino")

        ' MECCANOPLASTICA4
        settings.Meccanoplastica4Percorso = parser.GetSetting("MECCANOPLASTICA4", "Percorso")
        settings.Meccanoplastica4PercorsoErrori = Path.Combine(settings.Meccanoplastica4Percorso, "Errori")
        settings.Meccanoplastica4PercorsoOld = Path.Combine(settings.Meccanoplastica4Percorso, "Old")
        settings.Meccanoplastica4NomeMacchina = parser.GetSetting("MECCANOPLASTICA4", "NomeMacchina")
        settings.Meccanoplastica4Magazzino = parser.GetSetting("MECCANOPLASTICA4", "Magazzino")

        ' DUETTI
        settings.DuettiPercorso = parser.GetSetting("DUETTI", "Percorso")
        settings.DuettiPercorsoErrori = Path.Combine(settings.DuettiPercorso, "Errori")
        settings.DuettiPercorsoOld = Path.Combine(settings.DuettiPercorso, "Old")
        settings.DuettiNomeMacchina = parser.GetSetting("DUETTI", "NomeMacchina")
        settings.DuettiMagazzino = parser.GetSetting("DUETTI", "Magazzino")

        ' DUETTI2
        settings.Duetti2Percorso = parser.GetSetting("DUETTI2", "Percorso")
        settings.Duetti2PercorsoErrori = Path.Combine(settings.Duetti2Percorso, "Errori")
        settings.Duetti2PercorsoOld = Path.Combine(settings.Duetti2Percorso, "Old")
        settings.Duetti2NomeMacchina = parser.GetSetting("DUETTI2", "NomeMacchina")
        settings.Duetti2Magazzino = parser.GetSetting("DUETTI2", "Magazzino")

        'AXOMATIC
        settings.AxomaticPercorso = parser.GetSetting("AXOMATIC", "Percorso")
        settings.AxomaticPercorsoErrori = Path.Combine(settings.AxomaticPercorso, "Errori")
        settings.AxomaticPercorsoOld = Path.Combine(settings.AxomaticPercorso, "Old")
        settings.AxomaticNomeMacchina = parser.GetSetting("AXOMATIC", "NomeMacchina")
        settings.AxomaticMagazzino = parser.GetSetting("AXOMATIC", "Magazzino")

        'ETICH
        settings.EtichPercorso = parser.GetSetting("ETICH", "Percorso")
        settings.EtichPercorsoErrori = Path.Combine(settings.EtichPercorso, "Errori")
        settings.EtichPercorsoOld = Path.Combine(settings.EtichPercorso, "Old")
        settings.EtichNomeMacchina = parser.GetSetting("ETICH", "NomeMacchina")
        settings.EtichMagazzino = parser.GetSetting("ETICH", "Magazzino")

        'LAY
        settings.LayPercorso = parser.GetSetting("LAY", "Percorso")
        settings.LayPercorsoErrori = Path.Combine(settings.LayPercorso, "Errori")
        settings.LayPercorsoOld = Path.Combine(settings.LayPercorso, "Old")
        settings.LayNomeMacchina = parser.GetSetting("LAY", "NomeMacchina")
        settings.LayMagazzino = parser.GetSetting("LAY", "Magazzino")

        'PICKER23017
        settings.Picker23017Percorso = parser.GetSetting("PICKER23017", "Percorso")
        settings.Picker23017PercorsoErrori = Path.Combine(settings.Picker23017Percorso, "Errori")
        settings.Picker23017PercorsoOld = Path.Combine(settings.Picker23017Percorso, "Old")
        settings.Picker23017NomeMacchina = parser.GetSetting("PICKER23017", "NomeMacchina")
        settings.Picker23017Magazzino = parser.GetSetting("PICKER23017", "Magazzino")

        'PICKER23018
        settings.Picker23018Percorso = parser.GetSetting("PICKER23018", "Percorso")
        settings.Picker23018PercorsoErrori = Path.Combine(settings.Picker23018Percorso, "Errori")
        settings.Picker23018PercorsoOld = Path.Combine(settings.Picker23018Percorso, "Old")
        settings.Picker23018NomeMacchina = parser.GetSetting("PICKER23018", "NomeMacchina")
        settings.Picker23018Magazzino = parser.GetSetting("PICKER23018", "Magazzino")






        Return settings
    End Function
End Class
