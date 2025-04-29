Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports MoodHavenJournal_Core.Models

Namespace Services
    Public Class UserProfileService
        Private Shared ReadOnly ProfilePath As String =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoodBloom", "userprofile.dat")

        ''' <summary>
        ''' Loads and decrypts the user profile from disk using the provided encryption service and root key.
        ''' </summary>
        Public Shared Function LoadProfile(encryptionService As EncryptionService, rootKey As Byte()) As UserProfile
            If Not File.Exists(ProfilePath) Then Return Nothing
            Try
                Dim encryptedData As Byte() = File.ReadAllBytes(ProfilePath)
                Dim jsonBytes As Byte() = encryptionService.Decrypt(encryptedData, rootKey)
                Dim json As String = Encoding.UTF8.GetString(jsonBytes)
                Return JsonSerializer.Deserialize(Of UserProfile)(json)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Encrypts and saves the user profile to disk using the provided encryption service and root key.
        ''' </summary>
        Public Shared Sub SaveProfile(profile As UserProfile, encryptionService As EncryptionService, rootKey As Byte())
            Dim json As String = JsonSerializer.Serialize(profile, New JsonSerializerOptions With {.WriteIndented = True})
            Dim jsonBytes As Byte() = Encoding.UTF8.GetBytes(json)
            Dim encryptedData As Byte() = encryptionService.Encrypt(jsonBytes, rootKey)
            Directory.CreateDirectory(Path.GetDirectoryName(ProfilePath))
            File.WriteAllBytes(ProfilePath, encryptedData)
        End Sub

        ''' <summary>
        ''' Checks if the encrypted user profile exists.
        ''' </summary>
        Public Shared Function ProfileExists() As Boolean
            Return File.Exists(ProfilePath)
        End Function
    End Class
End Namespace
