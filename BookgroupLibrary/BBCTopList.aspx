<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BBCTopList.aspx.cs" Inherits="BookgroupLibrary.BBCTopListForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="grdBBCList" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
        DataSourceID="SqlDataSourceBBCList" 
        EmptyDataText="There are no data records to display." ForeColor="#333333" 
        GridLines="None" onselectedindexchanged="grdBBCList_SelectedIndexChanged" 
        PageSize="50" DataKeyNames="BookID">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" ButtonType="Button" />
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
            <asp:BoundField DataField="Author" HeaderText="Author" 
                SortExpression="Author" ReadOnly="True" />
            <asp:BoundField DataField="YearPublished" HeaderText="Published" 
                SortExpression="YearPublished" ReadOnly="True" />
            <asp:BoundField DataField="MonthRead" HeaderText="Month Read" 
                SortExpression="DateRead" ReadOnly="True" />
            <asp:BoundField DataField="TopBookPosition" HeaderText="BBC Rank" 
                SortExpression="TopBookPosition" />
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
    <asp:SqlDataSource ID="SqlDataSourceBBCList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="SELECT [Title], [YearPublished], [MonthRead], [Author], [TopBookPosition], [BookID], [DateRead] FROM [BBCTopList] ORDER BY [TopBookPosition]">
    </asp:SqlDataSource>
</asp:Content>
