<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkingStats.aspx.cs" Inherits="BookgroupLibrary.MarkingStatsForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DropDownList ID="ddlMemberID" OnSelectedIndexChanged="ddlMemberID_SelectedIndexChanged" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <asp:GridView ID="grdStatsBookMarkView" runat="server" DataSourceID="objStatsBookMarks" AutoGenerateColumns="True" AllowSorting="True"
        BorderStyle="Ridge" BorderWidth="2px" 
        CellPadding="3" CellSpacing="1" GridLines="None"
        onrowdatabound="grdStatsBookMarkView_RowDataBound"
        ondatabound="grdStatsBookMarkView_DataBound"
        onsorting="grdStatsBookMarkView_Sorting" 
        onselectedindexchanged="grdStatsBookMarkView_SelectedIndexChanged" CssClass="markingGrid">
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
    <asp:ObjectDataSource ID="objStatsBookMarks" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="SelectStatsBookMarks" 
        TypeName="BookgroupLibraryCode.StatsBookMarks" 
        onobjectcreating="objStatsBookMarks_ObjectCreating">
    </asp:ObjectDataSource>
</asp:Content>
