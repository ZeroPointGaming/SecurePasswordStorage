Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Xml.Serialization
Imports System.Collections

Public Class DevTools
#Region "Important System Variables"
    Private SecurityBroker As New TripleDESCryptoServiceProvider
    Private Wrapper As New EncryptionWrapper(key:=0)
    Private PasswordManager As New PasswordDictionary
    Private KeyManager As New BlockKey
#End Region

    ''' <summary>
    ''' OnLoad event for the Dev Tools Form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DevTools_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim AppMan = My.Application.Info.Version
        TextBox1.Text = AppMan.Major.ToString + "." + AppMan.MajorRevision.ToString + "." + AppMan.Build.ToString + "." + AppMan.MinorRevision.ToString
    End Sub

#Region "Listbox Keypress Events"
    ''' <summary>
    ''' Try to delete selected listbox item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox1.KeyDown
        Try
            If e.KeyCode = Keys.Delete Then
                ListBox1.Items.Remove(ListBox1.SelectedItem)
            End If
        Catch ex As Exception
            ex = Nothing
        End Try
    End Sub
    ''' <summary>
    ''' Try to delete selected listbox item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox2.KeyDown
        Try
            If e.KeyCode = Keys.Delete Then
                ListBox2.Items.Remove(ListBox2.SelectedItem)
            End If
        Catch ex As Exception
            ex = Nothing
        End Try
    End Sub
    ''' <summary>
    ''' Try to delete selected listbox item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox3.KeyDown
        Try
            If e.KeyCode = Keys.Delete Then
                ListBox3.Items.Remove(ListBox3.SelectedItem)
            End If
        Catch ex As Exception
            ex = Nothing
        End Try
    End Sub
    ''' <summary>
    ''' Try to delete selected listbox item
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ListBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles ListBox4.KeyDown
        Try
            If e.KeyCode = Keys.Delete Then
                ListBox4.Items.Remove(ListBox4.SelectedItem)
            End If
        Catch ex As Exception
            ex = Nothing
        End Try
    End Sub
#End Region

#Region "Button Click Event Handlers"
    ''' <summary>
    ''' Button Event to add changes to the changes listbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not ChangeTxt.Text Is Nothing Then
            ListBox1.Items.Add(ChangeTxt.Text)
        End If
    End Sub

    ''' <summary>
    ''' Button Event to add features to the features listbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Not FeatureTxt.Text Is Nothing Then
            ListBox2.Items.Add(FeatureTxt.Text)
        End If
    End Sub

    ''' <summary>
    ''' Button Event to add revisisions to the revisions listbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Not RevisionTxt.Text Is Nothing Then
            ListBox3.Items.Add(RevisionTxt.Text)
        End If
    End Sub

    ''' <summary>
    ''' Button Event to add bug fixes to the bug fixes listbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Not BugfixTxt.Text Is Nothing Then
            ListBox4.Items.Add(BugfixTxt.Text)
        End If
    End Sub

    ''' <summary>
    ''' Save the changelog button event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim NewUpdate As New Updates

        NewUpdate.Build.Version = TextBox1.Text

        If TextBox2.Text = "DD-MM-YY" Then
            NewUpdate.Build.Released = Today.ToString()
        Else
            NewUpdate.Build.Released = TextBox2.Text
        End If

        For Each item In ListBox1.Items
            Dim c As New Change With {
                .Change = item.ToString()
            }
            NewUpdate.Build.Changes.Add(c)
        Next

        For Each item In ListBox2.Items
            Dim f As New Feature With {
                .Feature = item.ToString()
            }
            NewUpdate.Build.Features.Add(f)
        Next

        For Each item In ListBox3.Items
            Dim r As New Revision With {
                .Revision = item.ToString()
            }
            NewUpdate.Build.Revisions.Add(r)
        Next

        For Each item In ListBox4.Items
            Dim b As New BugFix With {
                .BugFix = item.ToString()
            }
            NewUpdate.Build.BugFixes.Add(b)
        Next

        Dim serializer As New XmlSerializer(GetType(Updates))
        Dim Data As New StringWriter()

        serializer.Serialize(Data, NewUpdate)

        Clipboard.SetText(Data.ToString())
    End Sub
#End Region
End Class

''' <summary>
''' Changelog serialization wrapper
''' </summary>
Partial Public Class Updates
    Public Build As New Build
End Class

''' <summary>
''' Changelog build serialization container
''' </summary>
Partial Public Class Build
    Public Changes As New List(Of Change)
    Public Features As New List(Of Feature)
    Public Revisions As New List(Of Revision)
    Public BugFixes As New List(Of BugFix)

    <XmlAttribute>
    Public Version As String = ""
    <XmlAttribute>
    Public Released As String = ""
End Class

''' <summary>
''' Change object for changes list
''' </summary>
Public Class Change
    Public Change As String = ""
End Class

''' <summary>
''' Feature object for features list
''' </summary>
Public Class Feature
    Public Feature As String = ""
End Class

''' <summary>
''' Revision object for revision list
''' </summary>
Public Class Revision
    Public Revision As String = ""
End Class

''' <summary>
''' Bug fix object for bug fix list
''' </summary>
Public Class BugFix
    Public BugFix As String = ""
End Class