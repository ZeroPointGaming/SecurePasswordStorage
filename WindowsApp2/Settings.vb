Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO

Public Class Settings
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private PasswordManager As New PasswordDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' Settings form onload event hanlder for loading the saved password database
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim BinarySerializerAgent As New BinaryFormatter()
            If File.Exists(Directory.GetCurrentDirectory + "\database.db") Then
                Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\database.db", FileMode.Open)

                PasswordManager.PasswordBroker = BinarySerializerAgent.Deserialize(BinaryStream)

                For Each entry In PasswordManager.PasswordBroker
                    ListBox1.Items.Add(entry.Key)
                Next

                BinaryStream.Close()
                BinaryStream.Dispose()
            End If
        Catch ex As Exception
            MessageBox.Show("Warning! Critical application error has occurred! Unable to load the password database." + Environment.NewLine + ex.ToString() + "Please submit a bug report on the github page with a screenshot of this error and any other information you can provide so that we may solve this problem. Thanks!")
        End Try
    End Sub

    ''' <summary>
    ''' Encrypt a password and append it to the password dictionary button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Wrapper As New EncryptionWrapper(My.Settings.SecondaryHash)
        Dim EncryptedData As String = Wrapper.EncryptData(TextBox2.Text)

        PasswordManager.PasswordBroker.Add(TextBox1.Text, EncryptedData)
        ListBox1.Items.Add(TextBox1.Text)
    End Sub

    ''' <summary>
    ''' Save password database button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            Dim BinarySerializerAgent As New BinaryFormatter()
            Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\database.db", FileMode.OpenOrCreate)

            BinarySerializerAgent.Serialize(BinaryStream, PasswordManager.PasswordBroker)

            'Cleanup
            BinaryStream.Close()
            BinaryStream.Dispose()
        Catch ex As Exception
            MessageBox.Show("Warning! Critical application error has occurred! Unable to save the password database." + Environment.NewLine + ex.ToString() + "Please submit a bug report on the github page with a screenshot of this error and any other information you can provide so that we may solve this problem. Thanks!")
        End Try
    End Sub

    ''' <summary>
    ''' This saves the new master password to the password file button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        My.Settings.SecondaryHash = Wrapper.CalculateHash(Encoding.Unicode.GetBytes(TextBox3.Text))
        My.Settings.Save()
    End Sub

    ''' <summary>
    ''' This variable is used for a one time warning when the textbox3.click event is ran for the first time each application instance
    ''' </summary>
    Dim var = 0
    ''' <summary>
    ''' This provides a 1 time warning each program runtime to the user (textbox3 click event handler)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox3_Click(sender As Object, e As EventArgs) Handles TextBox3.Click
        If var = 0 Then
            var = 1
            MessageBox.Show("Warning changing the master password will render the previously encrypted passwords useless. Each encrypted password is tied to the master key individually.")
        End If
    End Sub

    ''' <summary>
    ''' Go back to setup button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim Password = InputBox("Please Enter The Master Application Password", "Authentication")
        If Wrapper.CalculateHash(Encoding.Unicode.GetBytes(Password)) = My.Settings.MainHash Then
            My.Settings.isFirstRun = True
            My.Settings.Save()
            Setup.Show()
            Form1.Close()
            Me.Close()
        Else
            MessageBox.Show("Invalid Password! Failed password attempt has been logged!")
        End If
    End Sub

    ''' <summary>
    ''' Delete a key from the list. on key down event handler for listbox1
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox1.KeyDown
        If Not ListBox1.SelectedItem Is Nothing Then
            If e.KeyCode = Keys.Delete Then
                Dim Areyousure As DialogResult = MessageBox.Show("Are you sure you want to delete the selected entry?", "Are you sure?", MessageBoxButtons.YesNo)

                If Areyousure = DialogResult.Yes Then
                    Try
                        PasswordManager.PasswordBroker.Remove(ListBox1.SelectedItem.ToString())
                        ListBox1.Items.Remove(ListBox1.SelectedItem)

                        MessageBox.Show("Key deleted successfully, Use the save button to save your changes!")
                    Catch ex As Exception
                        MessageBox.Show("There was an error deleting the key from the list." + Environment.NewLine + ex.ToString())
                    End Try
                End If
            End If
        End If
    End Sub
End Class

''' <summary>
''' A Wrapper containing the essential functions of the encryption services
''' </summary>
Public Class EncryptionWrapper
    ''' <summary>
    ''' A Public Identifier representing the class TripleDESCryptServiceProdivder accessible as a public member.
    ''' </summary>
    Public SecurityBroker As New TripleDESCryptoServiceProvider

    ''' <summary>
    ''' Calculates a SHA256 Hash from an input byte array. [Encoding.Unicode.GetBytes(string)]
    ''' </summary>
    ''' <param name="bytes">An array of bytes to be hashed.</param>
    ''' <returns>A string representing the SHA256 Hash of the inputted data.</returns>
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

    ''' <summary>
    ''' Truncates the hash to remove extra characters from the beginning and end
    ''' </summary>
    ''' <param name="key">Hash key</param>
    ''' <param name="length">Hash length</param>
    ''' <returns>A truncated hash byte array</returns>
    Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()
        Dim sha1 As New SHA1CryptoServiceProvider

        'Hash the key.
        Dim keyBytes() As Byte =
        System.Text.Encoding.Unicode.GetBytes(key)
        Dim hash() As Byte = sha1.ComputeHash(keyBytes)

        'Truncate or pad the hash.
        ReDim Preserve hash(length - 1)
        Return hash
    End Function

    ''' <summary>
    ''' Encrypts data to be safely stored in memory and on the users file system.
    ''' </summary>
    ''' <param name="plaintext">A plain text string to be encrypted.</param>
    ''' <returns>A string of encrypted data.</returns>
    Public Function EncryptData(ByVal plaintext As String) As String
        'Convert the plaintext string to a byte array.
        Dim plaintextBytes() As Byte =
        System.Text.Encoding.Unicode.GetBytes(plaintext)

        'Create the stream.
        Dim ms As New System.IO.MemoryStream
        'Create the encoder to write to the stream.
        Dim encStream As New CryptoStream(ms,
        SecurityBroker.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

        'Use the crypto stream to write the byte array to the stream.
        encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
        encStream.FlushFinalBlock()

        'Convert the encrypted stream to a printable string.
        Return Convert.ToBase64String(ms.ToArray)
    End Function

    ''' <summary>
    ''' Decrypts data from an encrypted string of data.
    ''' </summary>
    ''' <param name="encryptedtext">A string of encrypted data.</param>
    ''' <returns>A plain text string containing the decrypted data held in the input string.</returns>
    Public Function DecryptData(ByVal encryptedtext As String) As String
        'Convert the encrypted text string to a byte array.
        Dim encryptedBytes() As Byte = Convert.FromBase64String(encryptedtext)

        'Create the stream.
        Dim ms As New System.IO.MemoryStream
        'Create the decoder to write to the stream.
        Dim decStream As New CryptoStream(ms,
        SecurityBroker.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)

        ' Use the crypto stream to write the byte array to the stream.
        decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
        decStream.FlushFinalBlock()

        'Convert the plaintext stream to a string.
        Return System.Text.Encoding.Unicode.GetString(ms.ToArray)
    End Function

    ''' <summary>
    ''' Class initializer to initialize the encryption wrapper with a new encryption password.
    ''' </summary>
    ''' <param name="key">Encryption password string.</param>
    Sub New(ByVal key As String)
        'Initialize the crypto provider.
        SecurityBroker.Key = TruncateHash(key, SecurityBroker.KeySize \ 8)
        SecurityBroker.IV = TruncateHash("", SecurityBroker.BlockSize \ 8)
    End Sub
End Class

<Serializable>
Public Class BlockKey
    Public Key As String
End Class

<Serializable>
Public Class PasswordDictionary
    Public PasswordBroker As New Dictionary(Of String, String)
End Class