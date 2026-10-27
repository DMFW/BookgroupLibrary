<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookDetail.aspx.cs" Inherits="BookgroupLibrary.BookDetailForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="lblAuthors" runat="server" Text="Author(s)"></asp:Label>
    <asp:GridView ID="grdBookAuthors" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="AuthorID" DataSourceID="SqlDataSourceBookAuthors" 
        CellPadding="4" GridLines="None" CssClass="defaultgrid">
         <HeaderStyle CssClass="defaultgridHeaderStyle" />
        <FooterStyle CssClass="defaultgridFooterStyle" />
        <PagerStyle HorizontalAlign="Center" CssClass="defaultgridPagerStyle" />
        <RowStyle CssClass="defaultgridRowStyle" />
        <AlternatingRowStyle CssClass="defaultgridRowAltStyle"/>
        <EditRowStyle CssClass="defaultgridEditRowStyle" />
        <SelectedRowStyle CssClass="defaultgridSelectedRowStyle" />
        <SortedAscendingCellStyle CssClass="defaultgridSortedAscendingCellStyle" />
        <SortedAscendingHeaderStyle CssClass="defaultgridSortedAscendingHeaderStyle" />
        <SortedDescendingCellStyle CssClass="defaultgridSortedDescendingCellStyle" />
        <SortedDescendingHeaderStyle CssClass="defaultgridSortedDescendingHeaderStyle" />
        <Columns>
            <asp:BoundField DataField="FirstName" HeaderText="Forename" 
                SortExpression="FirstName" />
            <asp:BoundField DataField="LastName" HeaderText="Surname" 
                SortExpression="LastName" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceBookAuthors" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        SelectCommand="SELECT [Author].[AuthorID], [LastName], [FirstName] FROM [BookAuthor] left join [Author] on [BookAuthor].[AuthorID] = [Author].[AuthorID] WHERE ([BookID] = @BookID) ORDER BY [LastName]">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="1" Name="BookID" 
                QueryStringField="SelectedBookID" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <hr />
    <asp:Label ID="lblDetails" runat="server" Text="Details"></asp:Label>
    <asp:DetailsView ID="grdBookDetails" runat="server" AutoGenerateRows="False" 
        DataKeyNames="BookID" DataSourceID="SqlDataSourceBookDetail" Height="50px" 
        Width="865px" CellPadding="4" ForeColor="#333333" GridLines="None">
        <AlternatingRowStyle BackColor="White" />
        <CommandRowStyle BackColor="#C5BBAF" Font-Bold="True" />
        <EditRowStyle BackColor="#7C6F57" />
        <FieldHeaderStyle BackColor="#D0D0D0" Font-Bold="True" />
        <Fields>
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
            <asp:BoundField DataField="YearOfPublication" HeaderText="Year Of Publication" 
                SortExpression="YearOfPublication" DataFormatString="{0:yyyy}" />
            <asp:BoundField DataField="DateRead" HeaderText="Month Read" 
                SortExpression="DateRead" DataFormatString="{0:MMMM yyyy}" />
            <asp:BoundField DataField="Synopsis" HeaderText="Synopsis" 
                SortExpression="Synopsis" />
            <asp:BoundField DataField="ChosenBy" HeaderText="Chosen By" 
                SortExpression="ChosenBy" ReadOnly="True" />
            <asp:BoundField DataField="BBCTopBookPosition" HeaderText="BBC Top 100 Rank" 
                SortExpression="BBCTopBookPosition" ReadOnly="True" />
        </Fields>
        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#E3EAEB" />
    </asp:DetailsView>
    <asp:SqlDataSource ID="SqlDataSourceBookDetail" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        SelectCommand="SELECT * FROM [BookDetail] WHERE ([BookID] = @BookID)">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="1" Name="BookID" 
                QueryStringField="SelectedBookID" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <hr />
    <asp:GridView ID="grdBookMarks" runat="server" DataSourceID="objBookMarks" AutoGenerateColumns="True" AllowSorting="True"
        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" 
        CellPadding="3" CellSpacing="1" GridLines="None"
        onrowdatabound="grdBookMarks_RowDataBound"
        ondatabound="grdBookMarks_DataBound"
        onsorting="grdBookMarks_Sorting" 
        onselectedindexchanged="grdBookMarks_SelectedIndexChanged" CssClass="markingGrid">
        <HeaderStyle CssClass="markingGridHeaderStyle" />
        <FooterStyle CssClass="markingGridFooterStyle" />
        <PagerStyle HorizontalAlign="Right" CssClass="markingGridPagerStyle" />
        <RowStyle CssClass="markingGridRowStyle" />
        <AlternatingRowStyle CssClass="markingGridRowAltStyle"/>
        <EditRowStyle CssClass="markingGridRowEditStyle" />
        <SelectedRowStyle CssClass="markingGridRowSelectedStyle" />
        <SortedAscendingCellStyle CssClass="markingGridSortedAscendingCellStyle" />
        <SortedAscendingHeaderStyle CssClass="markingGridSortedAscendingHeaderStyle" />
        <SortedDescendingCellStyle CssClass="markingGridSortedDescendingCellStyle" />
        <SortedDescendingHeaderStyle CssClass="markingGridSortedDescendingHeaderStyle" />
    </asp:GridView>
    <asp:ObjectDataSource ID="objBookMarks" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="SelectPersonBookMarks" 
        TypeName="BookgroupLibraryCode.PersonBookMarks" 
        onobjectcreating="objBookMarks_ObjectCreating">
    </asp:ObjectDataSource>
    <asp:Button ID="btnEdit" runat="server" onclick="btnEdit_Click" Text="Edit" />
		<asp:ValidationSummary ID="vsmEditState" ValidationGroup="EditState" CssClass="validationSummaryStyle" runat="server" />
    <hr />
</asp:Content>
