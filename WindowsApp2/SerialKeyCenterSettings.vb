Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class SerialKeyCenterSettings
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private SerialManager As New SerialKeyDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' Settings form on load event handler to load currently saved serial keys from the db file to the listbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SerialKeyCenterSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'If the serial key database file exists then deserialize the file and import it to the listbox
        If File.Exists(Directory.GetCurrentDirectory + "\backup.db") Then
            Dim BinaryAgent As New BinaryFormatter
            Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\backup.db", FileMode.Open)
            SerialManager.SerialKeyChain = BinaryAgent.Deserialize(BinaryStream)

            For Each key In SerialManager.SerialKeyChain
                ListBox1.Items.Add(key.Key)
            Next

            BinaryStream.Close()
            BinaryStream.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Encrypt the serial key then add it to the key dictionary button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Wrapper As New EncryptionWrapper(My.Settings.SecondaryHash)
        Dim EncryptedData As String = Wrapper.EncryptData(TextBox2.Text)

        SerialManager.SerialKeyChain.Add(TextBox1.Text, EncryptedData)
        ListBox1.Items.Add(TextBox1.Text)
    End Sub

    ''' <summary>
    ''' Serialize the serial keys to file button event handler
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'If the serial key database file exists then Serialize the file and import it to the listbox
        If File.Exists(Directory.GetCurrentDirectory + "\backup.db") Then
            Dim BinaryAgent As New BinaryFormatter
            Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\backup.db", FileMode.Open)

            BinaryAgent.Serialize(BinaryStream, SerialManager.SerialKeyChain)

            BinaryStream.Close()
            BinaryStream.Dispose()
        End If

        'If the serial key database file does not exist then create it and serialize the serial keys to it.
        If Not File.Exists(Directory.GetCurrentDirectory + "\backup.db") Then
            Dim BinaryAgent As New BinaryFormatter
            Dim BinaryStream As New FileStream(Directory.GetCurrentDirectory + "\backup.db", FileMode.OpenOrCreate)

            BinaryAgent.Serialize(BinaryStream, SerialManager.SerialKeyChain)

            BinaryStream.Close()
            BinaryStream.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Delete a key from the list. OnKeyDown event handler for listbox1.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox1.KeyDown
        If Not ListBox1.SelectedItem Is Nothing Then
            If e.KeyCode = Keys.Delete Then
                Dim Areyousure As DialogResult = MessageBox.Show("Are you sure you want to delete the selected entry?", "Are you sure?", MessageBoxButtons.YesNo)

                If Areyousure = DialogResult.Yes Then
                    SerialManager.SerialKeyChain.Remove(ListBox1.SelectedItem.ToString())
                    ListBox1.Items.Remove(ListBox1.SelectedItem)

                    MessageBox.Show("Key deleted successfully, Use the save button to save your changes!")
                End If
            End If
        End If
    End Sub
End Class

<Serializable>
Public Class SerialKeyDictionary
    Public SerialKeyChain As New Dictionary(Of String, String)
End Class