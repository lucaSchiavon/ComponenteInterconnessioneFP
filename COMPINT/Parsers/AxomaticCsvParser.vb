Imports System.Globalization
Imports System.IO

Module AxomaticCsvParser

    ' Parser tollerante per CSV malformati Axomatic
    Private Function ParseCsvLineTollerante(riga As String) As String()
        If String.IsNullOrWhiteSpace(riga) Then
            Return Array.Empty(Of String)()
        End If

        Dim s As String = riga.Trim()

        ' Rimuove virgolette esterne se presenti
        If s.StartsWith("""") AndAlso s.EndsWith("""") Then
            s = s.Substring(1, s.Length - 2)
        End If

        ' Sostituisce doppie virgolette con una singola
        s = s.Replace("""""", """")

        ' Ora splitta normalmente
        Return s.Split(","c)
    End Function

    Public Function ParseProduzioneCsvTollerante(percorsoFile As String, settings As Settings) As AxomaticCsvDto

        Dim NomeFile As String = Path.GetFileName(percorsoFile)
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        If righe.Length < 2 Then
            Throw New InvalidDataException($"Il file {NomeFile} non contiene abbastanza righe.")
        End If

        ' --- PARSING HEADER TOLLERANTE ---
        Dim header() As String = ParseCsvLineTollerante(righe(0))

        ' Intestazioni attese (senza doppie virgolette)
        Dim intestazioniAttese As String() = {
            "Data_Ora", "NomeRicetta", "PezziBuoni", "PezziScartati", "Pezzicartone", "CartoniBuoni"
        }

        If header.Length <> intestazioniAttese.Length Then
            ' Non bloccare: salva comunque nelle note
            ' e continua il parsing dei dati
        End If

        ' --- PARSING DATI TOLLERANTE ---
        Dim dati() As String = ParseCsvLineTollerante(righe(1))

        If dati.Length < 6 Then
            Throw New InvalidDataException($"La riga dati nel file {NomeFile} non contiene abbastanza colonne.")
        End If

        If Integer.Parse(dati(5).Trim().Replace("""", "")) = 0 Then
            'se non vi è stata produzione di alcun prodotto sposta in errore
            Throw New InvalidDataException($"Il file {Path.GetFileName(percorsoFile)} presenta una produzione pari a 0 nel campo CartoniBuoni")
        End If
        ' --- POPOLAMENTO DTO ---
        Dim dto As New AxomaticCsvDto()

        ' Data
        Dim dataString As String = dati(0).Trim().Replace("""", "")
        dto.Data_Ora = DateTime.Parse(
            General.ConvertiDataCustom(dataString, GlobalConstants.MACHINENAME_AXOMATIC),
            New CultureInfo("it-IT")
        )

        ' Altri campi
        dto.NomeRicetta = dati(1).Trim().Replace("""", "")
        dto.PezziBuoni = Integer.Parse(dati(2).Trim().Replace("""", ""))
        dto.PezziScartati = Integer.Parse(dati(3).Trim().Replace("""", ""))
        dto.PezziCartone = Integer.Parse(dati(4).Trim().Replace("""", ""))
        dto.CartoniBuoni = Integer.Parse(dati(5).Trim().Replace("""", ""))

        ' Note diagnostiche
        dto.Note =
            settings.AxomaticNomeMacchina & Environment.NewLine &
            "HEADER RAW: " & righe(0).Trim() & Environment.NewLine &
            "DATI RAW: " & righe(1).Trim()

        Return dto
    End Function



    Public Function ParseProduzioneCsv(percorsoFile As String, settings As Settings) As AxomaticCsvDto

        Dim NomeFile As String = Path.GetFileName(percorsoFile)
        ' Lettura righe
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        'If righe.Length <> 2 Then
        '    Throw New InvalidDataException($"Il file {NomeFile} deve contenere una riga di intestazione e una sola riga di dati.")
        'End If

        'Intestazioni previste
        Dim intestazioniAttese As String() = {
            """Data_Ora""", """NomeRicetta""", """PezziBuoni""", """PezziScartati""", """Pezzicartone""", """CartoniBuoni"""
        }

        Dim header() As String = righe(0).Split(","c)
        If header.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException($"Numero di colonne nel file {NomeFile} non corrispondente.")
        End If

        For i As Integer = 0 To intestazioniAttese.Length - 1
            If Not header(i).Trim().Equals(intestazioniAttese(i), StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidDataException($"Colonna {i + 1} non valida nel file {NomeFile}: atteso '{intestazioniAttese(i)}', trovato '{header(i)}'")
            End If
        Next

        'Parsing della riga dati
        Dim dati() As String = righe(1).Split(","c)
        If dati.Length <> intestazioniAttese.Length Then
            Throw New InvalidDataException($"La riga dati nel file {NomeFile} non contiene il numero corretto di colonne.")
        End If

        'Popolamento DTO
        Dim dto As New AxomaticCsvDto()
        Dim culture As CultureInfo = CultureInfo.InvariantCulture

        dto.Data_Ora = DateTime.Parse(General.ConvertiDataCustom(dati(0).Trim().Replace("""", ""), GlobalConstants.MACHINENAME_AXOMATIC), New CultureInfo("it-IT"))
        dto.NomeRicetta = dati(1).Trim().Replace("""", "")
        dto.PezziBuoni = Integer.Parse(dati(2).Trim().Replace("""", ""))
        dto.PezziScartati = Integer.Parse(dati(3).Trim().Replace("""", ""))
        dto.PezziCartone = Integer.Parse(dati(4).Trim().Replace("""", ""))
        dto.CartoniBuoni = Integer.Parse(dati(5).Trim().Replace("""", ""))

        dto.Note = settings.AxomaticNomeMacchina & Environment.NewLine & righe(0).Trim().Replace("""", "") & Environment.NewLine & righe(1).Trim().Replace("""", "")
        Return dto
    End Function

End Module
