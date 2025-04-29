Imports System.IO
Imports System.Text.Json

Namespace Utils
    Public Class SettingsManager

        Public Class Config
            Public Property Username As String
            Public Property EncryptedRootKey As String
            Public Property StoragePath As String
            Public Property Theme As String = "Light"
        End Class

        Private Const ConfigFileName As String = "config.json"

        Public Shared Function ConfigExists(basePath As String) As Boolean
            Dim fullPath As String = Path.Combine(basePath, ConfigFileName)
            Return File.Exists(fullPath)
        End Function

        Public Shared Sub SaveConfig(cfg As Config, basePath As String)
            Dim fullPath As String = Path.Combine(basePath, ConfigFileName)
            Dim options As New JsonSerializerOptions With {.WriteIndented = True}
            Dim json As String = JsonSerializer.Serialize(Of Config)(cfg, options)
            File.WriteAllText(fullPath, json)
        End Sub

        Public Shared Function LoadConfig(basePath As String) As Config
            Dim fullPath As String = Path.Combine(basePath, ConfigFileName)
            If Not File.Exists(fullPath) Then
                Throw New FileNotFoundException($"Config not found: {fullPath}")
            End If
            Dim json As String = File.ReadAllText(fullPath)
            Return JsonSerializer.Deserialize(Of Config)(json)
        End Function

        ' TODO: Add ChangePassword and ExportKey methods
    End Class
End Namespace