<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MaintainGroup.aspx.cs" Inherits="BookgroupLibrary.MaintainGroup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h1>Group Members</h1>
		<asp:Panel ID="pnlGridView" runat="server">
			<table>
				<tr>
					<td>
						<asp:GridView ID="grdGroupMembers" runat="server" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="GroupMemberID" DataSourceID="SqlDataSourceGroupMembers"
													AllowPaging="True" pagesize="25" onselectedindexchanging="grdGroupMembers_SelectedIndexChanging" OnDataBound="grdGroupMembers_DataBound" OnRowDataBound="grdGroupMembers_RowDataBound">
							<Columns>
								<asp:TemplateField ShowHeader="False">
									<ItemTemplate>
										<asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>                          
                  </ItemTemplate>
                  <EditItemTemplate>
                      <asp:LinkButton ID="lnkButtonEdit" runat="server" CausesValidation="True" CommandName="Update" Text="Update"></asp:LinkButton>
                      &nbsp;
											<asp:LinkButton ID="lnkButtonCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                  </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="GroupMemberID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="GroupMemberID" />
                <asp:TemplateField HeaderText="Forename" SortExpression="FirstName">
                  <ItemTemplate>
                      <asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:Label>
                  </ItemTemplate>
									<EditItemTemplate>
                      <asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName") %>' columns="12"></asp:TextBox>
                  </EditItemTemplate>
                  <FooterTemplate>
                      <asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName") %>' columns="12"></asp:TextBox>
                  </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Surname" SortExpression="LastName">
                  <ItemTemplate>
										<asp:Label ID="lbllastName" runat="server" Text='<%# Bind("LastName") %>'></asp:Label>
                   </ItemTemplate>
                   <EditItemTemplate>
											<asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName") %>' columns="12"></asp:TextBox>
                   </EditItemTemplate>
                   <FooterTemplate>
											<asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName") %>' columns="12"></asp:TextBox>
                   </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sex" SortExpression="Sex">
                  <ItemTemplate>
                      <asp:Label ID="lblSexText" runat="server" Text='<%# Bind("SexText") %>'></asp:Label>
                  </ItemTemplate>
                  <EditItemTemplate>
                        <asp:DropDownList ID="ddlSex" runat="server" DataSource='<%# SexText() %>'
                              DataTextField="value" DataValueField="key" SelectedValue='<%# Bind("Sex") %>'>
                      </asp:DropDownList>
                  </EditItemTemplate>
                  <FooterTemplate>
                        <asp:DropDownList ID="ddlSex" runat="server" DataSource='<%# SexText() %>'
                              DataTextField="value" DataValueField="key" SelectedValue='<%# Bind("Sex") %>'>
                      </asp:DropDownList>
                  </FooterTemplate>
                </asp:TemplateField>                 
                <asp:TemplateField HeaderText="Date Of Birth" SortExpression="DateOfBirth">
                    <ItemTemplate>
                        <asp:Label ID="lblDOB" runat="server" 
                            Text='<%# Bind("DateOfBirth", "{0:d}") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtDOB" runat="server" columns="10"
                            Text='<%# Bind("DateOfBirth", "{0:d}") %>'></asp:TextBox>
                        <asp:CustomValidator ID="valDOB" runat="server" ControlToValidate="txtDOB" 
                            Display="None" OnServerValidate="valDOB_ServerValidate" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="txtDOB" runat="server" columns="10"
                            Text='<%# Bind("DateOfBirth", "{0:d}") %>'></asp:TextBox>
                        <asp:CustomValidator ID="valDOB" runat="server" ControlToValidate="txtDOB" 
                            Display="None" OnServerValidate="valDOB_ServerValidate" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email Address" SortExpression="EmailAddress">
									<ItemTemplate>
											<asp:Label ID="lblEmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>'></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
											<asp:TextBox ID="txtEmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>' columns="25"></asp:TextBox>
									</EditItemTemplate>
									<FooterTemplate>
											<asp:TextBox ID="txtEmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>' columns="25"></asp:TextBox>
									</FooterTemplate>
                </asp:TemplateField>
								<asp:TemplateField HeaderText="Login Name" SortExpression="LoginName">
                  <ItemTemplate>
                      <asp:Label ID="lblLoginName" runat="server" Text='<%# Bind("LoginName") %>'></asp:Label>
                  </ItemTemplate>
									<EditItemTemplate>
                      <asp:TextBox ID="txtLoginName" runat="server" Text='<%# Bind("LoginName") %>' columns="12"></asp:TextBox>
                  </EditItemTemplate>
                  <FooterTemplate>
                      <asp:TextBox ID="txtLoginName" runat="server" Text='<%# Bind("LoginName") %>' columns="12"></asp:TextBox>
                  </FooterTemplate>
                </asp:TemplateField>
								<asp:TemplateField HeaderText="Administrator" SortExpression="Administrator">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkAdministrator" runat="server" Checked='<%# Bind("Administrator") %>' Enabled="false" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="chkAdministrator" runat="server" Checked='<%# Bind("Administrator") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:CheckBox ID="chkAdministrator" runat="server" Checked='<%# Bind("Administrator") %>' />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LastLogOn" HeaderText="Last Log On" SortExpression="LastLogOn" InsertVisible="False" ReadOnly="True" />
								<asp:TemplateField HeaderText="Password">
									<ItemTemplate>
										<asp:Button ID="btnChangePassword" runat="server" CommandName="select" Text="" />
                  </ItemTemplate>
									<EditItemTemplate>
										<asp:Button ID="btnChangePassword" runat="server" CommandName="select" Text="" />
                  </EditItemTemplate>
									<FooterTemplate>
										<asp:Button ID="btnChangePassword" runat="server" CommandName="select" Text="" />
                  </FooterTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Locked?" SortExpression="IsLockedOut">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkLocked" runat="server" Checked='<%# Bind("IsLockedOut") %>' Enabled="false" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="chkLocked" runat="server" Checked='<%# Bind("IsLockedOut") %>' />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:CheckBox ID="chkLocked" runat="server" Checked='<%# Bind("IsLockedOut") %>' />
                    </FooterTemplate>
                </asp:TemplateField>        
							</Columns>
						</asp:GridView>
           </td> 
          </tr> 
          <tr>
            <td align="left">
                <asp:LinkButton runat="server"  ID="lbtnAddNewGroupMember" CssClass="CommandButton" Text="Add New Group Member" OnClick="lbtnAddNewGroupMember_Click" />
                <asp:LinkButton runat="server"  ID="lbtnInsertGroupMember" CausesValidation="True" CssClass="CommandButton" Text="Insert" OnClick="lbtnInsertGroupMember_Click" Visible="False"/>
                <asp:LinkButton runat="server"  ID="lbtnCancelInsertGroupMember" CausesValidation="False" CssClass="CommandButton" Text="Cancel" OnClick="lbtnCancelInsertGroupMember_Click" Visible="False"/>
            </td>
          </tr>
          <tr><td><asp:ValidationSummary ID="vsmGrid" runat="server" ForeColor="Red"/></td></tr>
        </table>        
			</asp:Panel>
      
      <asp:SqlDataSource ID="SqlDataSourceGroupMembers" runat="server" 
          ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
          SelectCommand="SELECT *, CASE Sex WHEN 'True' THEN 'Female' ELSE 'Male' END AS SexText FROM [GroupMember] ORDER BY [LastName], [FirstName]" 
          UpdateCommand="UPDATE GroupMember SET LoginName = @LoginName, FirstName = @FirstName, LastName = @LastName, Sex = @Sex, DateOfBirth = @DateOfBirth, EmailAddress = @EmailAddress, Administrator = @Administrator, IsLockedOut = @IsLockedOut WHERE GroupMemberID = @GroupMemberID"
          InsertCommand="INSERT INTO GroupMember(LoginName, FirstName, LastName, Sex, DateOfBirth, EmailAddress, Administrator, IsLockedOut) VALUES (@LoginName, @FirstName, @LastName, @Sex, @DateOfBirth, @EmailAddress, @Administrator, @IsLockedOut); SELECT @GroupMemberID = SCOPE_IDENTITY()"
					OnUpdated="SqlDataSourceGroupMembers_Updated" OnInserted="SqlDataSourceGroupMembers_Inserted">
          <UpdateParameters>
              <asp:Parameter Type="Int16" Name="GroupMemberID"></asp:Parameter>
							<asp:Parameter Type="String" Name="LoginName"></asp:Parameter>
              <asp:Parameter Type="String" Name="FirstName"></asp:Parameter>
              <asp:Parameter Type="String" Name="LastName"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Sex"></asp:Parameter>
              <asp:Parameter Type="DateTime" Name="DateOfBirth"></asp:Parameter>
              <asp:Parameter Type="string" Name="EmailAddress"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Administrator"></asp:Parameter>
							<asp:Parameter Type="Boolean" Name="IsLockedOut"></asp:Parameter>
          </UpdateParameters>
          <InsertParameters>
							<asp:Parameter Type="String" Name="LoginName"></asp:Parameter>
              <asp:Parameter Type="String" Name="FirstName"></asp:Parameter>
              <asp:Parameter Type="String" Name="LastName"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Sex"></asp:Parameter>
              <asp:Parameter Type="DateTime" Name="DateOfBirth"></asp:Parameter>
              <asp:Parameter Type="string" Name="EmailAddress"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Administrator"></asp:Parameter>
							<asp:Parameter Type="Boolean" Name="IsLockedOut"></asp:Parameter>
              <asp:Parameter Name="GroupMemberID" Direction="Output" Type="Int16" DefaultValue="0" />
          </InsertParameters>
      </asp:SqlDataSource>
        
</asp:Content>

