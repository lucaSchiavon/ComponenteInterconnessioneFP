
Module ModuleEntryPoint

    'todo:rivedere i valori di fallback per la mail
    'todo:creare due routine per gestire i carichi etich
    'todo:far compilare gli unittest
    'todo.finire interfaccia ed integrare compint_ui nella solution

    Sub Main()

        'avvia il componente di interconnessione
        Dim OCompInterconnManager As New CompInterconnManager()
        Dim exitCode As Integer = 0

        Try
            exitCode = OCompInterconnManager.EseguiJobPrincipale()
        Catch ex As Exception
            Console.WriteLine("Errore critico: " & ex.Message)
            LogTestoManager.LoggaErrore(ex)
            exitCode = 1
        End Try

        ' Chiude esplicitamente il processo con codice di uscita
        Environment.Exit(exitCode)
    End Sub


End Module
