<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ListFilesVB.aspx.vb" Inherits="ListFilesVB" %>

<%@ Register src="UserControls/FileGridVB.ascx" tagname="FileGridVB" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>
        A Simple Filebrowser (VB)
    </h2>
    
    <uc1:FileGridVB ID="FileGridVB1" HomeFolder="~/Content" runat="server" PageSize="10" />
</asp:Content>

