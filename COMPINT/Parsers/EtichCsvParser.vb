Imports System.Globalization
Imports System.IO

Module EtichCsvParser
    Public Function ParseProduzioneCsv(percorsoFile As String, settings As Settings) As EtichCsvDto

        Dim NomeFile As String = Path.GetFileName(percorsoFile)
        ' Lettura righe
        Dim righe() As String = File.ReadAllLines(percorsoFile, System.Text.Encoding.UTF8)

        'If righe.Length <> 2 Then
        '    Throw New InvalidDataException($"Il file {NomeFile} deve contenere una riga di intestazione e una sola riga di dati.")
        'End If

        'Intestazioni previste
        Dim intestazioniAttese As String() = {
            """Data""", """Tempo""", """Nome_Ricetta""", """Nome Etichetta Gruppo 1""", """Etichette Erogate Gruppo 1""", """Nome Etichetta Gruppo 2""", """Etihchette Erogate Gruppo 2"""
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

        If Integer.Parse(dati(4)) = 0 And Integer.Parse(dati(6)) = 0 Then
            'se non vi è stata produzione di alcun prodotto sposta in errore
            Throw New InvalidDataException($"Il file {Path.GetFileName(percorsoFile)} presenta una produzione pari a 0 nel campo EtichetteErogateGruppo1 e nel campo EtichetteErogateGruppo2")
        End If

        'Popolamento DTO
        Dim dto As New EtichCsvDto()
        Dim culture As CultureInfo = CultureInfo.InvariantCulture

        dto.Data = DateTime.Parse(General.ConvertiDataCustom(dati(0).Trim().Replace("""", ""), GlobalConstants.MACHINENAME_ETICH), New CultureInfo("it-IT"))
        dto.Tempo = dati(1).Trim().Replace("""", "")
        dto.Nome_Ricetta = dati(2).Trim().Replace("""", "")
        dto.NomeEtichettaGruppo1 = dati(3).Trim().Replace("""", "")
        dto.EtichetteErogateGruppo1 = Integer.Parse(dati(4).Trim().Replace("""", ""))
        dto.NomeEtichettaGruppo2 = dati(5).Trim().Replace("""", "")
        dto.EtichetteErogateGruppo2 = Integer.Parse(dati(6).Trim().Replace("""", ""))
        dto.Note = settings.EtichNomeMacchina & Environment.NewLine & righe(0) & Environment.NewLine & righe(1)
        Return dto
    End Function



End Module
