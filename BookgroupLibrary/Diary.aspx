<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Diary.aspx.cs" Inherits="BookgroupLibrary.Diary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Forthcoming Meetings</h1>
    <p>The "normal" meeting for books we voted to read will be on the second Monday of the month. These dates are shown in this <span class="datemaintext">colour.</span>
    <p>A special online meeting for Christmas Secret Santa selections will be on the fourth Monday of the month, marked in this <span class="datesantatext">colour.</span>
    <p>Meetings on any other date will be shown in this <span class="datewarningtext">colour</span> and will normally have an explanation in the comments.</p>
	<asp:GridView ID="grdForthcomingMeetings" runat="server" DataSourceID="SqlDataSourceDiary" DataKeyNames="MeetingID" AutoGenerateColumns="False" Width="871px"
	OnSelectedIndexChanged="grdForthcomingMeetings_SelectedIndexChanged" OnRowDataBound="grdForthcomingMeetings_DataBound">
		<Columns>
		<asp:TemplateField ShowHeader="False" Visible="False">
			<ItemTemplate>
				<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="False" CommandName="Select" Text="Edit"></asp:LinkButton>
				<asp:LinkButton ID="lnkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>                                                    
      </ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Date">
				<ItemTemplate>
					<asp:Label ID="lblMeetingDate" runat="server" 
						Text='<%# Bind("MeetingDate", "{0:d}") %>'></asp:Label>
				</ItemTemplate>
			</asp:TemplateField>
			<asp:BoundField DataField="Leader" HeaderText="Leader" ReadOnly="True" />
			<asp:BoundField DataField="Title" HeaderText="Book" ReadOnly="True" />
			<asp:BoundField DataField="Author" HeaderText="Author(s)" ReadOnly="True"/>
			<asp:BoundField DataField="Chooser" HeaderText="Chooser" ReadOnly="True" />
			<asp:BoundField DataField="Comments" HeaderText="Comments" ReadOnly="True" />
			<asp:TemplateField ShowHeader="True" HeaderText="LeaderID" Visible="False">
					<ItemTemplate>
            <asp:Label ID="lblLeaderID" runat="server" Text='<%# Eval("LeaderID") %>'></asp:Label>
          </ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField ShowHeader="True" HeaderText="BookID" Visible="False">
					<ItemTemplate>
            <asp:Label ID="lblBookID" runat="server" Text='<%# Eval("BookID") %>'></asp:Label>
          </ItemTemplate>
			</asp:TemplateField>
		</Columns>
		<SelectedRowStyle CssClass="SelectedRowStyle" />
	</asp:GridView>
	<asp:Button ID="btnAddMeeting" runat="server" Text="Add Meeting" 
		Visible="false" onclick="btnAddMeeting_Click"/>
	<asp:Panel ID="pnlEditingArea" runat="server" Visible="false">
		<h2><asp:Label ID="lblEditType" runat="server" Text=""></asp:Label></h2>
		<asp:DetailsView ID="dtvMeeting" runat="server" AutoGenerateRows="False"
			DataSourceID="SqlDataSourceDiaryEntry" Height="50px" Width="873px" 
			OnPreRender="dtvMeeting_PreRender" 
			OnItemInserting="dtvMeeting_ItemInserting"
			OnItemUpdating="dtvMeeting_ItemUpdating"
			EnableViewState="false">
			<Fields>
				<asp:TemplateField HeaderText="ID" InsertVisible="False">
					<ItemTemplate>
						<asp:Label ID="lblMeetingID" runat="server" Text='<%# Bind("MeetingID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Meeting Date" SortExpression="MeetingDate">
					<EditItemTemplate>
						<asp:TextBox ID="txtMeetingDate" runat="server" columns="10" 
							Text='<%# Bind("MeetingDate", "{0:d}") %>'></asp:TextBox>
						<asp:CustomValidator ID="valMeetingDate" runat="server" 
							ControlToValidate="txtMeetingDate" Display="None" 
							OnServerValidate="valMeetingDate_ServerValidate" ValidationGroup="Meeting" />
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:TextBox ID="txtMeetingDate" runat="server" columns="10" 
							Text='<%# Bind("MeetingDate", "{0:d}") %>'></asp:TextBox>
						<asp:CustomValidator ID="valMeetingDate" runat="server" 
							ControlToValidate="txtMeetingDate" Display="None" 
							OnServerValidate="valMeetingDate_ServerValidate" ValidationGroup="Meeting" />
					</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Meeting Leader" SortExpression="Leader">
					<ItemTemplate>
						<asp:Label ID="lblMeetingDate" runat="server" 
							Text='<%# Bind("MeetingDate", "{0:d}") %>'></asp:Label>
					</ItemTemplate>
					<EditItemTemplate>
							<asp:DropDownList ID="ddlLeader" runat="server" 
								OnDataBound="ddlListRequiringUnselectedItem_DataBound" AppendDataBoundItems="true"
									DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" 
								DataValueField="GroupMemberID">
							</asp:DropDownList>
					</EditItemTemplate>
					<InsertItemTemplate>
							<asp:DropDownList ID="ddlLeader" runat="server" 
								OnDataBound="ddlListRequiringUnselectedItem_DataBound" AppendDataBoundItems="true"
									DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" 
								DataValueField="GroupMemberID">
							</asp:DropDownList>
					</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Book Title" SortExpression="Title">
					<ItemTemplate>
						<asp:Label ID="lblLeader" runat="server" Text='<%# Bind("Leader") %>'></asp:Label>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:DropDownList ID="ddlBook" runat="server" AppendDataBoundItems="true" 
							DataSourceID="SqlOurBooks" DataTextField="Title" DataValueField="BookID" 
							OnDataBound="ddlListRequiringUnselectedItem_DataBound">
						</asp:DropDownList>
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:DropDownList ID="ddlBook" runat="server" AppendDataBoundItems="true" 
							DataSourceID="SqlOurBooks" DataTextField="Title" DataValueField="BookID" 
							OnDataBound="ddlListRequiringUnselectedItem_DataBound">
						</asp:DropDownList>
					</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Comments" SortExpression="Comments">
					<ItemTemplate>
						<asp:Label ID="lblTitle" runat="server" Text='<%# Bind("Title") %>'></asp:Label>
					</ItemTemplate>
					<ItemTemplate>
						<asp:Label ID="lblComments" runat="server" Text='<%# Bind("Comments") %>'></asp:Label>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:TextBox ID="txtComments" runat="server" columns="100" 
							Text='<%# Bind("Comments") %>' TextMode="multiline"></asp:TextBox>
					</EditItemTemplate>
					<InsertItemTemplate>
						<asp:TextBox ID="txtComments" runat="server" columns="100" 
							Text='<%# Bind("Comments") %>' TextMode="multiline"></asp:TextBox>
					</InsertItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="LeaderID" SortExpression="LeaderID" Visible="False">
					<ItemTemplate>
						<asp:Label ID="lblLeaderID" runat="server" Text='<%# Bind("LeaderID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="ChooserID" SortExpression="ChooserID" Visible="False">
					<ItemTemplate>
						<asp:Label ID="lblChooserID" runat="server" Text='<%# Bind("ChooserID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="BookID" SortExpression="BookID" Visible="False">
					<ItemTemplate>
						<asp:Label ID="lblBookID" runat="server" Text='<%# Bind("BookID") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
			</Fields>
		</asp:DetailsView>

		<asp:ValidationSummary ID="vsmDetailView" ValidationGroup="Meeting" runat="server" ForeColor="Red"/>

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

	<asp:SqlDataSource ID="SqlDataSourceDiary" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [MeetingView] WHERE MeetingDate >= DATEADD(day, -1, convert(date, GETDATE())) OR MeetingDate Is Null Order By SortMeetingDate, MeetingID"
		DeleteCommand="DELETE [Meetings] WHERE [MeetingID] = @MeetingID"
		OnDeleting="SqlDataSourceDiary_Deleting">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="SqlDataSourceDiaryEntry" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT * FROM [MeetingView] WHERE ([MeetingID] = @MeetingID)"
		UpdateCommand="UPDATE [Meetings] Set [MeetingDate] = @MeetingDate, [BookID] = @BookID, [LeaderID] = @LeaderID, [Comments] = @Comments WHERE ([MeetingID] = @MeetingID)"
		InsertCommand="INSERT INTO Meetings (MeetingDate, BookID, LeaderID, Comments) VALUES (@MeetingDate, @BookID, @LeaderID, @Comments); SELECT @MeetingID = SCOPE_IDENTITY()"
		OnInserting="SqlDataSourceDiaryEntry_Inserting"
		OnInserted="SqlDataSourceDiaryEntry_Inserted"
		OnUpdating="SqlDataSourceDiaryEntry_Updating"
		OnUpdated="SqlDataSourceDiaryEntry_Updated">
			<SelectParameters>
				<asp:ControlParameter ControlID="grdForthcomingMeetings" Name="MeetingID" 
					PropertyName="SelectedValue" Type="Int32" />
			</SelectParameters>
			<UpdateParameters>
					<asp:Parameter Type="DateTime" Name="MeetingDate" ConvertEmptyStringToNull="true"></asp:Parameter>   
          <asp:Parameter Type="Int16" Name="BookID" ConvertEmptyStringToNull="true"></asp:Parameter>
          <asp:Parameter Type="Int16" Name="LeaderID" ConvertEmptyStringToNull="true"></asp:Parameter>
          <asp:Parameter Type="String" Name="Comments"></asp:Parameter>
					<asp:Parameter Type="Int16" Name="MeetingID"></asp:Parameter>
      </UpdateParameters>
      <InsertParameters>
          <asp:Parameter Type="DateTime" Name="MeetingDate" ConvertEmptyStringToNull="true"></asp:Parameter>   
          <asp:Parameter Type="Int16" Name="BookID" ConvertEmptyStringToNull="true"></asp:Parameter>
          <asp:Parameter Type="Int16" Name="LeaderID" ConvertEmptyStringToNull="true"></asp:Parameter>
          <asp:Parameter Type="String" Name="Comments"></asp:Parameter>
          <asp:Parameter Type="Int16" Name="MeetingID" Direction="Output" DefaultValue="0" />
      </InsertParameters>
		</asp:SqlDataSource>
	
	<asp:SqlDataSource ID="SqlAllGroupMembers" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT [GroupMemberID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [GroupMember] ORDER BY [LastName], [FirstName]">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="SqlOurBooks" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT [Title], [BookID], [Author] FROM [OurList] ORDER BY [Title] ">
	</asp:SqlDataSource>

</asp:Content>
