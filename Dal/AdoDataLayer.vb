Imports System
Imports System.Collections
Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient


Public Class AdoDataLayer


    Implements IDisposable

    Private _connectionString As String = String.Empty
    Private _parameterList As Hashtable = New Hashtable()
    Private _connection As SqlConnection
    Private _dr As DbDataReader
    Private disposedValue As Boolean = False

    Public Sub New(connectionString As String)
        ClearParameters()
        Dim connectionStringMaster As String = connectionString

        If TestConnectionString(connectionStringMaster) Then
            _connectionString = connectionStringMaster
        Else
            Throw New Exception("Invalid Connection String, can not connect to database")
        End If
    End Sub

    Private Sub OpenConnection(connection As SqlConnection)
        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If
    End Sub

    Public Sub CloseConnection(Optional connection As SqlConnection = Nothing)
        If _dr IsNot Nothing AndAlso Not _dr.IsClosed Then
            _dr.Close()
        End If

        If connection Is Nothing Then
            connection = _connection
        End If

        If connection IsNot Nothing Then
            If connection.State = ConnectionState.Open OrElse connection.State = ConnectionState.Broken Then
                connection.Close()
            End If
            connection.Dispose()
        End If
    End Sub

    Public Function TestConnectionString(connectionString As String) As Boolean
        Using connection As New SqlConnection(connectionString)
            Try
                OpenConnection(connection)
                Return True
            Catch ex As Exception
                Return False
            Finally
                CloseConnection(connection)
            End Try
        End Using
    End Function

    Public Sub ClearParameters()
        _parameterList.Clear()
    End Sub

    Public Sub AddOrReplaceParameter(name As String, value As Object, Optional sqlDbType As SqlDbType = SqlDbType.NVarChar, Optional direction As ParameterDirection = ParameterDirection.Input)
        Dim sqlParameter As New SqlParameter(name, value) With {
                .direction = direction,
                .sqlDbType = sqlDbType
            }

        Select Case sqlParameter.SqlDbType
            Case SqlDbType.Char, SqlDbType.NChar, SqlDbType.NVarChar, SqlDbType.VarChar
                sqlParameter.Size = -1
        End Select

        If Not _parameterList.ContainsKey(name) Then
            _parameterList.Add(name, sqlParameter)
        Else
            _parameterList(name) = sqlParameter
        End If
    End Sub

    Public Function GetParameterValue(name As String) As Object
        If _parameterList.ContainsKey(name) Then
            Return CType(_parameterList(name), SqlParameter).Value
        Else
            Return Nothing
        End If
    End Function

    Public Function ExecuteNonQuery(queryString As String, Optional commandType As CommandType = CommandType.Text) As Integer
        If Not String.IsNullOrEmpty(_connectionString) AndAlso Not String.IsNullOrEmpty(queryString) Then
            Using connection As New SqlConnection(_connectionString)
                Dim command As New SqlCommand(queryString, connection) With {
                        .commandType = commandType
                    }

                For Each de As DictionaryEntry In _parameterList
                    command.Parameters.Add(CType(de.Value, SqlParameter))
                Next

                OpenConnection(connection)
                Dim rowCount As Integer = command.ExecuteNonQuery()
                command.Parameters.Clear()
                command.Dispose()
                Return rowCount
            End Using
        Else
            Return 0
        End If
    End Function

    Public Function GetScalar(queryString As String, Optional commandType As CommandType = CommandType.Text) As Object
        If Not String.IsNullOrEmpty(_connectionString) AndAlso Not String.IsNullOrEmpty(queryString) Then
            Using connection As New SqlConnection(_connectionString)
                Dim command As New SqlCommand(queryString, connection) With {
                        .commandType = commandType
                    }

                For Each de As DictionaryEntry In _parameterList
                    command.Parameters.Add(CType(de.Value, SqlParameter))
                Next

                OpenConnection(connection)
                Dim returnValue As Object = command.ExecuteScalar()
                command.Parameters.Clear()
                command.Dispose()
                Return returnValue
            End Using
        Else
            Return Nothing
        End If
    End Function

    Public Function GetDataSet(queryString As String, Optional commandType As CommandType = CommandType.Text) As DataSet
        If Not String.IsNullOrEmpty(_connectionString) AndAlso Not String.IsNullOrEmpty(queryString) Then
            Using connection As New SqlConnection(_connectionString)
                Dim command As New SqlCommand(queryString, connection) With {
                        .commandType = commandType
                    }

                For Each de As DictionaryEntry In _parameterList
                    command.Parameters.Add(CType(de.Value, SqlParameter))
                Next

                Dim adapter As New SqlDataAdapter(command)
                Dim dataset As New DataSet()
                adapter.Fill(dataset)
                command.Parameters.Clear()
                command.Dispose()
                adapter.Dispose()
                Return dataset
            End Using
        Else
            Return Nothing
        End If
    End Function

    Public Function GetDataReader(queryString As String, Optional commandType As CommandType = CommandType.Text) As DbDataReader
        If Not String.IsNullOrEmpty(_connectionString) AndAlso Not String.IsNullOrEmpty(queryString) Then
            _connection = New SqlConnection(_connectionString)
            Dim command As New SqlCommand(queryString, _connection) With {
                    .CommandTimeout = 1800,
                    .commandType = commandType
                }

            For Each de As DictionaryEntry In _parameterList
                command.Parameters.Add(CType(de.Value, SqlParameter))
            Next

            OpenConnection(_connection)
            _dr = command.ExecuteReader()
            command.Parameters.Clear()
            command.Dispose()
            Return _dr
        Else
            Return Nothing
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                CloseConnection()
                _parameterList.Clear()
                _parameterList = Nothing
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class



