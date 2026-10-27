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

</asp:Content>
