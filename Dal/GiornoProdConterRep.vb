
Public Class GiornoProdConterRep

    Private ReadOnly _connString As String
    Private ReadOnly _settings As Settings

    Public Sub New(connString As String, settings As Settings)
        _connString = connString
        _settings = settings
    End Sub

    Public Function GetNextProdCounter(DataProduzione As DateTime) As Int32
        'Torna la prossima numerazione di produzione utile
        'si assume che non si possa inserire una produzione con data inferiore all'ultima produzione ma sempre successiva
        Dim AnnoDiProduzione As Integer = DataProduzione.Year
        Dim adoLayer As New AdoDataLayer(_connString)
        adoLayer.ClearParameters()
        adoLayer.AddOrReplaceParameter("@DataProduzione", AnnoDiProduzione, SqlDbType.Date)

        Dim query As String = "SELECT *
                               FROM TblGiornoProdConter 
                               WHERE DataProduzione = @DataProduzione"

        Dim Ds As DataSet = adoLayer.GetDataSet(query)
        Dim DtRes As DataTable = Ds.Tables(0)

        If DtRes.Rows.Count > 0 Then
            Return Convert.ToInt32(DtRes.Rows("IncrementaleGDiProdConter"))
        Else
            'ritorna il prossimo giorno di produzione incrementando di uno l'ultimo
            Return GetNextProdCounterByYear(DataProduzione.Year)
        End If

    End Function

    Private Function GetNextProdCounterByYear(AnnoProduzione As Int32) As Int32
        'Torna la prossima numerazione di produzione utile
        Dim adoLayer As New AdoDataLayer(_connString)
        adoLayer.ClearParameters()
        adoLayer.AddOrReplaceParameter("@AnnoProduzione", AnnoProduzione, SqlDbType.Int)

        Dim query As String = "SELECT MAX(IncrementaleGDiProdConter) 
                               FROM TblGiornoProdConter 
                               WHERE year(DataProduzione) = @AnnoProduzione"

        Dim obj As Object = adoLayer.GetScalar(query)

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            Return CInt(obj) + 1
        Else
            'si tratta del primo giorno di produzione dell'anno corrente
            Return 1
        End If
    End Function
#Region "Private routine"
    'Private Function GetVerbosityLevel() As VerbosityLogLevel
    '    Return EnumManager(Of VerbosityLogLevel).Parse(_settings.VerbosityLogLevel, VerbosityLogLevel.Info)
    'End Function

    Private Sub InsertLog(livello As String, messaggio As String, stackTrace As String, macchina As String, codArt As String, fileName As String)
        Dim adoLayer As New AdoDataLayer(_connString)

        adoLayer.ClearParameters()

        adoLayer.AddOrReplaceParameter("@Livello", livello, SqlDbType.NVarChar)
        adoLayer.AddOrReplaceParameter("@Messaggio", messaggio, SqlDbType.NVarChar)
        adoLayer.AddOrReplaceParameter("@StackTrace", If(stackTrace, DBNull.Value), SqlDbType.NVarChar)
        adoLayer.AddOrReplaceParameter("@Macchina", If(macchina, DBNull.Value), SqlDbType.NVarChar)
        adoLayer.AddOrReplaceParameter("@CodArt", If(codArt, DBNull.Value), SqlDbType.NVarChar)
        adoLayer.AddOrReplaceParameter("@FileName", If(fileName, DBNull.Value), SqlDbType.NVarChar)

        Dim insertQuery As String = "INSERT INTO TblLog (Livello, Messaggio, StackTrace, Macchina, CodArt, FileName) " &
                                    "VALUES ( @Livello, @Messaggio, @StackTrace, @Macchina, @CodArt, @FileName)"

        Dim rowsInserted As Integer = adoLayer.ExecuteNonQuery(insertQuery)
        'Console.WriteLine($"Log {livello} inserito. Righe interessate: {rowsInserted}")
    End Sub
#End Region

End Class


