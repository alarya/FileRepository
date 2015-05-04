Imports Microsoft.VisualBasic
Imports System.IO

Public Class FileSystemItemVB
    Public Sub New(ByVal file As FileInfo)
        Me.Name = file.Name
        Me.FullName = file.FullName
        Me.Size = file.Length
        Me.CreationTime = file.CreationTime
        Me.LastAccessTime = file.LastAccessTime
        Me.LastWriteTime = file.LastWriteTime
        Me.IsFolder = False
    End Sub

    Public Sub New(ByVal folder As DirectoryInfo)
        Me.Name = folder.Name
        Me.FullName = folder.FullName
        Me.Size = Nothing
        Me.CreationTime = folder.CreationTime
        Me.LastAccessTime = folder.LastAccessTime
        Me.LastWriteTime = folder.LastWriteTime
        Me.IsFolder = True
    End Sub

    Public Property Name As String
    Public Property FullName As String
    Public Property Size As Nullable(Of Long)
    Public Property CreationTime As DateTime
    Public Property LastAccessTime As DateTime
    Public Property LastWriteTime As DateTime
    Public Property IsFolder As Boolean

    Public ReadOnly Property FileSystemType As String
        Get
            If Me.IsFolder Then
                Return "File folder"
            Else
                Dim extension = Path.GetExtension(Me.Name)
                If IsMatch(extension, ".txt") Then
                    Return "Text file"
                ElseIf IsMatch(extension, ".pdf") Then
                    Return "PDF file"
                ElseIf IsMatch(extension, ".doc", ".docx") Then
                    Return "Microsoft Word document"
                ElseIf IsMatch(extension, ".xls", ".xlsx") Then
                    Return "Microsoft Excel document"
                ElseIf IsMatch(extension, ".jpg", ".jpeg") Then
                    Return "JPEG image file"
                ElseIf IsMatch(extension, ".gif") Then
                    Return "GIF image file"
                ElseIf IsMatch(extension, ".png") Then
                    Return "PNG image file"
                End If

                'If we reach here, return the name of the extension
                If String.IsNullOrEmpty(extension) Then
                    Return "Unknown file type"
                Else
                    Return extension.Substring(1).ToUpper() & " file"
                End If
            End If
        End Get
    End Property

    Private Function IsMatch(ByVal extension As String, ByVal ParamArray extensionsToCheck As String()) As Boolean
        For Each str As String In extensionsToCheck
            If String.CompareOrdinal(extension, str) = 0 Then
                Return True
            End If
        Next

        'If we reach here, no match
        Return False
    End Function
End Class
