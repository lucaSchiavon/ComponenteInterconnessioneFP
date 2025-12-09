Imports System.IO


<TestClass>
Public Class ICAVL08616CsvParserTests

    Private settings As Settings

    <TestInitialize>
    Public Sub TestInitialize()
        ' Inizializzazione comune a tutti i test
        Settings = New Settings()
        settings.ICAVL08616NomeMacchina = "TestMachine"
    End Sub

    Private Function CreateTempCsvFile(content As String) As String
        Dim tempFile As String = Path.GetTempFileName()
        File.WriteAllText(tempFile, content, System.Text.Encoding.UTF8)
        Return tempFile
    End Function

    <TestMethod>
    <ExpectedException(GetType(InvalidDataException))>
    Public Sub ParseProduzioneCsv_ShouldThrow_WhenScatoleTeoricheProdotteIsZero()
        ' Arrange: contenuto CSV con Scatole teoriche prodotte = 0
        Dim csvContent As String =
"Inizio turno;Fine turno;Codice articolo;Doypack per scatole;Tabs per doypack;Velocità macchina;Scarichi effettuati dosatore 1;Scarichi effettuati dosatore 2;Scarichi effettuati dosatore 3;Scarichi effettuati dosatore 4;Scarichi effettuati dosatore 5;Scarichi effettuati dosatore 6;Numero ordine;Quantità in ordine;Ricetta attiva;Cialde espulse;Passi eseguiti dal tamburo;Passi eseguiti traino;Passi eseguiti traino 2;Carta svolta;Cialde accettate;Cialde riempite;Doypack teorici prodotti;Scatole teoriche prodotte" & Environment.NewLine &
"26/11/2025 15:04:00;27/11/2025 06:05:00;""M74408"";6;12;40;0;0;0;0;0;0;"";0;""FOSSE"";0;0;0;0;0;0;0;0;0"

        Dim tempFile = CreateTempCsvFile(csvContent)

        ' Act: chiamata al parser
        Dim result = ICAVL08616CsvParser.ParseProduzioneCsv(tempFile, settings)

        ' Assert: gestito da ExpectedException
    End Sub

    <TestMethod>
    Public Sub ParseProduzioneCsv_ShouldReturnDto_WhenCsvIsValid()
        ' Arrange: contenuto CSV con Scatole teoriche prodotte = 32
        Dim csvContent As String =
"Inizio turno;Fine turno;Codice articolo;Doypack per scatole;Tabs per doypack;Velocità macchina;Scarichi effettuati dosatore 1;Scarichi effettuati dosatore 2;Scarichi effettuati dosatore 3;Scarichi effettuati dosatore 4;Scarichi effettuati dosatore 5;Scarichi effettuati dosatore 6;Numero ordine;Quantità in ordine;Ricetta attiva;Cialde espulse;Passi eseguiti dal tamburo;Passi eseguiti traino;Passi eseguiti traino 2;Carta svolta;Cialde accettate;Cialde riempite;Doypack teorici prodotti;Scatole teoriche prodotte" & Environment.NewLine &
"26/11/2025 15:04:00;27/11/2025 06:05:00;""M74408"";6;12;40;0;0;0;0;0;0;"";0;""FOSSE"";0;0;0;0;0;0;0;0;32"

        Dim tempFile = CreateTempCsvFile(csvContent)

        ' Act
        Dim dto = ICAVL08616CsvParser.ParseProduzioneCsv(tempFile, settings)

        ' Assert: controlliamo alcuni campi chiave
        Assert.IsNotNull(dto)
        Assert.AreEqual("M74408", dto.CodiceArticolo)
        Assert.AreEqual(6, dto.DoypackPerScatola)
        Assert.AreEqual(12, dto.TabsPerDoypack)
        Assert.AreEqual(40, dto.VelocitaMacchina)
        Assert.AreEqual("FOSSE", dto.RicettaAttiva)
        Assert.AreEqual(32, dto.ScatoleTeoricheProdotte)

        ' Verifica date parsing
        Assert.AreEqual(New DateTime(2025, 11, 26, 15, 4, 0), dto.InizioTurno)
        Assert.AreEqual(New DateTime(2025, 11, 27, 6, 5, 0), dto.FineTurno)
    End Sub

End Class

