<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OurChoicesList.aspx.cs" Inherits="BookgroupLibrary.OurChoicesListForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="grdOurList" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
        DataSourceID="SqlDataSourceOurList" 
        EmptyDataText="There are no data records to display." 
        GridLines="None" onselectedindexchanged="grdOurList_SelectedIndexChanged" 
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
            <asp:BoundField DataField="DisplayDate" HeaderText="Month Read" 
                SortExpression="SortDate" ReadOnly="True" />
            <asp:BoundField DataField="ChosenBy" HeaderText="Chosen By" 
                SortExpression="ChosenBy" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceOurList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="SELECT [Title], [YearPublished], [MonthRead], [Author], [ChosenBy], [BookID], [DisplayDate], [SortDate] FROM [OurList] ORDER BY [SortDate] Desc">
    </asp:SqlDataSource>
</asp:Content>
