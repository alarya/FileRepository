Imports System.IO

Partial Class UserControls_FileGridVB
    Inherits System.Web.UI.UserControl

#Region "Properties"
    Public Property HomeFolder As String
        Get
            Dim o As Object = ViewState("HomeFolder")
            If o Is Nothing Then
                Return Nothing
            Else
                Return o.ToString()
            End If
        End Get
        Set(ByVal value As String)
            ViewState("HomeFolder") = value
        End Set
    End Property

    Public Property CurrentFolder As String
        Get
            Dim o As Object = ViewState("CurrentFolder")
            If o Is Nothing Then
                Return Nothing
            Else
                Return o.ToString()
            End If
        End Get
        Set(ByVal value As String)
            ViewState("CurrentFolder") = value
        End Set
    End Property

    Public Property PageSize As Integer
        Get
            Return gvFiles.PageSize
        End Get
        Set(ByVal value As Integer)
            gvFiles.PageSize = value

            If value <= 0 Then
                gvFiles.AllowPaging = False
            Else
                gvFiles.AllowPaging = True
            End If
        End Set
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If String.IsNullOrEmpty(Me.HomeFolder) OrElse Not Directory.Exists(GetFullyQualifiedFolderPath(Me.HomeFolder)) Then
                Throw New ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", Me.HomeFolder))
            End If

            Me.CurrentFolder = Me.HomeFolder

            PopulateGrid()
        End If
    End Sub

    Private Sub PopulateGrid()
        'Get the list of files & folders in the CurrentFolder
        Dim currentDirInfo As New DirectoryInfo(GetFullyQualifiedFolderPath(Me.CurrentFolder))
        Dim folders = currentDirInfo.GetDirectories()
        Dim files = currentDirInfo.GetFiles()

        Dim fsItems As New List(Of FileSystemItemVB)(folders.Length + files.Length)

        'Add the ".." option, if needed
        If Not TwoFoldersAreEquivalent(currentDirInfo.FullName, GetFullyQualifiedFolderPath(Me.HomeFolder)) Then
            Dim parentFolder As New FileSystemItemVB(currentDirInfo.Parent)
            parentFolder.Name = ".."
            fsItems.Add(parentFolder)
        End If

        For Each folder In folders
            fsItems.Add(New FileSystemItemVB(folder))
        Next
        For Each file In files
            fsItems.Add(New FileSystemItemVB(file))
        Next

        gvFiles.DataSource = fsItems
        gvFiles.DataBind()


        Dim currentFolderDisplay = Me.CurrentFolder
        If currentFolderDisplay.StartsWith("~/") OrElse currentFolderDisplay.StartsWith("~\") Then
            currentFolderDisplay = currentFolderDisplay.Substring(2)
        End If
        lblCurrentPath.Text = "Viewing the folder <b>" & currentFolderDisplay & "</b>"
    End Sub

#Region "GridView Event Handlers"
    Protected Sub gvFiles_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvFiles.PageIndexChanging
        gvFiles.PageIndex = e.NewPageIndex

        PopulateGrid()
    End Sub

    Protected Sub gvFiles_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvFiles.RowCommand
        If e.CommandName = "OpenFolder" Then
            If String.CompareOrdinal(e.CommandArgument.ToString(), "..") = 0 Then
                Dim currentFullPath = Me.CurrentFolder
                If currentFullPath.EndsWith("\") OrElse currentFullPath.EndsWith("/") Then
                    currentFullPath = currentFullPath.Substring(0, currentFullPath.Length - 1)
                End If
                currentFullPath = currentFullPath.Replace("/", "\")

                Dim folders = currentFullPath.Split("\".ToCharArray())

                Me.CurrentFolder = String.Join("\", folders, 0, folders.Length - 1)
            Else
                Me.CurrentFolder = Path.Combine(Me.CurrentFolder, e.CommandArgument)
            End If

            PopulateGrid()
        End If
    End Sub

    Protected Sub gvFiles_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvFiles.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim item As FileSystemItemVB = CType(e.Row.DataItem, FileSystemItemVB)

            If item.IsFolder Then
                Dim lbFolderItem As LinkButton = CType(e.Row.FindControl("lbFolderItem"), LinkButton)
                lbFolderItem.Text = String.Format("<img src=""{0}"" alt="""" />&nbsp;{1}", Page.ResolveClientUrl("~/Images/folder.png"), item.Name)
            Else
                Dim ltlFileItem As Literal = CType(e.Row.FindControl("ltlFileItem"), Literal)
                If Me.CurrentFolder.StartsWith("~") Then
                    ltlFileItem.Text = String.Format("<a href=""{0}"" target=""_blank"">{1}</a>",
                                                     Page.ResolveClientUrl(String.Concat(Me.CurrentFolder, "/", item.Name).Replace("//", "/")),
                                                     item.Name)
                Else
                    ltlFileItem.Text = item.Name
                End If
            End If
        End If
    End Sub
#End Region

    Protected Function DisplaySize(ByVal size As Nullable(Of Long)) As String
        If size Is Nothing Then
            Return String.Empty
        Else
            If size < 1024 Then
                Return String.Format("{0:N0} bytes", size.Value)
            Else
                Return String.Format("{0:N0} KB", size.Value / 1024)
            End If
        End If
    End Function

    Private Function GetFullyQualifiedFolderPath(ByVal folderPath As String) As String
        If folderPath.StartsWith("~") Then
            Return Server.MapPath(folderPath)
        Else
            Return folderPath
        End If
    End Function

    Private Function TwoFoldersAreEquivalent(ByVal folderPath1 As String, ByVal folderPath2 As String) As Boolean
        'Chop off any trailing slashes...
        If folderPath1.EndsWith("\") OrElse folderPath1.EndsWith("/") Then
            folderPath1 = folderPath1.Substring(0, folderPath1.Length - 1)
        End If

        If folderPath2.EndsWith("\") OrElse folderPath2.EndsWith("/") Then
            folderPath2 = folderPath1.Substring(0, folderPath2.Length - 1)
        End If

        Return String.CompareOrdinal(folderPath1, folderPath2) = 0
    End Function
End Class
