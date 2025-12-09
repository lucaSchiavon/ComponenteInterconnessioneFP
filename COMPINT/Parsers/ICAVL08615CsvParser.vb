Imports System.Globalization
Imports System.IO

Public Module ICAVL08615CsvParser
    Public Function ParseProduzioneCsv(percorsoFile As String, settings As Settings) As ICAVL08615CsvDto

        ' Lettura righe
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        'If righe.Length <> 2 Then
        '    Throw New InvalidDataException($"Il file {Path.GetFileName(percorsoFile)} deve contenere una riga di intestazione e una sola riga di dati.")
        'End If


        Dim intestazioniAttese As String() = {
            "Inizio turno", "Fine turno", "Codice articolo", "Doypack per scatole",
            "Tabs per doypack", "Velocità macchina", "Scarichi effettuati dosatore 1",
            "Scarichi effettuati dosatore 2", "Scarichi effettuati dosatore 3", "Scarichi effettuati dosatore 4",
            "Scarichi effettuati dosatore 5", "Scarichi effettuati dosatore 6", "Numero ordine", "Quantità in ordine", "Ricetta attiva",
            "Cialde espulse", "Passi eseguiti dal tamburo", "Passi eseguiti traino",
            "Passi eseguiti traino 2", "Carta svolta", "Cialde accettate", "Cialde riempite",
            "Doypack teorici prodotti", "Scatole teoriche prodotte"
        }

        Dim header() As String = righe(0).Split(";"c)
        If header.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException($"Numero di colonne non corrispondente nel file {Path.GetFileName(percorsoFile)} .")
        End If

        For i As Integer = 0 To intestazioniAttese.Length - 1
            If Not header(i).Trim().Equals(intestazioniAttese(i), StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidDataException($"Colonna {i + 1} non valida nel file {Path.GetFileName(percorsoFile)}: atteso '{intestazioniAttese(i)}', trovato '{header(i)}'")
            End If
        Next

        'Parsing della riga dati
        Dim dati() As String = righe(1).Split(";"c)
        If dati.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException($"La riga dati del file {Path.GetFileName(percorsoFile)} non contiene il numero corretto di colonne.")
        End If

        If Integer.Parse(dati(23)) = 0 Then
            'se non vi è stata produzione di alcun prodotto sposta in errore
            Throw New InvalidDataException($"Il file {Path.GetFileName(percorsoFile)} presenta una produzione pari a 0 nel campo Scatole teoriche prodotte")
        End If

        'Popolamento DTO
        Dim dto As New ICAVL08615CsvDto()
        Dim culture As CultureInfo = CultureInfo.InvariantCulture

        dto.InizioTurno = DateTime.Parse(dati(0), New CultureInfo("it-IT"))
        dto.FineTurno = DateTime.Parse(dati(1), New CultureInfo("it-IT"))
        dto.CodiceArticolo = dati(2).Trim().Replace("""", "")
        dto.DoypackPerScatola = Integer.Parse(dati(3))
        dto.TabsPerDoypack = Integer.Parse(dati(4))
        dto.VelocitaMacchina = Integer.Parse(dati(5))
        dto.ScarichiDosatore1 = Integer.Parse(dati(6))
        dto.ScarichiDosatore2 = Integer.Parse(dati(7))
        dto.ScarichiDosatore3 = Integer.Parse(dati(8))
        dto.ScarichiDosatore4 = Integer.Parse(dati(9))
        dto.ScarichiDosatore5 = Integer.Parse(dati(10))
        dto.ScarichiDosatore6 = Integer.Parse(dati(11))
        dto.NumeroOrdine = dati(12).Trim().Replace("""", "")
        dto.QuantitaInOrdine = Integer.Parse(dati(13))
        dto.RicettaAttiva = dati(14).Trim().Replace("""", "")
        dto.TabsEspulse = Integer.Parse(dati(15))
        dto.PassiTamburo = Integer.Parse(dati(16))
        dto.PassiTraino = Integer.Parse(dati(17))
        dto.PassiTraino2 = Integer.Parse(dati(18))
        dto.CartaSvolta = Double.Parse(dati(19), CultureInfo.InvariantCulture)
        dto.TabsAccettate = Integer.Parse(dati(20))
        dto.TabsProdotte = Integer.Parse(dati(21))
        dto.DoypackTeoriciProdotti = Integer.Parse(dati(22))
        dto.ScatoleTeoricheProdotte = Integer.Parse(dati(23))
        dto.Note = settings.ICAVL08615NomeMacchina & Environment.NewLine & righe(0) & Environment.NewLine & righe(1)

        Return dto
    End Function
End Module
