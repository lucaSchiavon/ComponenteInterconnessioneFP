Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()


        'todo:finire refactor portando cio inerente al carico nel caricomanager
        'todo:gestire il doppio file dopo la copia del file elaborato
        'todo:dinamicizzare la creazione del nome del file che viene copiato
        'todo:verifica generazione conter nome lotto



        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
