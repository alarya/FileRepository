<html>
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ListFilesCS.aspx.cs" Inherits="ListFilesCS" %>

<%@ Register src="UserControls/FileGridCS.ascx" tagname="FileGridVB" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>
        A Simple Filebrowser (C#)
    </h2>
    
    <uc1:FileGridVB ID="FileGridVB1" HomeFolder="~/Content" runat="server" PageSize="10" />

</asp:Content>
</html>
