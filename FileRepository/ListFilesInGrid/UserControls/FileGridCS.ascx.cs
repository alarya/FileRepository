using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UserControls_FileGridCS : System.Web.UI.UserControl
{
    #region Properties
    public string HomeFolder
    {
        get
        {
            return ViewState["HomeFolder"] as string;
        }
        set
        {
            ViewState["HomeFolder"] = value;
        }
    }

    public string CurrentFolder
    {
        get
        {
            return ViewState["CurrentFolder"] as string;
        }
        set
        {
            ViewState["CurrentFolder"] = value;
        }
    }

    public int PageSize
    {
        get
        {
            return gvFiles.PageSize;
        }
        set
        {
            gvFiles.PageSize = value;

            if (value <= 0)
                gvFiles.AllowPaging = false;
            else
                gvFiles.AllowPaging = true;
        }
    }
#endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (string.IsNullOrEmpty(this.HomeFolder) || !Directory.Exists(GetFullyQualifiedFolderPath(this.HomeFolder)))
                throw new ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", this.HomeFolder));
            
            this.CurrentFolder = this.HomeFolder;

            PopulateGrid();
        }
    }

    private void PopulateGrid()
    {
        // Get the list of files & folders in the CurrentFolder
        var currentDirInfo = new DirectoryInfo(GetFullyQualifiedFolderPath(this.CurrentFolder));
        var folders = currentDirInfo.GetDirectories();
        var files = currentDirInfo.GetFiles();

        var fsItems = new List<FileSystemItemCS>(folders.Length + files.Length);

        // Add the ".." option, if needed
        if (!TwoFoldersAreEquivalent(currentDirInfo.FullName, GetFullyQualifiedFolderPath(this.HomeFolder)))
        {
            var parentFolder = new FileSystemItemCS(currentDirInfo.Parent);
            parentFolder.Name = "..";
            fsItems.Add(parentFolder);
        }

        foreach(var folder in folders)
            fsItems.Add(new FileSystemItemCS(folder));
        
        foreach(var file in files)
            fsItems.Add(new FileSystemItemCS(file));

        gvFiles.DataSource = fsItems;
        gvFiles.DataBind();


        var currentFolderDisplay = this.CurrentFolder;
        if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
            currentFolderDisplay = currentFolderDisplay.Substring(2);
        
        lblCurrentPath.Text = "Viewing the folder <b>" + currentFolderDisplay + "</b>";
    }

    protected string DisplaySize(long? size)
    {
        if (size == null)
            return string.Empty;
        else
        {
            if (size < 1024)
                return string.Format("{0:N0} bytes", size.Value);
            else
                return String.Format("{0:N0} KB", size.Value / 1024);
        }
    }    

    private string GetFullyQualifiedFolderPath(string folderPath)
    {
        if (folderPath.StartsWith("~"))
            return Server.MapPath(folderPath);
        else
            return folderPath;
    }

    private bool TwoFoldersAreEquivalent(string folderPath1, string folderPath2)
    {
        // Chop off any trailing slashes...
        if (folderPath1.EndsWith("\\") || folderPath1.EndsWith("/"))
            folderPath1 = folderPath1.Substring(0, folderPath1.Length - 1);

        if (folderPath2.EndsWith("\\") || folderPath2.EndsWith("/"))
            folderPath2 = folderPath1.Substring(0, folderPath2.Length - 1);

        return string.CompareOrdinal(folderPath1, folderPath2) == 0;
    }

    #region GridView Event Handlers
    protected void gvFiles_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvFiles.PageIndex = e.NewPageIndex;

        PopulateGrid();
    }

    protected void gvFiles_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "OpenFolder")
        {
            if (string.CompareOrdinal(e.CommandArgument.ToString(), "..") == 0)
            {
                var currentFullPath = this.CurrentFolder;
                if (currentFullPath.EndsWith("\\") || currentFullPath.EndsWith("/"))
                    currentFullPath = currentFullPath.Substring(0, currentFullPath.Length - 1);

                currentFullPath = currentFullPath.Replace("/", "\\");

                var folders = currentFullPath.Split("\\".ToCharArray());

                this.CurrentFolder = string.Join("\\", folders, 0, folders.Length - 1);
            }
            else
                this.CurrentFolder = Path.Combine(this.CurrentFolder, e.CommandArgument as string);


            PopulateGrid();
        }
    }

    protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var item = e.Row.DataItem as FileSystemItemCS;

            if (item.IsFolder)
            {
                var lbFolderItem = e.Row.FindControl("lbFolderItem") as LinkButton;
                lbFolderItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", Page.ResolveClientUrl("~/Images/folder.png"), item.Name);
            }
            else
            {
                var ltlFileItem = e.Row.FindControl("ltlFileItem") as Literal;
                if (this.CurrentFolder.StartsWith("~"))
                    ltlFileItem.Text = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>",
                            Page.ResolveClientUrl(string.Concat(this.CurrentFolder, "/", item.Name).Replace("//", "/")),
                            item.Name);
                else
                    ltlFileItem.Text = item.Name;
            }
        }
    }
    #endregion
    
}