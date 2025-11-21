Public Class CaricoProdManager

    'Private ReadOnly _settings As Settings
    'Private ReadOnly _logRep As LogRep

    'Public Sub New(settings As Settings)
    Public Sub New()
        '_settings = settings
        '_logRep = New LogRep(settings.ConnStr)
    End Sub

    Public Function GetSerie(Macchina As String, CodArt As String) As String

        Dim PrimoCar As Char = CodArt(0)
        Dim Serie As String = ""
        Select Case Macchina
            Case GlobalConstants.MACHINENAME_MECCANOPLASTICA1, GlobalConstants.MACHINENAME_MECCANOPLASTICA4
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                    Case "5"c
                        Serie = "/P"
                End Select
            Case GlobalConstants.MACHINENAME_DUETTI, GlobalConstants.MACHINENAME_DUETTI2
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                    Case "F"c
                        If CodArt.Length >= 3 AndAlso CodArt.Substring(0, 3) = "FAA" Then
                            Serie = "CON"
                        Else
                            Throw New Exception($"Articolo {CodArt} processato dalla macchina {Macchina} non riconosciuto per ricavarne la serie")
                        End If
                    Case "P"c
                        Serie = "PAG"
                End Select
            Case GlobalConstants.MACHINENAME_AXOMATIC
                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                End Select
            Case GlobalConstants.MACHINENAME_ETICH
                Select Case PrimoCar
                    Case "M"c, "C"c, "P"c
                        Serie = "ET"
                End Select
            Case GlobalConstants.MACHINENAME_LAY
                Select Case PrimoCar
                    Case "2"c
                        Serie = "LY"
                End Select
            Case GlobalConstants.MACHINENAME_PICKER2317
                Select Case PrimoCar
                    Case "5"c, "M"c, "P"c
                        Serie = "FL"
                End Select
            Case GlobalConstants.MACHINENAME_PICKER2318
                Select Case PrimoCar
                    Case "5"c, "C"c, "M"c, "P"c
                        Serie = "FL"
                End Select
            Case GlobalConstants.MACHINENAME_PRONTOWASH1, GlobalConstants.MACHINENAME_PRONTOWASH2

                Select Case PrimoCar
                    Case "M"c
                        Serie = "PB"
                End Select
            Case Else
                Throw New Exception($"Articolo {CodArt} processato dalla macchina {Macchina} non riconosciuto per ricavarne la serie")
        End Select

        Return Serie

    End Function


End Class
