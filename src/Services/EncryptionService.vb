' EncryptionService.vb
Imports System.Security.Cryptography
Imports System.Text

Namespace Services
    Public Class EncryptionService

        Public Function DeriveKey(password As String, salt As Byte(), iterations As Integer) As Byte()
            If String.IsNullOrEmpty(password) Then Throw New ArgumentException("Password cannot be empty.")
            If salt Is Nothing OrElse salt.Length = 0 Then Throw New ArgumentException("Salt must be provided.")
            If iterations <= 0 Then Throw New ArgumentException("Iterations must be a positive number.")
            Dim pbkdf2 As New Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)
            Return pbkdf2.GetBytes(32)
        End Function

        Public Function Encrypt(data As Byte(), key As Byte()) As Byte()
            If data Is Nothing OrElse data.Length = 0 Then Throw New ArgumentException("No data to encrypt.")
            If key Is Nothing OrElse key.Length <> 32 Then Throw New ArgumentException("Key must be 256 bits (32 bytes).")
            Using aes As Aes = Aes.Create()
                aes.KeySize = 256
                aes.Key = key
                aes.Mode = CipherMode.CBC
                aes.Padding = PaddingMode.PKCS7
                aes.GenerateIV()
                Using ms As New IO.MemoryStream()
                    ms.Write(aes.IV, 0, aes.IV.Length)
                    Using cs As New CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)
                        cs.Write(data, 0, data.Length)
                        cs.FlushFinalBlock()
                    End Using
                    Return ms.ToArray()
                End Using
            End Using
        End Function

        Public Function Decrypt(encrypted As Byte(), key As Byte()) As Byte()
            If encrypted Is Nothing OrElse encrypted.Length <= 16 Then
                Throw New ArgumentException("Encrypted data is invalid or too short.")
            End If
            If key Is Nothing OrElse key.Length <> 32 Then
                Throw New ArgumentException("Key must be 256 bits (32 bytes).")
            End If
            Dim iv(15) As Byte
            Array.Copy(encrypted, 0, iv, 0, iv.Length)
            Using aes As Aes = Aes.Create()
                aes.KeySize = 256
                aes.Key = key
                aes.IV = iv
                aes.Mode = CipherMode.CBC
                aes.Padding = PaddingMode.PKCS7
                Try
                    Using ms As New IO.MemoryStream()
                        Using cs As New CryptoStream(New IO.MemoryStream(encrypted, 16, encrypted.Length - 16), aes.CreateDecryptor(), CryptoStreamMode.Read)
                            cs.CopyTo(ms)
                        End Using
                        Return ms.ToArray()
                    End Using
                Catch ex As CryptographicException
                    Throw New CryptographicException("Decryption failed. The password may be incorrect, or the data may be corrupted.", ex)
                End Try
            End Using
        End Function

    End Class
End Namespace
