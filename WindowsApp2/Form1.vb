Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class Form1
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private PasswordManager As New PasswordDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' When the selected listbox item is changed attempt to find the password for the new selected listbox item and display it to the user.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Try
            If Wrapper.CalculateHash(Encoding.Unicode.GetBytes(TextBox2.Text.ToString())) = My.Settings.SecondaryHash Then
                Dim Data = PasswordManager.PasswordBroker(ListBox1.SelectedItem.ToString)
                Dim Wrapper As New EncryptionWrapper(My.Settings.SecondaryHash)

                Try
                    Dim plainText As String = Wrapper.DecryptData(Data)
                    TextBox1.Text = plainText
                Catch ex As System.Security.Cryptography.CryptographicException
                    MsgBox("The data could not be decrypted with the password.")
                End Try
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Settings Form Button click event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Settings.Show()
    End Sub

    ''' <summary>
    ''' Form1 Onload event handler that loads saved passwords from the database
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
    ''' Transition to the serial key manager button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        SerialKeyCenter.Show()
        Me.Close()
    End Sub

    ''' <summary>
    ''' Developer Tools (Changelog Generator?) button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DevTools.Show()
    End Sub
End Class