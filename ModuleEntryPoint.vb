Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()


        'todo creare un log più verboso quando aggiunge il carico:
        'finire di scrivere  Private Function GetMsgForConsole(livello As String, messaggio As String, stackTrace As String, macchina As String, codArt As String, fileName As String) As String
        'Dim strRet As String = $"{livello} - {messaggio} {Environment.NewLine} "
        'End Function
        'todo:attivare un log debug ed attivare la verbositylevel

        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
