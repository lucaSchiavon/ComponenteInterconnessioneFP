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

        settings.MachineFoldersUserName = parser.GetSetting("ROOT", "MachineFoldersUserName")
        settings.MachineFoldersPassword = parser.GetSetting("ROOT", "MachineFoldersPassword")


        settings.Fornitore = parser.GetSetting("ROOT", "Fornitore")

        'EXPERIENCEAUTH
        settings.ExpAuthType = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthType")
        settings.ExpAuthDittaCorrente = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthDittaCorrente")
        settings.ExpAuthUserNameTrust = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNameTrust")
        settings.ExpAuthUserNameOffLineAuth = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNameOffLineAuth")
        settings.ExpAuthUserNamePwdOffLineAuth = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthUserNamePwdOffLineAuth")
        settings.ExpAuthDatabase = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthDatabase")
        settings.ExpAuthProfile = parser.GetSetting("EXPERIENCEAUTH", "ExpAuthProfile")

        ' ICA1
        settings.Ica1Percorso = parser.GetSetting("ICA1", "Percorso")
        settings.Ica1PercorsoErrori = Path.Combine(settings.Ica1Percorso, "Errori")
        settings.Ica1PercorsoOld = Path.Combine(settings.Ica1Percorso, "Old")
        settings.Ica1Serie = parser.GetSetting("ICA1", "Serie")

        ' MECCANOPLASTICA1
        settings.Meccanoplastica1Percorso = parser.GetSetting("MECCANOPLASTICA1", "Percorso")
        settings.Meccanoplastica1PercorsoErrori = Path.Combine(settings.Meccanoplastica1Percorso, "Errori")
        settings.Meccanoplastica1PercorsoOld = Path.Combine(settings.Meccanoplastica1Percorso, "Old")
        settings.Meccanoplastica1Serie = parser.GetSetting("MECCANOPLASTICA1", "Serie")
        'settings.MeccanoplasticaOld = parser.GetSetting("MECCANOPLASTICA", "Old")
        'settings.MeccanoplasticaErrori = parser.GetSetting("MECCANOPLASTICA", "Errori")

        '' MECCANOPLASTICA4
        'settings.Meccanoplastica4Percorso = parser.GetSetting("MECCANOPLASTICA4", "Percorso")
        'settings.Meccanoplastica4Old = parser.GetSetting("MECCANOPLASTICA4", "Old")
        'settings.Meccanoplastica4Errori = parser.GetSetting("MECCANOPLASTICA4", "Errori")

        Return settings
    End Function
End Class
