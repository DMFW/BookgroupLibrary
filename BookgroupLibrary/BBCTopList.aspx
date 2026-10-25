<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BBCTopList.aspx.cs" Inherits="BookgroupLibrary.BBCTopListForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="grdBBCList" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
        DataSourceID="SqlDataSourceBBCList" 
        EmptyDataText="There are no data records to display."
        GridLines="None" onselectedindexchanged="grdBBCList_SelectedIndexChanged" 
        PageSize="50" DataKeyNames="BookID" CssClass="bookGrid">
        <HeaderStyle CssClass="bookGridHeaderStyle" />
        <FooterStyle CssClass="bookGridFooterStyle" />
        <PagerStyle HorizontalAlign="Center" CssClass="bookGridPagerStyle" />
        <RowStyle CssClass="bookGridRowStyle" />
        <AlternatingRowStyle CssClass="bookGridRowAltStyle"/>
        <EditRowStyle CssClass="bookGridRowEditStyle" />
        <SelectedRowStyle CssClass="bookGridRowSelectedStyle" />
        <SortedAscendingCellStyle CssClass="bookGridSortedAscendingCellStyle" />
        <SortedAscendingHeaderStyle CssClass="bookGridSortedAscendingHeaderStyle" />
        <SortedDescendingCellStyle CssClass="bookGridSortedDescendingCellStyle" />
        <SortedDescendingHeaderStyle CssClass="bookGridSortedDescendingHeaderStyle" />
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
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceBBCList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="SELECT [Title], [YearPublished], [MonthRead], [Author], [TopBookPosition], [BookID], [DateRead] FROM [BBCTopList] ORDER BY [TopBookPosition]">
    </asp:SqlDataSource>
</asp:Content>
