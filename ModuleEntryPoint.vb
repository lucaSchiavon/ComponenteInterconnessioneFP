Imports System.Threading.Thread
Imports System.Globalization

Module ModuleEntryPoint



    Sub Main()

        'todo:ARRIVATO A TESTARE MECCANOPLASTICA1 INSERIMENTO MOUSE... VUOLE UNA NUMERAZIONE...INVENTARLA E METTERE B
        'todo:finire di testare gestisci eventi con la i log (riguardare video per capire meglio)
        'todo: RIFINIRE MEGLIO TUTTI I LOG INSERENDO MACCHINA, ARTICOLO ED ALTRE INFO IDENTIFICATIVE
        'todo: rivedere la logica di creazione lotto e creazione movimento di carico


        'avvia il componente di interconnessione
        Dim OCompInterconnManager = New CompInterconnManager()

        OCompInterconnManager.Main()
    End Sub


End Module
