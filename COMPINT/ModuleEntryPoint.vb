Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint


    'todo:
    '    Crea un metodo SpedisciEmailNotifica vicino a PrintConsoleMsg In LogManager e chiamalo nei vari metodi di Logmanager dove vedi  'todo:invia una mail se l'errore o la info è presente nella tabella TblErrorsUsedForNotifications
    'Il metodo dovrà verificare se la variabile messaggio è presente nel campo ErrorsToCheckForNotifications scorrendosi la lista ritornata da ErrorsUsedForNotificationsRep.GetAll()
    'in caso affermativo spedisce una mail agli indirizzi presenti nel campo ErrorUsedForNotificationDto.Addresses usando ErrorUsedForNotificationDto.sender come mittente

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
