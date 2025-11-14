Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()

        'todo:attivare un log debug ed attivare la verbositylevel
        'creare un log più verboso quando aggiunge il carico

        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
