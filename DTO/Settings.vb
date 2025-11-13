Public Class Settings

    'la connessione al database del componente di interscambio dove viene scritto il log nella tabella TblLog
    Public Property ConnStr As String
    'livello di verbosità del log
    Public Property VerbosityLogLevel As String

    'nome utente e password per accedere alle cartelle condivise delle macchine di produzione
    'dove risiedono i CSV
    Public Property MachineFoldersUserName As String
    Public Property MachineFoldersPassword As String

    'Il nome del fornitore da usare quando si effettua il carico di produzione
    Public Property Fornitore As String


    Public Property ExpAuthType As String
    Public Property ExpAuthDittaCorrente As String
    Public Property ExpAuthUserNameTrust As String
    Public Property ExpAuthUserNameOffLineAuth As String
    Public Property ExpAuthUserNamePwdOffLineAuth As String
    Public Property ExpAuthDatabase As String
    Public Property ExpAuthProfile As String

    'ICA1
    Public Property Ica1Percorso As String
    Public Property Ica1PercorsoErrori As String
    Public Property Ica1PercorsoOld As String
    Public Property Ica1Serie As String

    'Meccanoplastica1
    Public Property Meccanoplastica1Percorso As String
    Public Property Meccanoplastica1PercorsoErrori As String
    Public Property Meccanoplastica1PercorsoOld As String
    Public Property Meccanoplastica1Serie As String

    'Public Property MeccanoplasticaOld As String
    'Public Property MeccanoplasticaErrori As String

    '' Sezione MECCANOPLASTICA4
    'Public Property Meccanoplastica4Percorso As String
    'Public Property Meccanoplastica4Old As String
    'Public Property Meccanoplastica4Errori As String
End Class
