<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="BookgroupLibrary._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        This is the web site of the Southend-On-Sea &quot;big readers&quot; book group. 
        The group was originally started in 2003 by friends working for a software company called 
        Rebus (now part of Xchanging) and inspired by a BBC series called the &quot;Big Read&quot; 
        which was shown on television that year. The BBC conducted a poll open to all ages and 
        types of readers to find the UK&#39;s top 100 favourite works of fiction. More 
        details about the BBC project can be found here:-
        <a href="http://www.bbc.co.uk/arts/bigread/top100.shtml" name="BBCBigRead">BBC Big Read</a></p>
    <p>
        Our group has been meeting once a month, every month since then to discuss one (or 
        very occasionally more) of the top 100 books and in June 2011 we were able to celebrate
				the completion of the original list and this web site was launched. Since then we have been striking
				out on our own with some self selected titles. Members of the group take turns to offer five 
				choices to the others and the rules we are currently following are that at least one must be
				a pre 20th century work and at least one a book which the chooser has not read. We all vote
				on the options and a diferent group member is elected to lead the meeting for the selected book
				to mix things up a bit.
    </p> 
    <p> We meet in various locations but usually in Leigh-On-Sea despite the name of the group!
				The &quot;leader&quot; for the monthly session begins with a brief biography of the author and
				some background about the book before opening up the discussion. At the end of the meeting we
				all score the book based on a number of literary qualities. Then we select the next book, usually
				following the plan explained above but sometimes by agreement of the whole group if there is
				some special reason we all would like to read a book. We try to maintain a rolling programme
				with a three month horizon. These are the categories we use to score books we have read:-</p>
    <asp:GridView 
        ID="grdCategories" runat="server" DataSourceID="SqlDataSourceMarkingCategory" 
        AutoGenerateColumns="False" CellPadding="4" 
        GridLines="None" CssClass="defaultgrid">
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
            <asp:BoundField DataField="CategoryDescription" HeaderText="Scoring Category" 
                SortExpression="CategoryDescription" />
            <asp:BoundField DataField="CategoryNotes" HeaderText="Notes" 
                SortExpression="CategoryNotes" />
        </Columns>
    </asp:GridView>
    <p>
        Although we have had a number of people leave and join us over the years the policy of the group has
        been to open it by invitation only for interested family and friends known to existing group members.
        This is mainly because there is a limit to the number of people who can fit round a table and all contribute
        without the discussion fragmenting. We are not actively seeking any new members at the moment. 
    </p>
    <p> 
        If you are reading this and thinking you might like to join a group, you can find information about public groups here:-
        <a href="http://www.bookgroup.info/041205/index.php" name="PublicBookGroups">Public Book Groups</a></p>
    <p>
        The purpose of this web site is to record the group history and our own scoring 
        for the interest of members. A major new release of the site database and software has just been
				launched at the time of writing allowing all group members to now add and edit the books and to update
				their own scores (previously this had to be done by the site administrator).</p>
    <p>
        David Worton - 14th June : 2012 </p>
    <asp:SqlDataSource ID="SqlDataSourceMarkingCategory" runat="server" 
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
        ProviderName="<%$ ConnectionStrings:SouthendBookgroupConnectionString.ProviderName %>" 
        
        SelectCommand="SELECT [CategoryID], [CategoryDescription], [CategoryNotes] FROM [MarkingCategory] ORDER BY [CategoryID]">
    </asp:SqlDataSource>
</asp:Content>
