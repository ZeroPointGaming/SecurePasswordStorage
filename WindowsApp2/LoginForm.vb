Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class LoginForm
#Region "Important System Functions"
    ''' <summary>
    ''' Calculate the HASH value of a string using SHA256 encryption and validated it against the user input.
    ''' </summary>
    Public Function CalculateHash(bytes() As Byte)
        Dim DataArray() = bytes
        Dim HashAgent As New SHA256Managed()
        Dim Hash() = HashAgent.ComputeHash(DataArray)

        Dim HashString As String = String.Empty
        For Each bit In Hash
            HashString += String.Format("{0:x2}", bit)
        Next

        Return HashString.ToString()
    End Function
#End Region

#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As EncryptionWrapper()
    Private PasswordManager As New PasswordDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' Authenticate the user using the binary keyfile
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then MessageBox.Show("Please enter a valid password")

        Dim LoginAttempt = CalculateHash(Encoding.Unicode.GetBytes(TextBox1.Text))

        '''<summary>
        '''Hash the inputted passowrd and verify against the keyfile hash
        ''' </summary>
        If Not TextBox1.Text = "" Then
            Try
                If LoginAttempt = My.Settings.MainHash Then
                    Form1.Show()
                    Me.Close()
                Else
                    TextBox1.Text = ""
                    MessageBox.Show("Login Attempt Failed!")
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class