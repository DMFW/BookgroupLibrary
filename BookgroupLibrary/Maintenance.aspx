<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Maintenance.aspx.cs" Inherits="BookgroupLibrary.Maintenance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <asp:Button ID="btnGroupMembers" runat="server" Text="Edit Group Members" 
                    Width="150px" onclick="btnGroupMembers_Click" />
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btnAuthors" runat="server" Text="Edit Authors" 
										Width="150px" onclick="btnAuthors_Click"/>
                &nbsp;
            </td>
            <td>
                <asp:Button ID="btnBooks" runat="server" Text="Add Books" 
										Width="150px" onclick="btnBooks_Click" />
                &nbsp;
            </td>
        </tr>
				<tr>
				<td>
					<img src="Images/Group.gif" /></td>
				<td>
					<img src="Images/Authors.gif" /></td>
				<td>
					<img src="Images/Books.gif" /></td>
				</tr>
    </table>
</asp:Content>
