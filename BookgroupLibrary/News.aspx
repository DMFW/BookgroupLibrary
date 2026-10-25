<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="BookgroupLibrary.News" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Bookgroup News</h1>
	<asp:GridView ID="grdNews" runat="server" DataSourceID="SqlDataSourceNews" DataKeyNames="NewsID" AutoGenerateColumns="False" Width="871px"
	OnSelectedIndexChanged="grdNews_SelectedIndexChanged" OnRowDataBound="grdNews_DataBound">
		<Columns>
			<asp:TemplateField ShowHeader="False" Visible="False">
				<ItemTemplate>
					<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="False" CommandName="Select" Text="Edit"></asp:LinkButton>
					<asp:LinkButton ID="lnkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Date Posted">
				<ItemTemplate>
				<asp:Label ID="lblDatePosted" runat="server" 
					Text='<%# Bind("DatePosted", "{0:d}") %>'></asp:Label>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="News">
                <ItemTemplate>
                    <asp:Label ID="lblDescription"
                        runat="server"
                        CssClass="preserve-lines"
                        Text='<%# Eval("NewsText") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
		<SelectedRowStyle CssClass="SelectedRowStyle" />
	</asp:GridView>
	<asp:Button ID="btnAddNews" runat="server" Text="Add News" 
		Visible="false" onclick="btnAddNews_Click"/>
	<asp:Panel ID="pnlEditingArea" runat="server" Visible="false">
		<h2><asp:Label ID="lblEditType" runat="server" Text=""></asp:Label></h2>
		<asp:DetailsView ID="dtvNews" runat="server" AutoGenerateRows="False"
			DataSourceID="SqlDataSourceNewsEntry" Height="50px" Width="873px" 
			OnPreRender="dtvNews_PreRender" 
			OnItemInserting="dtvNews_ItemInserting"
			OnItemUpdating="dtvNews_ItemUpdating"
			EnableViewState="false">
			<Fields>
				<asp:TemplateField HeaderText="ID" InsertVisible="False">
					<ItemTemplate>
						<asp:Label ID="lblNewsID" runat="server" Text='<%# Bind("NewsID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Date Posted" SortExpression="DatePosted">
					<EditItemTemplate>
					<asp:TextBox ID="txtDatePosted" runat="server" columns="10" 
						Text='<%# Bind("DatePosted", "{0:d}") %>'></asp:TextBox>
					<asp:CustomValidator ID="valDatePosted" runat="server" 
						ControlToValidate="txtDatePosted" Display="None" 
						OnServerValidate="valDatePosted_ServerValidate" ValidationGroup="Meeting" />
					</EditItemTemplate>
					<InsertItemTemplate>
					<asp:TextBox ID="txtDatePosted" runat="server" columns="10" 
						Text='<%# Bind("DatePosted", "{0:d}") %>'></asp:TextBox>
					<asp:CustomValidator ID="CustomValidator1" runat="server" 
						ControlToValidate="txtDatePosted" Display="None" 
						OnServerValidate="valDatePosted_ServerValidate" ValidationGroup="Meeting" />
					</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="News" SortExpression="NewsText">
					<EditItemTemplate>
						<asp:TextBox ID="txtNewsText" runat="server" columns="100" 
							Text='<%# Bind("NewsText") %>' TextMode="multiline"></asp:TextBox>
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:TextBox ID="txtNewsText" runat="server" columns="100" 
							Text='<%# Bind("NewsText") %>' TextMode="multiline"></asp:TextBox>
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

	<asp:SqlDataSource ID="SqlDataSourceNews" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [News] Order By DatePosted"
		UpdateCommand="UPDATE [News] Set [DatePosted] = @DatePosted, [NewsText]=@NewsText WHERE ([NewsID] = @NewsID)"
		DeleteCommand="DELETE [News] WHERE [NewsID] = @NewsID"
		OnDeleting="SqlDataSourceNews_Deleting">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="SqlDataSourceNewsEntry" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [News] WHERE ([NewsID] = @NewsID)"
		UpdateCommand="UPDATE [News] Set [DatePosted] = @DatePosted, [NewsText]=@NewsText WHERE ([NewsID] = @NewsID)"
		InsertCommand="INSERT INTO News (DatePosted, NewsText) VALUES (@DatePosted, @NewsText); SELECT @NewsID = SCOPE_IDENTITY()"
		OnInserting="SqlDataSourceNewsEntry_Inserting"
		OnInserted="SqlDataSourceNewsEntry_Inserted"
		OnUpdating="SqlDataSourceNewsEntry_Updating"
		OnUpdated="SqlDataSourceNewsEntry_Updated">
		<SelectParameters>
			<asp:ControlParameter ControlID="grdNews" Name="NewsID" PropertyName="SelectedValue" Type="Int32" />
		</SelectParameters>
		<UpdateParameters>
			<asp:Parameter Type="Int16" Name="NewsID"></asp:Parameter>
			<asp:Parameter Type="DateTime" Name="DatePosted" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="NewsText" ConvertEmptyStringToNull="true"></asp:Parameter>
		</UpdateParameters>
		<InsertParameters>
			<asp:Parameter Type="Int16" Name="NewsID" Direction="Output" DefaultValue="0" />
			<asp:Parameter Type="DateTime" Name="DatePosted" ConvertEmptyStringToNull="true"></asp:Parameter>
			<asp:Parameter Type="String" Name="NewsText" ConvertEmptyStringToNull="true"></asp:Parameter>
		</InsertParameters>
	</asp:SqlDataSource>
	
</asp:Content>
