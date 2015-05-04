<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileGridCS.ascx.cs" Inherits="UserControls_FileGridCS" %>
<p>
    <asp:Label ID="lblCurrentPath" runat="server"></asp:Label>
</p>

<asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" 
    CellPadding="4" ForeColor="#333333" GridLines="None" 
    onpageindexchanging="gvFiles_PageIndexChanging" 
    onrowcommand="gvFiles_RowCommand" onrowdatabound="gvFiles_RowDataBound">
    <AlternatingRowStyle BackColor="White" />
    <Columns>
        <asp:TemplateField HeaderText="Name" SortExpression="Name">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenFolder" CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                <asp:Literal runat="server" ID="ltlFileItem"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="FileSystemType" HeaderText="Type" 
            SortExpression="FileSystemType" />
        <asp:BoundField DataField="LastWriteTime" HeaderText="Date Modified" 
            SortExpression="LastWriteTime" />
        <asp:TemplateField HeaderText="Size" SortExpression="Size" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# DisplaySize((long?) Eval("Size")) %>
            </ItemTemplate>

<ItemStyle HorizontalAlign="Right"></ItemStyle>
        </asp:TemplateField>
    </Columns>
    <EditRowStyle BackColor="#2461BF" />
    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
    <RowStyle BackColor="#EFF3FB" />
    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    <SortedAscendingCellStyle BackColor="#F5F7FB" />
    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
    <SortedDescendingCellStyle BackColor="#E9EBEF" />
    <SortedDescendingHeaderStyle BackColor="#4870BE" />
</asp:GridView>