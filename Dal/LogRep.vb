
Public Class LogRep

    Private ReadOnly _connString As String
    Private ReadOnly _settings As Settings

    Public Sub New(connString As String, settings As Settings)
        _connString = connString
        _settings = settings
    End Sub

    Public Sub LogInfo(messaggio As String, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("INFO", messaggio, Nothing, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogWarning(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("WARNING", messaggio, stackTrace, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub

    Public Sub LogError(messaggio As String, Optional stackTrace As String = Nothing, Optional macchina As String = Nothing, Optional codArt As String = Nothing, Optional fileName As String = Nothing)
        InsertLog("ERROR", messaggio, stackTrace, macchina, codArt, fileName)
        PrintConsoleMsg(messaggio, macchina, codArt, fileName)
    End Sub
    Private Sub PrintConsoleMsg(messaggio As String, macchina As String, codArt As String, fileName As String)

        'Dim levelEnum As VerbosityLogLevel = GetVerbosityLevel()

        'If levelEnum = VerbosityLogLevel.Debug Then
        If _settings.VerbosityLogLevel = VERBOSITYLOGLEVEL_DEBUG Then
            Console.WriteLine($"{_settings.VerbosityLogLevel} - {messaggio} {Environment.NewLine} Macchina: {macchina} CodArt: {codArt} Nome del file csv: {fileName}")
        End If

    End Sub

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


