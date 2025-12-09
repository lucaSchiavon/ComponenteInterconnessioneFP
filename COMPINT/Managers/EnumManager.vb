Public NotInheritable Class EnumManager(Of T As {Structure})

    Private Sub New()
        ' Classe statica/utility, impedisce istanziazione
    End Sub

    ''' <summary>
    ''' Converte una stringa in un valore Enum del tipo T.
    ''' </summary>
    Public Shared Function Parse(value As String, Optional defaultValue As T = Nothing) As T
        Dim result As T

        If [Enum].TryParse(value, True, result) Then
            Return result
        Else
            Return defaultValue
        End If
    End Function

    ''' <summary>
    ''' Converte un valore Enum T in stringa.
    ''' </summary>
    Public Shared Function EnumToString(value As T) As String
        Return [Enum].GetName(GetType(T), value)
    End Function

End Class

