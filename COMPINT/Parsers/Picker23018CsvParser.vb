Imports System.Globalization
Imports System.IO

Module Picker23018CsvParser
    Public Function ParseProduzioneCsv(percorsoFile As String, settings As Settings) As Picker23018CsvDto

        Dim NomeFile As String = Path.GetFileName(percorsoFile)
        ' Lettura righe
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        'If righe.Length <> 2 Then
        '    Throw New InvalidDataException($"Il file {NomeFile} deve contenere una riga di intestazione e una sola riga di dati.")
        'End If

        'Intestazioni previste
        'Dim intestazioniAttese As String() = {
        '    "DATA E ORA", "RICETTA", "PEZZI BUONI", "PEZZI SCARTO", " "
        '}

        'Dim header() As String = righe(0).Split(";"c)
        'If header.Length <> intestazioniAttese.Length Then
        '    Throw New InvalidDataException($"Numero di colonne nel file {NomeFile} non corrispondente.")
        'End If

        'non si può fare il controllo qui sotto per Picker23017Csv, il file è una porcheria con un sacco di campi in più
        'rispetto ad intestazioni

        'For i As Integer = 0 To intestazioniAttese.Length - 1
        '    If Not header(i).Trim().Equals(intestazioniAttese(i), StringComparison.OrdinalIgnoreCase) Then
        '        Throw New InvalidDataException($"Colonna {i + 1} non valida nel file {NomeFile}: atteso '{intestazioniAttese(i)}', trovato '{header(i)}'")
        '    End If
        'Next

        ''Parsing della riga dati
        Dim dati() As String = righe(1).Split(";"c)
        'If dati.Length <> intestazioniAttese.Length Then
        '    Throw New InvalidDataException($"La riga dati nel file {NomeFile} non contiene il numero corretto di colonne.")
        'End If
        If Integer.Parse(dati(2)) = 0 Then
            'se non vi è stata produzione di alcun prodotto sposta in errore
            Throw New InvalidDataException($"Il file {Path.GetFileName(percorsoFile)} presenta una produzione pari a 0 nel campo PezziBuoni")
        End If
        'Popolamento DTO
        Dim dto As New Picker23018CsvDto()
        Dim culture As CultureInfo = CultureInfo.InvariantCulture

        dto.DataEOra = DateTime.Parse(General.ConvertiDataCustom(dati(0), GlobalConstants.MACHINENAME_PICKER23018), New CultureInfo("it-IT"))
        dto.Ricetta = dati(1).Trim()
        dto.PezziBuoni = Integer.Parse(dati(2))
        dto.PezziScarto = Integer.Parse(dati(3))
        dto.Note = settings.LayNomeMacchina & Environment.NewLine & righe(0) & Environment.NewLine & righe(1)
        Return dto
    End Function



End Module
