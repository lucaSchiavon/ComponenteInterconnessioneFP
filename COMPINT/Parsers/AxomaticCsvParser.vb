Imports System.Globalization
Imports System.IO

Module AxomaticCsvParser
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
