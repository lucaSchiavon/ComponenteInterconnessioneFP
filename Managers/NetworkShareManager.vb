Imports System
Imports System.IO
Imports System.Runtime.InteropServices

Public Class NetworkShareManager
    <StructLayout(LayoutKind.Sequential)>
    Private Structure NETRESOURCE
        Public dwScope As Integer
        Public dwType As Integer
        Public dwDisplayType As Integer
        Public dwUsage As Integer
        <MarshalAs(UnmanagedType.LPWStr)> Public lpLocalName As String
        <MarshalAs(UnmanagedType.LPWStr)> Public lpRemoteName As String
        <MarshalAs(UnmanagedType.LPWStr)> Public lpComment As String
        <MarshalAs(UnmanagedType.LPWStr)> Public lpProvider As String
    End Structure

    <DllImport("mpr.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function WNetAddConnection2(ByRef netResource As NETRESOURCE, ByVal password As String, ByVal username As String, ByVal flags As Integer) As Integer
    End Function

    <DllImport("mpr.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function WNetCancelConnection2(ByVal name As String, ByVal flags As Integer, ByVal force As Boolean) As Integer
    End Function

    Private Const RESOURCETYPE_DISK As Integer = 1

    'Public Shared Function ConnectToShare(remoteUNC As String, username As String, password As String) As Integer
    '    Dim nr As New NETRESOURCE
    '    nr.dwType = RESOURCETYPE_DISK
    '    nr.lpRemoteName = remoteUNC
    '    nr.lpLocalName = Nothing
    '    Return WNetAddConnection2(nr, password, username, 0)
    'End Function

    Public Shared Sub ConnectToShare(remoteUNC As String, username As String, password As String)
        Dim ret As Integer
        Dim nr As New NETRESOURCE
        nr.dwType = RESOURCETYPE_DISK
        nr.lpRemoteName = remoteUNC
        nr.lpLocalName = Nothing
        ret = WNetAddConnection2(nr, password, username, 0)
        If ret <> 0 Then
            Throw New Exception($"Apertura della cartella{remoteUNC} fallita, utilizzando l'utente {username} probabilmente il nome utente o password non sono corretti")
        End If
    End Sub

    Public Shared Function DisconnectShare(UNC As String) As Integer
        Return WNetCancelConnection2(UNC, 0, True)
    End Function
End Class
