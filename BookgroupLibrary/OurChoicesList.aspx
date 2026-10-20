<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OurChoicesList.aspx.cs" Inherits="BookgroupLibrary.OurChoicesListForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="grdOurList" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
        DataSourceID="SqlDataSourceOurList" 
        EmptyDataText="There are no data records to display." ForeColor="#333333" 
        GridLines="None" onselectedindexchanged="grdOurList_SelectedIndexChanged" 
        PageSize="50" DataKeyNames="BookID">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" ButtonType="Button" />
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
            <asp:BoundField DataField="Author" HeaderText="Author" 
                SortExpression="Author" ReadOnly="True" />
            <asp:BoundField DataField="YearPublished" HeaderText="Published" 
                SortExpression="YearPublished" ReadOnly="True" />
            <asp:BoundField DataField="DisplayDate" HeaderText="Month Read" 
                SortExpression="SortDate" ReadOnly="True" />
            <asp:BoundField DataField="ChosenBy" HeaderText="Chosen By" 
                SortExpression="ChosenBy" />
        </Columns>
        <EditRowStyle BackColor="#7C6F57" />
        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#E3EAEB" />
        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F8FAFA" />
        <SortedAscendingHeaderStyle BackColor="#246B61" />
        <SortedDescendingCellStyle BackColor="#D4DFE1" />
        <SortedDescendingHeaderStyle BackColor="#15524A" />
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceOurList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="SELECT [Title], [YearPublished], [MonthRead], [Author], [ChosenBy], [BookID], [DisplayDate], [SortDate] FROM [OurList] ORDER BY [SortDate] Desc">
    </asp:SqlDataSource>
</asp:Content>
