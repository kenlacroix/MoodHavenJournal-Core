Imports System.Text.Json.Serialization

Namespace Models
    Public Class UserProfile
        <JsonPropertyName("username")>
        Public Property Username As String

        <JsonPropertyName("salt")>
        Public Property Salt As String ' base64-encoded

        <JsonPropertyName("lastLogin")>
        Public Property LastLogin As DateTime

        <JsonPropertyName("theme")>
        Public Property Theme As String = "light" ' default
    End Class
End Namespace
