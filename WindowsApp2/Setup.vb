Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text

Public Class Setup
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private PasswordManager As New PasswordDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' Setup form on load event handler to check if the program has been run before.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Setup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.isFirstRun = True Then
            My.Settings.isFirstRun = False
            My.Settings.Save()
        Else
            LoginForm.Show()
            Me.Close()
        End If
    End Sub

    ''' <summary>
    ''' Save main application settings and proceed to the login form button event handler.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then MessageBox.Show("Please enter an application password")
        If TextBox2.Text = "" Then MessageBox.Show("Please enter a master encryption password")

        '''<summary>
        '''If the textboxes arent empty save the password hashes to the application configuration file then display the login form
        ''' </summary>
        If Not TextBox1.Text = "" Or Not TextBox2.Text = "" Then
            My.Settings.MainHash = Wrapper.CalculateHash(Encoding.Unicode.GetBytes(TextBox1.Text))
            My.Settings.Save()
            My.Settings.SecondaryHash = Wrapper.CalculateHash(Encoding.Unicode.GetBytes(TextBox2.Text))
            My.Settings.Save()

            LoginForm.Show()
            Me.Close()
        Else
            MessageBox.Show("Error one or more of the password boxes are empty or contain invalid passwords.")
        End If
    End Sub
End Class