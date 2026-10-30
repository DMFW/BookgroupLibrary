<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SecretSantas.aspx.cs" Inherits="BookgroupLibrary.SecretSantasListForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="grdSantasList" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
        DataSourceID="SqlDataSourceSantasList" 
        EmptyDataText="There are no data records to display." 
        GridLines="None" onselectedindexchanged="grdSantasList_SelectedIndexChanged" 
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
            <asp:BoundField DataField="ChristmasYear" HeaderText="Christmas" SortExpression="ChristmasYear" />
            <asp:BoundField DataField="GiftedByName" HeaderText="Gifted By" SortExpression="GiftedBySortName" />
            <asp:BoundField DataField="GiftedToName" HeaderText="Gifted To" SortExpression="GiftedToSortName" />
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
            <asp:BoundField DataField="DateRead" HeaderText="Date Read" 
                SortExpression="DateRead" DataFormatString="{0:D}" NullDisplayText="Not Read Yet" ReadOnly="True" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceSantasList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="Select ChristmasYear,
                        Concat(GiftedByMember.FirstName, ' ', GiftedByMember.LastName) as GiftedByName,
                        Concat(GiftedByMember.LastName, ' ', GiftedByMember.FirstName) as GiftedBySortName,
                        Concat(GiftedToMember.FirstName,' ', GiftedToMember.LastName) as GiftedToName,
                        Concat(GiftedToMember.LastName,' ', GiftedToMember.FirstName) as GiftedToSortName,
                        Book.BookID,
                        Book.Title,
                        Book.DateRead
                        from SecretSantaBook Left join GroupMember as GiftedByMember on GiftedBy = GiftedByMember.GroupMemberID
                        Left join GroupMember as GiftedToMember on GiftedTo = GiftedToMember.GroupMemberID
                        inner join Book on SecretSantaBook.BookID=Book.BookID
                        order by ChristmasYear desc, CASE WHEN Book.DateRead IS NULL THEN 0 ELSE 1 END, DateRead desc, Book.Title">
    </asp:SqlDataSource>
</asp:Content>
