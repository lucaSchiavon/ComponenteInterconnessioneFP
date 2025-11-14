Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()

        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
