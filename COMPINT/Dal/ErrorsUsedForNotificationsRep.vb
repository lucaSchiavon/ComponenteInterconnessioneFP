Imports System.Data

Public Class ErrorsUsedForNotificationsRep

    Private ReadOnly _connString As String

    Public Sub New(connString As String)
        _connString = connString
    End Sub

    Public Function GetAll() As List(Of ErrorUsedForNotificationDto)
        Dim result As New List(Of ErrorUsedForNotificationDto)()

        Dim sql As String = "SELECT SubjectOfEmail, ErrorsToCheckForNotifications, Addresses, Sender FROM TblErrorsUsedForNotifications"

        Dim ado As New AdoDataLayer(_connString)
        Dim ds As DataSet = ado.GetDataSet(sql)

        If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
            For Each row As DataRow In ds.Tables(0).Rows
                Dim item As New ErrorUsedForNotificationDto() With {
                    .SubjectOfEmail = If(row.IsNull("SubjectOfEmail"), Nothing, row("SubjectOfEmail").ToString()),
                    .ErrorsToCheckForNotifications = If(row.IsNull("ErrorsToCheckForNotifications"), Nothing, row("ErrorsToCheckForNotifications").ToString()),
                    .Addresses = If(row.IsNull("Addresses"), Nothing, row("Addresses").ToString()),
                    .Sender = If(row.IsNull("Sender"), Nothing, row("Sender").ToString())
                }
                result.Add(item)
            Next
        End If

        Return result
    End Function
End Class
