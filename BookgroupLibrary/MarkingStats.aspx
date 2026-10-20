<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MarkingStats.aspx.cs" Inherits="BookgroupLibrary.MarkingStatsForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DropDownList ID="ddlMemberID" OnSelectedIndexChanged="ddlMemberID_SelectedIndexChanged" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    <asp:GridView ID="grdStatsBookMarkView" runat="server" DataSourceID="objStatsBookMarks" AutoGenerateColumns="True" AllowSorting="True"
        BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" 
        CellPadding="3" CellSpacing="1" GridLines="None"
        onrowdatabound="grdStatsBookMarkView_RowDataBound"
        ondatabound="grdStatsBookMarkView_DataBound"
        onsorting="grdStatsBookMarkView_Sorting" 
        onselectedindexchanged="grdStatsBookMarkView_SelectedIndexChanged">
        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />
        <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle BackColor="#DEDFDE" ForeColor="Black" />
        <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#594B9C" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#33276A" />
    </asp:GridView>
    <asp:ObjectDataSource ID="objStatsBookMarks" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="SelectStatsBookMarks" 
        TypeName="BookgroupLibraryCode.StatsBookMarks" 
        onobjectcreating="objStatsBookMarks_ObjectCreating">
    </asp:ObjectDataSource>
</asp:Content>
