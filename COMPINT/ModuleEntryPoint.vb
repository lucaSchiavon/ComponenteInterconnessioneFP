Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()

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
