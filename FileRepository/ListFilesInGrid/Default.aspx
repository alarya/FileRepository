<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <p>
        This demo showcases a User Control that uses a GridView to list the files (and folders) of a 
        particular folder in a GridView control. Specifically, this demo lists the 
        contents of the <code>Content</code> folder.</p>
    <ul>
        <li><a href="ListFilesCS.aspx">List Files in a Grid - C# Demo</a></li>
        <li><a href="ListFilesVB.aspx">List Files in a Grid - VB Demo</a></li>
    </ul>
    <p>
        Happy Programming!
    </p>
    <p style="padding-left: 20px">
        <a href="http://www.4guysfromrolla.com/ScottMitchell.shtml">Scott Mitchell</a><br />
        <a href="http://scottonwriting.net/">My Blog</a>
    </p>
</asp:Content>

