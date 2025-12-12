Public Class Settings

    'ROOT
    'la connessione al database del componente di interscambio dove viene scritto il log nella tabella TblLog
    Public Property ConnStr As String
    'livello di verbosità del log
    Public Property VerbosityLogLevel As String 'valori ammessi: INFO, DEBUG

    'SMTP
    Public Property SmtpServer As String
    Public Property SmtpPort As Integer
    Public Property SmtpUser As String
    Public Property SmtpPassword As String
    Public Property SmtpEnableSsl As Boolean
    Public Property SmtpUseDefaultCredentials As Boolean
    Public Property SmtpFrom As String

    'nome utente e password per accedere alle cartelle condivise delle macchine di produzione
    'dove risiedono i CSV
    Public Property MachineFoldersSecurity As String
    Public Property MachineFoldersUserName As String
    Public Property MachineFoldersPassword As String

    'Il nome del fornitore da usare quando si effettua il carico di produzione
    Public Property Fornitore As String
    'il tipo di bollo fattura
    Public Property TipoBfCaricoScarico As String
    Public Property TipoBfScarico As String

    'LOTTOPAGLIERI
    Public Property NomeLottoPagCampoFissoFp As String

    'LOTTOCONTER
    Public Property NumeroLineaDiRiempimento As String
    Public Property LetteraIdentifFp As String


    'EXPERIENCEAUTH
    Public Property ExpAuthType As String 'valori ammessi TRUSTED, OFFLINE
    Public Property ExpAuthDittaCorrente As String
    Public Property ExpAuthUserNameTrust As String
    Public Property ExpAuthUserNameOffLineAuth As String
    Public Property ExpAuthUserNamePwdOffLineAuth As String
    Public Property ExpAuthDatabase As String
    Public Property ExpAuthProfile As String

    'ICAVL08615
    Public Property ICAVL08615Percorso As String
    Public Property ICAVL08615PercorsoErrori As String
    Public Property ICAVL08615PercorsoOld As String
    Public Property ICAVL08615NomeMacchina As String

    'ICAVL08616
    Public Property ICAVL08616Percorso As String
    Public Property ICAVL08616PercorsoErrori As String
    Public Property ICAVL08616PercorsoOld As String
    Public Property ICAVL08616NomeMacchina As String

    'MECCANOPLASTICA1
    Public Property Meccanoplastica1Percorso As String
    Public Property Meccanoplastica1PercorsoErrori As String
    Public Property Meccanoplastica1PercorsoOld As String
    Public Property Meccanoplastica1NomeMacchina As String

    'MECCANOPLASTICA4
    Public Property Meccanoplastica4Percorso As String
    Public Property Meccanoplastica4PercorsoErrori As String
    Public Property Meccanoplastica4PercorsoOld As String
    Public Property Meccanoplastica4NomeMacchina As String

    'DUETTI
    Public Property DuettiPercorso As String
    Public Property DuettiPercorsoErrori As String
    Public Property DuettiPercorsoOld As String
    Public Property DuettiNomeMacchina As String

    'DUETTI2
    Public Property Duetti2Percorso As String
    Public Property Duetti2PercorsoErrori As String
    Public Property Duetti2PercorsoOld As String
    Public Property Duetti2NomeMacchina As String

    'AXOMATIC
    Public Property AxomaticPercorso As String
    Public Property AxomaticPercorsoErrori As String
    Public Property AxomaticPercorsoOld As String
    Public Property AxomaticNomeMacchina As String

    'ETICH
    Public Property EtichPercorso As String
    Public Property EtichPercorsoErrori As String
    Public Property EtichPercorsoOld As String
    Public Property EtichNomeMacchina As String

    'LAY
    Public Property LayPercorso As String
    Public Property LayPercorsoErrori As String
    Public Property LayPercorsoOld As String
    Public Property LayNomeMacchina As String

    'PICKER23017
    Public Property Picker23017Percorso As String
    Public Property Picker23017PercorsoErrori As String
    Public Property Picker23017PercorsoOld As String
    Public Property Picker23017NomeMacchina As String

    'PICKER23018
    Public Property Picker23018Percorso As String
    Public Property Picker23018PercorsoErrori As String
    Public Property Picker23018PercorsoOld As String
    Public Property Picker23018NomeMacchina As String

End Class
