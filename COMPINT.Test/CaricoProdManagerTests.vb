Imports Moq
Imports NTSInformatica

<TestClass>
Public Class CaricoProdManagerTests

    Private Settings As Settings
    Private FakeCleBoll As Mock(Of CLEVEBOLL)
    Private Manager As MovimentazioneManager

    <TestInitialize>
    Public Sub Setup()
        ' Inizializzazione comune
        Me.Settings = New Settings()
        FakeCleBoll = New Mock(Of CLEVEBOLL)()
        manager = New MovimentazioneManager(Settings, fakeCleBoll.Object)
    End Sub

    '  TEST CASI VALIDI
    <DataTestMethod>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_MECCANOPLASTICA1, "M123", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_MECCANOPLASTICA4, "M999", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_MECCANOPLASTICA1, "5123", "/P")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_MECCANOPLASTICA4, "5123", "/P")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI, "M123", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI2, "M999", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI, "FAA001", "CON")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI2, "FAA999", "CON")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI, "P555", "PAG")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI2, "P777", "PAG")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_AXOMATIC, "M123", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_ETICH, "M123", "ET")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_ETICH, "C001", "ET")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_ETICH, "P999", "ET")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_LAY, "2123", "LY")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23017, "5123", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23017, "M001", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23017, "P777", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23018, "5123", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23018, "C001", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23018, "M001", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PICKER23018, "P777", "FL")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PRONTOWASH1, "M123", "PB")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_PRONTOWASH2, "M555", "PB")>
    Public Sub GetSerie_ValidCases(macchina As String, codart As String, expected As String)
        'Dim settings As New Settings()
        'Dim fakeCleBoll As New Mock(Of CLEVEBOLL)
        'Dim manager As New MovimentazioneManager(settings, fakeCleBoll.Object)

        Dim result = manager.GetSerie(macchina, codart)

        Assert.AreEqual(expected, result)

    End Sub

    '  TEST CASI CHE DEVONO FARE THROW
    <DataTestMethod>
    <ExpectedException(GetType(Exception))>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI, "F12")>
    <DataRow(COMPINT.GlobalConstants.MACHINENAME_DUETTI2, "FBB99")>
    <DataRow("MACHINA_X", "M123")>
    Public Sub GetSerie_InvalidCases(macchina As String, codart As String)
        'Dim settings As New Settings()
        'Dim fakeCleBoll As New Mock(Of CLEVEBOLL)
        'Dim manager As New MovimentazioneManager(settings, fakeCleBoll.Object)

        Dim result = manager.GetSerie(macchina, codart)

    End Sub

End Class

