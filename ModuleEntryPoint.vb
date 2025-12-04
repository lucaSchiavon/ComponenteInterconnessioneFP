Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()
        '--------------------------old code
        'todo:verifica generazione conter nome lotto
        'todo:ricordarsi di valutare con elena in caso di counter che non ci stà nella tabella...le prod devono essere sequenziali

        'avvia il componente di interconnessione
        'Dim OCompInterconnManager = New CompInterconnManager()

        'OCompInterconnManager.Main()
        '----------------------------------

        'avvia il componente di interconnessione
        Dim OCompInterconnManager As New CompInterconnManager()
        Dim exitCode As Integer = 0

        Try
            exitCode = OCompInterconnManager.EseguiJobPrincipale()
        Catch ex As Exception
            Console.WriteLine("Errore critico: " & ex.Message)
            exitCode = 1
        End Try

        ' Chiude esplicitamente il processo con codice di uscita
        Environment.Exit(exitCode)
    End Sub


End Module
