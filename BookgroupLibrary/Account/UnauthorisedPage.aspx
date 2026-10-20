<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnauthorisedPage.aspx.cs" Inherits="BookgroupLibrary.Account.UnauthorisedPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<asp:Panel ID="pnlErrorText" runat="server">
	<p class="errortext">You were not authorised to view the requested page.</p>
	<p></p>
	<p class="errortext">Please sign on with an account which has the required permissions or browse to another page.</p>
	<p></p>
	<asp:Label ID="lblSpecificRedirectionError" runat="server" class="errortext" Text=""></asp:Label>
	</asp:Panel>
</asp:Content>
