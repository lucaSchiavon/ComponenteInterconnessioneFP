Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()

        'todo:verifica generazione conter nome lotto
        'todo:ricordarsi di valutare con elena in caso di counter che non ci stà nella tabella...le prod devono essere sequenziali

        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
