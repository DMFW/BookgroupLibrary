<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Statistics.aspx.cs" Inherits="BookgroupLibrary.StatisticsForm" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <h1>Library Statistics</h1>

    <div class="stats-grid">

        <div class="stats-card">
            <div class="stats-title">Total Books</div>
            <div class="stats-value">
                <asp:Label ID="lblTotalBooks" value="Test" runat="server" />
            </div>
        </div>

        <div class="stats-card">
            <div class="stats-title">Total Authors</div>
            <div class="stats-value">
                <asp:Label ID="lblTotalAuthors" runat="server" />
            </div>
        </div>

        <div class="stats-card">
            <div class="stats-title">Male Authors</div>
            <div class="stats-value">
                <asp:Label ID="lblMaleAuthors" runat="server" />
            </div>
        </div>

        <div class="stats-card">
            <div class="stats-title">Female Authors</div>
            <div class="stats-value">
                <asp:Label ID="lblFemaleAuthors" runat="server" />
            </div>
        </div>

        <div class="stats-card">
            <div class="stats-title">Oldest Publication</div>
            <div class="stats-value">
                <asp:Label ID="lblOldestPublication" runat="server" />
            </div>
        </div>

        <div class="stats-card">
            <div class="stats-title">Newest Publication</div>
            <div class="stats-value">
                <asp:Label ID="lblNewestPublication" runat="server" />
            </div>
        </div>

    </div>

    <div class="statsdetailGrid">

        <div>
        <h2>Most Popular Authors</h2>
        <asp:GridView
            ID="gvPopularAuthors"
            runat="server"
            DataSourceID="dsPopularAuthors"
            AutoGenerateColumns="False"
            CssClass="defaultgrid">
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
                <asp:BoundField DataField="AuthorRank" HeaderText="Rank">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="AuthorFullName" HeaderText="Author" />
                <asp:BoundField DataField="BookCount" HeaderText="Books Read">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        </div>
                
        <div>
        <h2>Books By Popular Authors</h2>
        <asp:GridView
            ID="gvPopularAuthorBooks"
            runat="server"
            DataSourceID="dsPopularAuthorBooks"
            AutoGenerateColumns="False"
            CssClass="defaultgrid">
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
                <asp:BoundField DataField="AuthorFullName" HeaderText="Author" />
                <asp:BoundField DataField="Title" HeaderText="Book" />
                <asp:BoundField DataField="DateRead"
                    HeaderText="Date Read"
                    DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="YearOfPublication"
                    HeaderText="Published" DataFormatString="{0:yyyy}"/>
                </Columns>

        </asp:GridView>
        </div>
    
    </div>

    <asp:SqlDataSource ID="dsPopularAuthors" runat="server" ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
    SelectCommand="SELECT ROW_NUMBER() OVER
                    (
                        ORDER BY BookCount DESC, AuthorLastName
                    ) AS AuthorRank,
                    AuthorFullName,
                    BookCount,
                    AuthorLastName
                    FROM
                    (
                    SELECT DISTINCT
                    AuthorFullName,
                    BookCount,
                    AuthorLastName
                    FROM PopularAuthorBooks
                    ) A
                    ORDER BY
                    AuthorRank;"></asp:SqlDataSource>

    <asp:SqlDataSource
    ID="dsPopularAuthorBooks"
    runat="server"
    ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
    SelectCommand="
        SELECT *
        FROM PopularAuthorBooks
        ORDER BY
            BookCount DESC,
            AuthorLastName,
            AuthorFirstName,
            DateRead ASC">
    </asp:SqlDataSource>

</asp:Content>
