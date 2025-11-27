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
        settings.TipoBf = parser.GetSetting("ROOT", "TipoBf")

        'LOTTOPAGLIERI
        settings.NomeLottoPagCampoFissoFp = parser.GetSetting("LOTTOPAGLIERI", "NomeLottoPagCampoFissoFp")


        'LOTTOCONTER
        settings.NomeLottoPagCampoFissoFp = parser.GetSetting("LOTTOCONTER", "NumeroLineaDiRiempimento")
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

        ' ICAVL08616
        settings.ICAVL08616Percorso = parser.GetSetting("ICAVL08616", "Percorso")
        settings.ICAVL08616PercorsoErrori = Path.Combine(settings.ICAVL08616Percorso, "Errori")
        settings.ICAVL08616PercorsoOld = Path.Combine(settings.ICAVL08616Percorso, "Old")
        settings.ICAVL08616NomeMacchina = parser.GetSetting("ICAVL08616", "NomeMacchina")

        ' MECCANOPLASTICA1
        settings.Meccanoplastica1Percorso = parser.GetSetting("MECCANOPLASTICA1", "Percorso")
        settings.Meccanoplastica1PercorsoErrori = Path.Combine(settings.Meccanoplastica1Percorso, "Errori")
        settings.Meccanoplastica1PercorsoOld = Path.Combine(settings.Meccanoplastica1Percorso, "Old")
        'settings.Meccanoplastica1Serie = parser.GetSetting("MECCANOPLASTICA1", "Serie")
        settings.Meccanoplastica1NomeMacchina = parser.GetSetting("MECCANOPLASTICA1", "NomeMacchina")



        Return settings
    End Function
End Class
