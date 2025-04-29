Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Security.Cryptography

Namespace Services
    Public Class SaltService
        Private Shared ReadOnly SaltPath As String =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MoodBloom", "salt.json")

        Public Class SaltData
            Public Property Version As String = "1"
            Public Property Salt As String
            Public Property KeyHash As String
        End Class

        ''' <summary>
        ''' Saves a new salt and derived key hash to salt.json.
        ''' </summary>
        Public Shared Sub SaveSalt(saltBytes As Byte(), derivedKey As Byte())
            Dim saltInfo As New SaltData With {
                .Salt = Convert.ToBase64String(saltBytes),
                .KeyHash = ComputeKeyHash(derivedKey)
            }
            Dim json As String = JsonSerializer.Serialize(saltInfo, New JsonSerializerOptions With {.WriteIndented = True})
            Directory.CreateDirectory(Path.GetDirectoryName(SaltPath))
            File.WriteAllText(SaltPath, json)
        End Sub

        ''' <summary>
        ''' Loads the stored salt data from salt.json.
        ''' </summary>
        Public Shared Function LoadSalt() As SaltData
            If Not File.Exists(SaltPath) Then Return Nothing
            Dim json As String = File.ReadAllText(SaltPath)
            Return JsonSerializer.Deserialize(Of SaltData)(json)
        End Function

        ''' <summary>
        ''' Verifies whether the given derived key matches the stored key hash.
        ''' </summary>
        Public Shared Function VerifyPassword(derivedKey As Byte()) As Boolean
            Dim saltInfo As SaltData = LoadSalt()
            If saltInfo Is Nothing Then Return False
            Dim actualHash As String = ComputeKeyHash(derivedKey)
            Return saltInfo.KeyHash = actualHash
        End Function

        ''' <summary>
        ''' Computes a base64-encoded SHA256 hash of the provided key.
        ''' </summary>
        Private Shared Function ComputeKeyHash(key As Byte()) As String
            Using sha As SHA256 = SHA256.Create()
                Dim hashBytes As Byte() = sha.ComputeHash(key)
                Return Convert.ToBase64String(hashBytes)
            End Using
        End Function

        ''' <summary>
        ''' Checks if salt.json exists.
        ''' </summary>
        Public Shared Function SaltExists() As Boolean
            Return File.Exists(SaltPath)
        End Function

        ''' <summary>
        ''' Deletes the salt.json file (used for reset flows).
        ''' </summary>
        Public Shared Sub DeleteSalt()
            If File.Exists(SaltPath) Then File.Delete(SaltPath)
        End Sub
    End Class
End Namespace
