<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MaintainBooks.aspx.cs" Inherits="BookgroupLibrary.MaintainBooks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h1><asp:Label ID="lblMaintenanceMode" runat="server" Text="" /></h1>
	<table>
		<tr>
			<td><asp:Label ID="lblSelectionMethod" runat="server" Text="Selection Method"></asp:Label></td>
			<td>
				<asp:DropDownList ID="ddlSelectionMethod" runat="server" AutoPostBack="True" 
					DataSourceID="SqlDataSourceSelectionMethod" DataTextField="SelectionMethod" 
					DataValueField="SelectionMethodType">
				</asp:DropDownList>
				<asp:SqlDataSource ID="SqlDataSourceSelectionMethod" runat="server" 
					ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
					SelectCommand="SELECT [SelectionMethodType], [SelectionMethod] FROM [SelectionMethod]">
				</asp:SqlDataSource>
			</td>
			<td><asp:Label ID="lblSelectionValueText" runat="server" Text=""></asp:Label></td>
			<td>
				<asp:DropDownList ID="ddlGroupMembers" runat="server" 
					DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
				</asp:DropDownList>
				<asp:TextBox ID="txtTop100Position" runat="server"></asp:TextBox>
			</td>
		</tr>
	</table>
	<asp:ValidationSummary ID="vsmSelectionMethod"  ValidationGroup="SelectionMethod" runat="server" ForeColor="Red"/>
	<asp:Label ID="lblAuthors" runat="server" Text="Author(s)"></asp:Label>
  <asp:GridView ID="grdBookAuthors" runat="server" AutoGenerateColumns="False" ShowFooter="true" 
      HeaderStyle-HorizontalAlign="Left" CellPadding="4" ForeColor="green" GridLines="None" BackColor="White" 
			OnRowDeleting="grdBookAuthors_RowDelete" >
			<EmptyDataTemplate>
				<asp:Button ID="btnAddFirstAuthor" runat="server" Text="Add Author" onclick="btnAddAuthor_Click" />
				<asp:DropDownList ID="ddlFirstAuthor" runat="server" DataSourceID="SqlDataSourceAllAuthors" 
					DataTextField="DisplayName" DataValueField="AuthorID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
				</asp:DropDownList>
      </EmptyDataTemplate>
			<RowStyle BackColor="#E3EAEB" />
			<AlternatingRowStyle BackColor="White" />
      <Columns>
					<asp:TemplateField ShowHeader="False">
						<ItemTemplate>
							<asp:LinkButton ID="lnkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>                          
            </ItemTemplate>        
          </asp:TemplateField>
					<asp:TemplateField HeaderText="ID" SortExpression="AuthorID">
            <ItemTemplate>
                <asp:Label ID="lblAuthorID" runat="server" Text='<%# Eval("AuthorID") %>'></asp:Label>
            </ItemTemplate>
						<FooterTemplate>
							<asp:Button ID="btnAddAuthor" runat="server" Text="Add Author" onclick="btnAddAuthor_Click" />
						</FooterTemplate>		
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Forename" SortExpression="FirstName">
            <ItemTemplate>
                <asp:Label ID="lblFirstName" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
            </ItemTemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Surname" SortExpression="LastName">
            <ItemTemplate>
							<asp:Label ID="lbllastName" runat="server" Text='<%# Eval("LastName") %>'></asp:Label>
            </ItemTemplate>
						<FooterTemplate>
							<asp:DropDownList ID="ddlMultipleAuthors" runat="server" DataSourceID="SqlDataSourceAllAuthors" 
								DataTextField="DisplayName" DataValueField="AuthorID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
							</asp:DropDownList>
            </FooterTemplate>		
          </asp:TemplateField>
      </Columns>
      <EditRowStyle BackColor="#7C6F57" />
      <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
      <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
      <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
      <RowStyle BackColor="#E3EAEB" />
      <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
      <SortedAscendingCellStyle BackColor="#F8FAFA" />
      <SortedAscendingHeaderStyle BackColor="#246B61" />
      <SortedDescendingCellStyle BackColor="#D4DFE1" />
      <SortedDescendingHeaderStyle BackColor="#15524A" />
  </asp:GridView>
	<asp:SqlDataSource ID="SqlDataSourceAllAuthors" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT [AuthorID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [Author] ORDER BY [LastName], [FirstName]">
	</asp:SqlDataSource>
	<asp:ValidationSummary ID="vsmSelectAuthor" ValidationGroup="Authors" ForeColor="Red" runat="server" />
	<asp:DetailsView ID="dtvBook" runat="server" Height="50px" Width="100%" 
		AutoGenerateRows="False" CellPadding="4" ForeColor="green" GridLines="None"
		OnItemInserting="dtvBook_ItemInserting" OnItemUpdating = "dtvBook_ItemUpdating">
		<AlternatingRowStyle BackColor="White" />
		<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
		<PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
		<RowStyle BackColor="#CEF6CE" />
		<EditRowStyle BackColor="#CEF6CE" />
		<FieldHeaderStyle BackColor="#D0FA58" Font-Bold="True" Width="20%"/>
		<Fields>
			<asp:TemplateField HeaderText="Book ID">
				<InsertItemTemplate>
						<asp:Label ID="lblBookID" runat="server" Text="Auto Generated"></asp:Label>
				</InsertItemTemplate>
				<EditItemTemplate>
            <asp:Label ID="lblBookID" runat="server" Text='<%# Eval("BookID") %>'></asp:Label>
        </EditItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Title" ItemStyle-Width="100%">
        <InsertItemTemplate>
            <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title") %>' Width="100%"></asp:TextBox>
						<asp:ValidationSummary ID="vsmTitle"  ValidationGroup="Title" runat="server" ForeColor="Red"/>
        </InsertItemTemplate>
        <EditItemTemplate>
            <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title") %>' Width="100%"></asp:TextBox>
						<asp:ValidationSummary ID="vsmTitle"  ValidationGroup="Title" runat="server" ForeColor="Red"/>
        </EditItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Year of Publication">
				<InsertItemTemplate>
					<asp:TextBox ID="txtYearOfPublication" runat="server" Text='<%# Bind("YearOfPublication") %>'></asp:TextBox>
					<asp:ValidationSummary ID="vsmYearOfPublication"  ValidationGroup="YearOfPublication" runat="server" ForeColor="Red"/>
        </InsertItemTemplate>     
				<EditItemTemplate>
           <asp:TextBox ID="txtYearOfPublication" runat="server" Text='<%# Bind("YearOfPublication", "{0:yyyy}") %>'></asp:TextBox>
					 <asp:ValidationSummary ID="vsmYearOfPublication"  ValidationGroup="YearOfPublication" runat="server" ForeColor="Red"/>
        </EditItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Date Read">
				<InsertItemTemplate>
					<asp:TextBox ID="txtDateRead" runat="server" Text='<%# Bind("DateRead", "{0:d}") %>'></asp:TextBox>
					<asp:ValidationSummary ID="vsmDateRead"  ValidationGroup="DateRead" runat="server" ForeColor="Red"/>
        </InsertItemTemplate>        
				<EditItemTemplate>
					<asp:TextBox ID="txtDateRead" runat="server" Text='<%# Bind("DateRead", "{0:d}") %>'></asp:TextBox>
					<asp:ValidationSummary ID="vsmDateRead"  ValidationGroup="DateRead" runat="server" ForeColor="Red"/>
        </EditItemTemplate>
			</asp:TemplateField>
		</Fields>
	</asp:DetailsView>
	<asp:Label ID="lblSynsopsis" runat="server" Text="Synopsis"></asp:Label>
	<br />
	<asp:TextBox ID="txtSynopsis" runat="server" TextMode="multiline" Wrap="true" 
		Rows="10" Width="100%"></asp:TextBox>
	<table>
		<tr>
			<td><asp:Button ID="btnSave" runat="server" Text="" onclick="btnSave_Click"/></td>
			<td><asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" /></td>
		</tr>
	</table>
	<asp:ValidationSummary ID="vsmUpdateDatabase"  ValidationGroup="Save" runat="server" ForeColor="Red"/>
	
	<hr />

	<h2><asp:Label ID="lblScores" runat="server" Text="Our Scores"></asp:Label></h2>
	<asp:Label ID="lblScoreMaintenanceInformation" runat="server" Text="Group member scores can be entered for a book once it has been added by selecting it for editing." />
	<asp:Panel ID="pnlBookScores" runat="server">
	<asp:GridView ID="grdBookMarks" runat="server" DataSourceID="objBookMarks" ShowFooter="True" 
      BackColor="White" BorderStyle="Ridge" BorderWidth="2px" 
			CellPadding="3" CellSpacing="1" GridLines="None"
      ondatabound="grdBookMarks_DataBound"
			onrowdatabound="grdBookMarks_RowDataBound"
			onrowupdating="grdBookMarks_RowUpdating"
			onrowupdated="grdBookMarks_RowUpdated"
			onrowdeleting="grdBookMarks_RowDeleting"
			onrowdeleted="grdBookMarks_RowDeleted"
			AutoGenerateColumns="False" CssClass="markingGrid">
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
			<EmptyDataTemplate>
				<asp:Button ID="btnAddFirstScorer" runat="server" Text="Add Scorer" onclick="btnAddScorer_Click" />
				<asp:DropDownList ID="ddlFirstScorer" runat="server" OnLoad="ddlScorer_Load"
						DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
				</asp:DropDownList>
			</EmptyDataTemplate>
			<Columns>
				<asp:TemplateField ShowHeader="False">
					<ItemTemplate>
						<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>  
						<asp:LinkButton ID="lnkButtonDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
					</ItemTemplate>
					<EditItemTemplate>
						<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>
						&nbsp;
						<asp:LinkButton ID="lnkButtonCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
					</EditItemTemplate>
					<FooterTemplate>
						<asp:Button ID="btnAddScorer" runat="server" Text="Add Scorer" onclick="btnAddScorer_Click"/>
					</FooterTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="GroupMember">
					<ItemTemplate>
              <asp:Label ID="lblGroupMember" runat="server" Text='<%# Eval("GroupMember") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:DropDownList ID="ddlScorer" runat="server" OnLoad="ddlScorer_Load" 
								DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true">
						</asp:DropDownList>
					</EditItemTemplate>
					<FooterTemplate>
						<asp:DropDownList ID="ddlNewScorer" runat="server" OnLoad="ddlScorer_Load"
							DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
						</asp:DropDownList>
					</FooterTemplate>		
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Plot">
					<ItemTemplate>
            <asp:Label ID="lbl1" runat="server" Text='<%# Eval("Plot") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt1" runat="server" Text='<%# Bind("Plot") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvPlot" ControlToValidate="txt1" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Plot score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Characters">
					<ItemTemplate>
            <asp:Label ID="lbl2" runat="server" Text='<%# Eval("Characters") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt2" runat="server" Text='<%# Bind("Characters") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvCharacter" ControlToValidate="txt2" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Characterisation score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Scenery">
					<ItemTemplate>
            <asp:Label ID="lbl3" runat="server" Text='<%# Eval("Scenery") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt3" runat="server" Text='<%# Bind("Scenery") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvScenery" ControlToValidate="txt3" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Scenery score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Writing">
					<ItemTemplate>
            <asp:Label ID="lbl4" runat="server" Text='<%# Eval("Writing") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt4" runat="server" Text='<%# Bind("Writing") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvWriting" ControlToValidate="txt4" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Writing Style & Structure score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Philosophy">
					<ItemTemplate>
            <asp:Label ID="lbl5" runat="server" Text='<%# Eval("Philosophy") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt5" runat="server" Text='<%# Bind("Philosophy") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvPhilisophy" ControlToValidate="txt5" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Philosophy score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Unputdown">
					<ItemTemplate>
            <asp:Label ID="lbl6" runat="server" Text='<%# Eval("Unputdown") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt6" runat="server" Text='<%# Bind("Unputdown") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvUnputdown" ControlToValidate="txt6" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Unputdownability score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="Overall">
					<ItemTemplate>
            <asp:Label ID="lbl7" runat="server" Text='<%# Eval("Overall") %>'></asp:Label>
          </ItemTemplate>
					<EditItemTemplate>
						<asp:Textbox ID="txt7" runat="server" Text='<%# Bind("Overall") %>' Width="40px" style="text-align: right;"></asp:Textbox>
						<asp:CustomValidator ID="ctvOverall" ControlToValidate="txt7" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Overall score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
					</EditItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField ShowHeader="True" HeaderText="ID" Visible="False">
					<ItemTemplate>
            <asp:Label ID="lblGroupMemberID" runat="server" Text='<%# Eval("GroupMemberID") %>'></asp:Label>
          </ItemTemplate>
				</asp:TemplateField>
		</Columns>
  </asp:GridView>
  <asp:ObjectDataSource ID="objBookMarks" runat="server" 
      OldValuesParameterFormatString="original_{0}" 
      SelectMethod="SelectPersonBookMarks"
			InsertMethod="InsertPersonBookMarks"
			UpdateMethod="UpdatePersonBookMarks"
			DeleteMethod="DeletePersonBookMarks"  
      TypeName="BookgroupLibraryCode.PersonBookMarks" 
      onobjectcreating="objBookMarks_ObjectCreating">
			<UpdateParameters>
				<asp:Parameter Name="Plot" />
				<asp:Parameter Name="Characters" />
				<asp:Parameter Name="Scenery" />
				<asp:Parameter Name="Writing" />
				<asp:Parameter Name="Philosophy" />
				<asp:Parameter Name="Unputdown" />
				<asp:Parameter Name="Overall" />
			</UpdateParameters>
  </asp:ObjectDataSource>
	<table>
		<tr>
		<td><asp:ValidationSummary ID="vsmUpdateScoreRow"  ValidationGroup="UpdateScoreGridRow" runat="server" ForeColor="Red"/></td>
		</tr>
		<tr>
			<td style="text-align: left">
				<asp:Button ID="btnSaveScores" runat="server" onclick="btnSaveScores_Click" 
					Text="Update Scores" />
			</td>
			<td style="text-align: left">
				<asp:Button ID="btnRevertScores" runat="server" onclick="btnRevertScores_Click" 
					Text="Revert Scores" />
			</td>
		</tr>
	</table>
		
	</asp:Panel>
	
	<asp:SqlDataSource ID="SqlAllGroupMembers" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT [GroupMemberID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [GroupMember] ORDER BY [LastName], [FirstName]">
	</asp:SqlDataSource>

	<asp:SqlDataSource ID="SqlMyGroupMembership" runat="server" 
		ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
		SelectCommand="SELECT [GroupMemberID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [GroupMember] WHERE [GroupMemberID] = @GroupMemberID">
		<SelectParameters>
			<asp:Parameter Name="GroupMemberID" Type="Int32" DefaultValue="0" />
		</SelectParameters>
	</asp:SqlDataSource>

	<asp:ValidationSummary ID="vsmUpdateScores"  ValidationGroup="SaveScores" runat="server" ForeColor="Red"/>

</asp:Content>
