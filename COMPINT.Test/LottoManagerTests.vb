Imports FPCOMPINTERCONN
Imports Moq

<TestClass>
Public Class LottoManagerTests

    Private Settings As Settings
    Private MockLettera As Mock(Of ILetteraIdentAnnoRep)
    Private MockGiorno As Mock(Of IGiornoProdConterRep)
    Private MockLotto As Mock(Of ILottoRep)
    Private Manager As LottoManager

    <TestInitialize>
    Public Sub Setup()
        ' Inizializzazione comune
        Settings = New Settings()
        MockLettera = New Mock(Of ILetteraIdentAnnoRep)()
        MockGiorno = New Mock(Of IGiornoProdConterRep)()
        MockLotto = New Mock(Of ILottoRep)()
        Manager = New LottoManager(Settings, mockLettera.Object, mockGiorno.Object, mockLotto.Object)
    End Sub

    <TestMethod()>
    Public Sub GetNomeLotto_MacchinaMeccanoplastica1_RestituisceNonApplicato()
        ' Arrange
        'Dim settings As New Settings()
        Settings.NomeLottoPagCampoFissoFp = "FP"

        'Dim mockLettera = New Mock(Of ILetteraIdentAnnoRep)()
        'Dim mockGiorno = New Mock(Of IGiornoProdConterRep)()
        'Dim mockLotto = New Mock(Of ILottoRep)()

        'Dim manager As New LottoManager(settings, mockLettera.Object, mockGiorno.Object, mockLotto.Object)

        ' Act
        Dim result = manager.GetNomeLotto("P123", GlobalConstants.MACHINENAME_MECCANOPLASTICA1, DateTime.Now)

        ' Assert
        Assert.AreEqual(GlobalConstants.LOTTO_NONAPPLICATO, result)
    End Sub


    <DataTestMethod>
    <DataRow("2024-02-01", "32AFP")>
    <DataRow("2024-12-31", "366AFP")>
    <DataRow("2024-01-01", "1AFP")>
    <DataRow("2024-03-01", "61AFP")> 'anno bisestile, ci si aspetta 61esimo giorno
    <DataRow("2023-03-01", "60AFP")> 'anno normale, ci si aspetta 60
    Public Sub GetNomeLotto_ArticoloP_RestituisceLottoGiuliano(dataStr As String, expected As String)
        ' Arrange
        'Dim settings As New Settings()
        Settings.NomeLottoPagCampoFissoFp = "FP"

        'Dim mockLettera = New Mock(Of ILetteraIdentAnnoRep)()
        MockLettera.Setup(Function(m) m.GetLetteraIdentAnno(It.IsAny(Of DateTime))) _
                   .Returns("A")

        'Dim mockGiorno = New Mock(Of IGiornoProdConterRep)()
        'Dim mockLotto = New Mock(Of ILottoRep)()

        'Dim manager As New LottoManager(settings, mockLettera.Object, mockGiorno.Object, mockLotto.Object)

        Dim dataProd = Date.Parse(dataStr) ' 32º giorno giuliano

        ' Act
        Dim result = manager.GetNomeLotto("P555", GlobalConstants.MACHINENAME_DUETTI, dataProd)

        ' Assert
        Assert.AreEqual(expected, result)
    End Sub

    <TestMethod>
    Public Sub GetNomeLotto_ArticoloFAA_UsaConter()
        ' Arrange
        'Dim settings As New Settings()
        Settings.NumeroLineaDiRiempimento = "03"
        settings.LetteraIdentifFp = "F"

        'Dim mockLettera = New Mock(Of ILetteraIdentAnnoRep)()

        'Dim mockGiorno = New Mock(Of IGiornoProdConterRep)()
        MockGiorno.Setup(Function(m) m.GetNextProdCounter(It.IsAny(Of DateTime))) _
                  .Returns(1234)
        'Dim mockLotto = New Mock(Of ILottoRep)()

        'Dim manager As New LottoManager(settings, mockLettera.Object, mockGiorno.Object, mockLotto.Object)

        Dim dataProd = New DateTime(2024, 3, 10) 'year month day

        ' Act
        Dim result = manager.GetNomeLotto("FAA001", GlobalConstants.MACHINENAME_DUETTI, dataProd)

        ' Assert
        ' 24 = anno (yy)
        ' 10 = settimana ISO
        ' "0"   = domenica DayOfWeek (puoi adattare se vuoi numero)
        ' 03 = linea
        ' 1234 = progressivo
        ' F = lettera FP
        Assert.AreEqual("24100031234F", result)
    End Sub

    <TestMethod>
    <ExpectedException(GetType(Exception))>
    Public Sub GetNomeLotto_ArticoloNonValido_LanciaEccezione()
        ' Arrange
        'Dim settings As New Settings()

        'Dim mockLettera = New Mock(Of ILetteraIdentAnnoRep)()
        'Dim mockGiorno = New Mock(Of IGiornoProdConterRep)()
        'Dim mockLotto = New Mock(Of ILottoRep)()

        'Dim manager As New LottoManager(settings, mockLettera.Object, mockGiorno.Object, mockLotto.Object)

        ' Act
        Dim res = manager.GetNomeLotto("X999", GlobalConstants.MACHINENAME_DUETTI, DateTime.Now)
    End Sub

End Class