<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Links.aspx.cs" Inherits="BookgroupLibrary.Links" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Bookgroup External Site Links</h1>
	<asp:GridView ID="grdLinks" runat="server" DataSourceID="SqlDataSourceLinks" DataKeyNames="LinkID" AutoGenerateColumns="False" Width="871px"
	OnSelectedIndexChanged="grdLinks_SelectedIndexChanged" OnRowDataBound="grdLinks_DataBound">
		<Columns>
			<asp:TemplateField ShowHeader="False" Visible="False">
				<ItemTemplate>
					<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="False" CommandName="Select" Text="Edit"></asp:LinkButton>
					<asp:LinkButton ID="lnkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="External Link">
				<ItemTemplate>
				<asp:HyperLink ID="hlLink"
							runat="server"
							Text='<%# Eval("LinkText") %>'
							NavigateUrl='<%# Eval("LinkURL") %>' 
							Target="_blank"
							rel="noopener noreferrer" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField DataField="LinkDescription" HeaderText="Site Description" ReadOnly="True" />
		</Columns>
		<SelectedRowStyle CssClass="SelectedRowStyle" />
	</asp:GridView>
	<asp:Button ID="btnAddLink" runat="server" Text="Add Link" 
		Visible="false" onclick="btnAddLink_Click"/>
	<asp:Panel ID="pnlEditingArea" runat="server" Visible="false">
		<h2><asp:Label ID="lblEditType" runat="server" Text=""></asp:Label></h2>
		<asp:DetailsView ID="dtvLink" runat="server" AutoGenerateRows="False"
			DataSourceID="SqlDataSourceLinkEntry" Height="50px" Width="873px" 
			OnPreRender="dtvLink_PreRender" 
			OnItemInserting="dtvLink_ItemInserting"
			OnItemUpdating="dtvLink_ItemUpdating"
			EnableViewState="false">
			<Fields>
				<asp:TemplateField HeaderText="ID" InsertVisible="False">
					<ItemTemplate>
						<asp:Label ID="lblLinkID" runat="server" Text='<%# Bind("LinkID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Link">
					<ItemTemplate>
						<asp:HyperLink ID="hlLink"
							runat="server"
							Text='<%# Eval("LinkText") %>'
							NavigateUrl='<%# Eval("LinkURL") %>' 
							Target="_blank"
							rel="noopener noreferrer"/>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:TextBox ID="txtLinkText" runat="server" columns="20" 
							Text='<%# Bind("LinkText") %>'></asp:TextBox>
						<asp:TextBox ID="txtLinkURL" runat="server" columns="80" 
							Text='<%# Bind("LinkURL") %>'></asp:TextBox>
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:TextBox ID="txtLinkText" runat="server" columns="20" 
							Text='<%# Bind("LinkText") %>'></asp:TextBox>
						<asp:TextBox ID="txtLinkURL" runat="server" columns="80" 
							Text='<%# Bind("LinkURL") %>'></asp:TextBox>
					</InsertItemTemplate>
				</asp:TemplateField>
					<asp:TemplateField HeaderText="Link Description" SortExpression="LinkDescription">
						<ItemTemplate>
							<asp:Label ID="lblLinkDescription" runat="server" Text='<%# Bind("LinkDescription") %>'></asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="txtLinkDescription" runat="server" columns="100" 
								Text='<%# Bind("LinkDescription") %>' TextMode="multiline"></asp:TextBox>
						</EditItemTemplate>
						<InsertItemTemplate>
							<asp:TextBox ID="txtLinkDescription" runat="server" columns="100" 
								Text='<%# Bind("LinkDescription") %>' TextMode="multiline"></asp:TextBox>
						</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="DisplayRank" SortExpression="DisplayRank">
					<ItemTemplate>
						<asp:Label ID="lbDisplayRank" runat="server" Text='<%# Bind("DisplayRank") %>'></asp:Label>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:TextBox ID="txtDisplayRank" runat="server" columns="100" 
							Text='<%# Bind("DisplayRank") %>'></asp:TextBox>
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:TextBox ID="txtDisplayRank" runat="server" columns="100" 
							Text='<%# Bind("DisplayRank") %>'></asp:TextBox>
					</InsertItemTemplate>
				</asp:TemplateField>
			</Fields>
		</asp:DetailsView>

		<table>
			<tr>
				<td>
					<asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click"/>
				</td>
				<td>
					<asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"/>
				</td>
			</tr>
		</table>
	</asp:Panel>

	<asp:SqlDataSource ID="SqlDataSourceLinks" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [Links] Order By DisplayRank"
		UpdateCommand="UPDATE [Links] Set [LinkText]=@LinkText, [LinkURL] = @LinkURL, [LinkDescription] = @LinkDescription, [DisplayRank] = @DisplayRank WHERE ([LinkID] = @LinkID)"
		DeleteCommand="DELETE [Links] WHERE [LinkID] = @LinkID"
		OnDeleting="SqlDataSourceLinks_Deleting">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="SqlDataSourceLinkEntry" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [Links] WHERE ([LinkID] = @LinkID)"
		UpdateCommand="UPDATE [Links] Set [LinkText]=@LinkText, [LinkURL] = @LinkURL, [LinkDescription] = @LinkDescription, [DisplayRank] = @DisplayRank WHERE ([LinkID] = @LinkID)"
		InsertCommand="INSERT INTO Links (LinkText, LinkURL, LinkDescription, DisplayRank) VALUES (@LinkText, @LinkURL, @LinkDescription,@DisplayRank); SELECT @LinkID = SCOPE_IDENTITY()"
		OnInserting="SqlDataSourceLinkEntry_Inserting"
		OnInserted="SqlDataSourceLinkEntry_Inserted"
		OnUpdating="SqlDataSourceLinkEntry_Updating"
		OnUpdated="SqlDataSourceLinkEntry_Updated">
		<SelectParameters>
			<asp:ControlParameter ControlID="grdLinks" Name="LinkID" PropertyName="SelectedValue" Type="Int32" />
		</SelectParameters>
		<UpdateParameters>
			<asp:Parameter Type="Int16" Name="LinkID"></asp:Parameter>
			<asp:Parameter Type="String" Name="LinkText" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="LinkURL" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="LinkDescription" ConvertEmptyStringToNull="true"></asp:Parameter> 
			<asp:Parameter Type="Int16" Name="DisplayRank"></asp:Parameter>
		</UpdateParameters>
		<InsertParameters>
			<asp:Parameter Type="Int16" Name="LinkID" Direction="Output" DefaultValue="0" />
			<asp:Parameter Type="String" Name="LinkText" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="LinkURL" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="LinkDescription" ConvertEmptyStringToNull="true"></asp:Parameter> 
			<asp:Parameter Type="Int16" Name="DisplayRank"></asp:Parameter>
		</InsertParameters>
	</asp:SqlDataSource>
	
</asp:Content>
