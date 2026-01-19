
Public Class GiornoProdConterRep
    Implements IGiornoProdConterRep

    Private ReadOnly _connString As String
    Private ReadOnly _settings As Settings

    Public Sub New(connString As String, settings As Settings)
        _connString = connString
        _settings = settings
    End Sub

    Public Function GetNextProdCounter(DataProduzione As DateTime) As String Implements IGiornoProdConterRep.GetNextProdCounter
        'Torna la prossima numerazione di produzione utile
        'si assume che non si possa inserire una produzione con data inferiore all'ultima produzione ma sempre successiva
        Dim AnnoDiProduzione As Integer = DataProduzione.Year
        Dim adoLayer As New AdoDataLayer(_connString)
        adoLayer.ClearParameters()
        adoLayer.AddOrReplaceParameter("@DataProduzione", AnnoDiProduzione, SqlDbType.NVarChar)

        Dim query As String = "SELECT DataProduzione,IncrementaleGDiProdConter
                               FROM TblGiornoProdConter 
                               WHERE year(DataProduzione) = @DataProduzione order by DataProduzione desc"

        Dim Ds As DataSet = adoLayer.GetDataSet(query)
        Dim DtRes As DataTable = Ds.Tables(0)

        If DtRes.Rows.Count > 0 Then
            ' Prendo l'ultima data di produzione dal DB
            Dim ultimaDataProduzione As DateTime = Convert.ToDateTime(DtRes.Rows(0)("DataProduzione"))
            ' Confronto solo la parte di data (senza ora)
            If ultimaDataProduzione.Date <> DataProduzione.Date Then
                ' Se la data è diversa, incremento
                Return (Convert.ToInt32(DtRes.Rows(0)("IncrementaleGDiProdConter")) + 1).ToString("0000")
            Else
                ' Se la data è uguale, ritorno il contatore così com'è
                Return Convert.ToInt32(DtRes.Rows(0)("IncrementaleGDiProdConter")).ToString("0000")
            End If

        Else
            'ritorna il prossimo giorno di produzione incrementando di uno l'ultimo
            Return 1.ToString("0000") 'GetNextProdCounterByYear(DataProduzione.Year)
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

    Public Sub InsertIncrementaleGDiProd(IncrementaleGDiProdConter As Int32, DataDiProduzionePerLottoConter As DateTime) Implements IGiornoProdConterRep.InsertIncrementaleGDiProd
        Dim adoLayer As New AdoDataLayer(_connString)

        adoLayer.ClearParameters()

        'adoLayer.AddOrReplaceParameter("@DataProduzione", Now, SqlDbType.DateTime)
        adoLayer.AddOrReplaceParameter("@DataProduzione", DataDiProduzionePerLottoConter, SqlDbType.DateTime)
        adoLayer.AddOrReplaceParameter("@IncrementaleGDiProdConter", IncrementaleGDiProdConter, SqlDbType.Int)


        Dim insertQuery As String = "INSERT INTO TblGiornoProdConter ([DataProduzione],[IncrementaleGDiProdConter]) " &
                                    "VALUES ( @DataProduzione, @IncrementaleGDiProdConter)"

        Dim rowsInserted As Integer = adoLayer.ExecuteNonQuery(insertQuery)
    End Sub

#End Region

End Class


