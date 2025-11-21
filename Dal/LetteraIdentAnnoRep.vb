
Public Class LetteraIdentAnnoRep

    Private ReadOnly _connString As String

    Public Sub New(connString As String)
        _connString = connString
    End Sub

    Public Function GetLetteraIdentAnno(Data As DateTime) As String
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



#Region "Private routine"


#End Region

End Class


