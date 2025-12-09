Imports System.IO
Public Class IniParser
    Private keyPairs As New Hashtable()
    Private iniFilePath As String

    Private Structure SectionPair
        Public Section As String
        Public Key As String
    End Structure

    ''' <summary>
    ''' Opens the INI file at the given path and enumerates the values in the IniParser.
    ''' </summary>
    ''' <param name="iniPath">Full path to INI file.</param>
    Public Sub New(ByVal iniPath As String)
        Dim iniFile As TextReader = Nothing
        Dim strLine As String = Nothing
        Dim currentRoot As String = Nothing
        Dim keyPair() As String = Nothing

        iniFilePath = iniPath

        If File.Exists(iniPath) Then
            Try
                iniFile = New StreamReader(iniPath)
                strLine = iniFile.ReadLine()

                While strLine IsNot Nothing
                    strLine = strLine.Trim()

                    If strLine <> "" Then
                        If strLine.StartsWith("[") AndAlso strLine.EndsWith("]") Then
                            currentRoot = strLine.Substring(1, strLine.Length - 2)
                        Else
                            If strLine.StartsWith("'") Then
                                ' comment, ignore
                            Else
                                keyPair = strLine.Split(New Char() {"="c}, 2)

                                Dim sectionPair As SectionPair
                                Dim value As String = Nothing

                                If currentRoot Is Nothing Then currentRoot = "ROOT"

                                sectionPair.Section = currentRoot
                                sectionPair.Key = keyPair(0)

                                If keyPair.Length > 1 Then value = keyPair(1)

                                keyPairs.Add(sectionPair, value)
                            End If
                        End If
                    End If

                    strLine = iniFile.ReadLine()
                End While

            Catch ex As Exception
                Throw
            Finally
                If iniFile IsNot Nothing Then iniFile.Close()
            End Try
        Else
            Throw New FileNotFoundException("Impossibile trovare il file " & iniPath)
        End If
    End Sub

    ''' <summary>
    ''' Returns the value for the given section, key pair.
    ''' </summary>
    Public Function GetSetting(ByVal sectionName As String, ByVal settingName As String) As String
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName
        sectionPair.Key = settingName

        Return CType(keyPairs(sectionPair), String)
    End Function

    ''' <summary>
    ''' Enumerates all lines for given section.
    ''' </summary>
    Public Function EnumSection(ByVal sectionName As String) As String()
        Dim tmpArray As New ArrayList()

        For Each pair As SectionPair In keyPairs.Keys
            If pair.Section = sectionName Then
                tmpArray.Add(pair.Key)
            End If
        Next

        Return CType(tmpArray.ToArray(GetType(String)), String())
    End Function

    ''' <summary>
    ''' Adds or replaces a setting to the table to be saved.
    ''' </summary>
    Public Sub AddSetting(ByVal sectionName As String, ByVal settingName As String, ByVal settingValue As String)
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName
        sectionPair.Key = settingName

        If keyPairs.ContainsKey(sectionPair) Then
            keyPairs.Remove(sectionPair)
        End If

        keyPairs.Add(sectionPair, settingValue)
    End Sub

    ''' <summary>
    ''' Adds or replaces a setting with a null value.
    ''' </summary>
    Public Sub AddSetting(ByVal sectionName As String, ByVal settingName As String)
        AddSetting(sectionName, settingName, Nothing)
    End Sub

    ''' <summary>
    ''' Remove a setting.
    ''' </summary>
    Public Sub DeleteSetting(ByVal sectionName As String, ByVal settingName As String)
        Dim sectionPair As SectionPair
        sectionPair.Section = sectionName
        sectionPair.Key = settingName

        If keyPairs.ContainsKey(sectionPair) Then
            keyPairs.Remove(sectionPair)
        End If
    End Sub

    ''' <summary>
    ''' Save settings to new file.
    ''' </summary>
    Public Sub SaveSettings(ByVal newFilePath As String)
        Dim sections As New ArrayList()
        Dim tmpValue As String = ""
        Dim strToSave As String = ""

        For Each sectionPair As SectionPair In keyPairs.Keys
            If Not sections.Contains(sectionPair.Section) Then
                sections.Add(sectionPair.Section)
            End If
        Next

        For Each section As String In sections
            strToSave &= "[" & section & "]" & vbCrLf

            For Each sectionPair As SectionPair In keyPairs.Keys
                If sectionPair.Section = section Then
                    tmpValue = CType(keyPairs(sectionPair), String)

                    If tmpValue IsNot Nothing Then tmpValue = "=" & tmpValue

                    strToSave &= sectionPair.Key & tmpValue & vbCrLf
                End If
            Next

            strToSave &= vbCrLf
        Next

        Try
            Using tw As New StreamWriter(newFilePath)
                tw.Write(strToSave)
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Save settings back to ini file.
    ''' </summary>
    Public Sub SaveSettings()
        SaveSettings(iniFilePath)
    End Sub

End Class
