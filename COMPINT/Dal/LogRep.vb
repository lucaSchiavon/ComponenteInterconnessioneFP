
Public Class LogRep

    Private ReadOnly _connString As String

    Public Sub New(connString As String)
        _connString = connString
    End Sub

    Public Sub InsertLog(livello As String, messaggio As String, stackTrace As String, macchina As String, codArt As String, fileName As String)
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
    End Sub


End Class


