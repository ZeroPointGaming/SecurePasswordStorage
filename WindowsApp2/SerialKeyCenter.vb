Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class SerialKeyCenter
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private SerialManager As New SerialKeyDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' OnLoad event handler for the serial key center form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SerialKeyCenter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'If the serial key database file exists then deserialize the file and import it to the listbox
        If File.Exists(Directory.GetCurrentDirectory + "\backup.db") Then
            Dim BinaryAgent As New BinaryFormatter
            Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\backup.db", FileMode.Open)
            SerialManager.SerialKeyChain = BinaryAgent.Deserialize(BinaryStream)

            For Each key In SerialManager.SerialKeyChain
                ListBox1.Items.Add(key.Key)
            Next

            'Once finished close and dispose of the binary stream so the file is later usable again and saving on memory.
            BinaryStream.Close()
            BinaryStream.Dispose()
        Else
            MessageBox.Show("No key file exists, please open the settings and create a new keyfile by saving atleast 1 key.")
        End If
    End Sub

    ''' <summary>
    ''' Open the serial key settings menu when the settings button is clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SerialKeyCenterSettings.Show()
    End Sub

    ''' <summary>
    ''' When the list box selection is changed check if the entered password is correct and then display the serial key in the textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Try
            Dim PasswordAttempt As String = Wrapper.CalculateHash(Encoding.Unicode.GetBytes(TextBox2.Text))
            If PasswordAttempt = My.Settings.SecondaryHash Then
                Dim Data = SerialManager.SerialKeyChain(ListBox1.SelectedItem.ToString)
                Dim Wrapper As New EncryptionWrapper(My.Settings.SecondaryHash)

                Try
                    Dim plainText As String = Wrapper.DecryptData(Data)
                    TextBox1.Text = plainText
                Catch ex As System.Security.Cryptography.CryptographicException
                    MsgBox("The data could not be decrypted with the master encryption password.")
                End Try
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Return to the password center button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form1.Show()
        Me.Close()
    End Sub
End Class