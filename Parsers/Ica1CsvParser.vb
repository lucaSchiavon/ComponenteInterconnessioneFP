Imports System.Globalization
Imports System.IO

Module Ica1CsvParser
    Public Function ParseProduzioneCsv(percorsoFile As String) As Ica1CsvDto
        ' Controlli di base
        'If Not File.Exists(percorsoFile) Then
        '    Throw New FileNotFoundException($"File non trovato: {percorsoFile}")
        'End If

        'If Not Path.GetExtension(percorsoFile).Equals(".csv", StringComparison.OrdinalIgnoreCase) Then
        '    Throw New InvalidDataException("Il file deve avere estensione .csv")
        'End If

        ' Lettura righe
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        If righe.Length <> 2 Then
            Throw New InvalidDataException("Il file deve contenere una riga di intestazione e una sola riga di dati.")
        End If

        'Intestazioni previste
        Dim intestazioniAttese As String() = {
            "Inizio turno", "Fine turno", "Codice articolo", "Doypack/barattoli per scatola",
            "Tabs per doypack/barattoli", "Velocità macchina", "Scarichi effettuati dosatore 1",
            "Scarichi effettuati dosatore 2", "Scarichi effettuati dosatore 3", "Scarichi effettuati dosatore 4",
            "Scarichi effettuati dosatore 5", "Scarichi effettuati dosatore 6", "Ricetta attiva",
            "Tabs espulse", "Passi eseguiti dal tamburo", "Passi eseguiti traino",
            "Passi eseguiti traino 2", "Carta svolta", "Tabs accettate", "Tabs prodotte",
            "Doypack/barattoli teorici prodotti", "Scatole teoriche prodotte"
        }

        Dim header() As String = righe(0).Split(";"c)
        If header.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException("Numero di colonne non corrispondente.")
        End If

        For i As Integer = 0 To intestazioniAttese.Length - 1
            If Not header(i).Trim().Equals(intestazioniAttese(i), StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidDataException($"Colonna {i + 1} non valida: atteso '{intestazioniAttese(i)}', trovato '{header(i)}'")
            End If
        Next

        'Parsing della riga dati
        Dim dati() As String = righe(1).Split(";"c)
        If dati.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException("La riga dati non contiene il numero corretto di colonne.")
        End If

        'Popolamento DTO
        Dim dto As New Ica1CsvDto()
        Dim culture As CultureInfo = CultureInfo.InvariantCulture

        dto.InizioTurno = DateTime.Parse(dati(0), New CultureInfo("it-IT"))
        dto.FineTurno = DateTime.Parse(dati(1), New CultureInfo("it-IT"))
        dto.CodiceArticolo = dati(2).Trim()
        dto.DoypackPerScatola = Integer.Parse(dati(3))
        dto.TabsPerDoypack = Integer.Parse(dati(4))
        dto.VelocitaMacchina = Integer.Parse(dati(5))
        dto.ScarichiDosatore1 = Integer.Parse(dati(6))
        dto.ScarichiDosatore2 = Integer.Parse(dati(7))
        dto.ScarichiDosatore3 = Integer.Parse(dati(8))
        dto.ScarichiDosatore4 = Integer.Parse(dati(9))
        dto.ScarichiDosatore5 = Integer.Parse(dati(10))
        dto.ScarichiDosatore6 = Integer.Parse(dati(11))
        dto.RicettaAttiva = dati(12).Trim()
        dto.TabsEspulse = Integer.Parse(dati(13))
        dto.PassiTamburo = Integer.Parse(dati(14))
        dto.PassiTraino = Integer.Parse(dati(15))
        dto.PassiTraino2 = Integer.Parse(dati(16))
        dto.CartaSvolta = Double.Parse(dati(17), CultureInfo.InvariantCulture)
        dto.TabsAccettate = Integer.Parse(dati(18))
        dto.TabsProdotte = Integer.Parse(dati(19))
        dto.DoypackTeoriciProdotti = Integer.Parse(dati(20))
        dto.ScatoleTeoricheProdotte = Integer.Parse(dati(21))

        Return dto
    End Function
End Module
