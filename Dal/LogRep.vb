
Public Class LogRep

    Private ReadOnly _connString As String

        Public Sub New(connString As String)
            _connString = connString
        End Sub

    Public Sub LogInfo(messaggio As String, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("INFO", messaggio, Nothing, macchina, codArt, fileName)
    End Sub

    Public Sub LogWarning(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("WARNING", messaggio, stackTrace, macchina, codArt, fileName)
    End Sub

    Public Sub LogError(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("ERROR", messaggio, stackTrace, macchina, codArt, fileName)
    End Sub

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
End Class


