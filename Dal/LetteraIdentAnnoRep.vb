
Public Class LetteraIdentAnnoRep
    Implements ILetteraIdentAnnoRep

    Private ReadOnly _connString As String

    Public Sub New(connString As String)
        _connString = connString
    End Sub

    Public Function GetLetteraIdentAnno(Data As DateTime) As String Implements ILetteraIdentAnnoRep.GetLetteraIdentAnno
        ' estrae anno dalla Data e recupera dalla tabella TblLetteraIdentAnnoPag la LetteraIdentAnno corrispondente:
        Dim anno As Integer = Data.Year
        Dim adoLayer As New AdoDataLayer(_connString)
        adoLayer.ClearParameters()
        adoLayer.AddOrReplaceParameter("@Anno", anno, SqlDbType.Int)

        Dim query As String = "SELECT LetteraIdentAnno 
                               FROM TblLetteraIdentAnnoPag 
                               WHERE Anno = @Anno"

        Dim obj As Object = adoLayer.GetScalar(query)

        If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
            Return obj.ToString().Trim()
        Else
            Throw New Exception($"Nessuna LetteraIdentAnno trovata per l'anno {anno}.")
        End If
    End Function

    'Private Function ILetteraIdentAnnoRep_GetLetteraIdentAnno(Data As Date) As String Implements ILetteraIdentAnnoRep.GetLetteraIdentAnno
    '    Throw New NotImplementedException()
    'End Function
End Class


