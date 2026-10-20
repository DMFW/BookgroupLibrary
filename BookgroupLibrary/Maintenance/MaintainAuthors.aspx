<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MaintainAuthors.aspx.cs" Inherits="BookgroupLibrary.MaintainAuthors" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Authors</h1>
		<asp:Panel ID="pnlGridView" runat="server">
			<table>
				<tr>
					<td>
						<asp:GridView ID="grdAuthors" runat="server" AllowSorting="True" 
							AutoGenerateColumns="False" DataKeyNames="AuthorID" 
							DataSourceID="SqlDataSourceAuthors" AllowPaging="True" pagesize="25" RowStyle-VerticalAlign="Top"
							OnRowDataBound="grdAuthors_RowDataBound">
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
                <asp:BoundField DataField="AuthorID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="AuthorID" />
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
								<asp:TemplateField HeaderText="Books Read">
									<ItemTemplate>
										<asp:GridView ID="grdAuthorBooks" runat="server" AutoGenerateColumns="False" DataKeyNames="BookID" DataSourceID="SqlDataSourceAuthorBooks" ShowHeader="False" gridlines="None">
											<Columns>
												<asp:BoundField DataField="Title" ReadOnly="True" ItemStyle-Width="250px" />
												<asp:BoundField DataField="DateRead" ReadOnly="True" DataFormatString="{0:Y}" />
											</Columns>
										</asp:GridView>
										<asp:SqlDataSource ID="SqlDataSourceAuthorBooks" runat="server" 
													ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
													SelectCommand="SELECT [Book].[BookID], [Book].[Title], [Book].[DateRead] FROM BookAuthor INNER JOIN Book on [BookAuthor].[BookID] = [Book].[BookID] WHERE [BookAuthor].[AuthorID] = @AuthorID ORDER BY DateRead, Title">
													 <SelectParameters>
															<asp:Parameter Type="Int16" Name="AuthorID"></asp:Parameter>
													 </SelectParameters>
										</asp:SqlDataSource>
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
							<RowStyle VerticalAlign="Top" />
						</asp:GridView>
           </td> 
          </tr> 
          <tr>
            <td align="left">
                <asp:LinkButton runat="server"  ID="lbtnAddNewAuthor" CssClass="CommandButton" Text="Add New Author" OnClick="lbtnAddNewAuthor_Click" />
                <asp:LinkButton runat="server"  ID="lbtnInsertAuthor" CausesValidation="True" CssClass="CommandButton" Text="Insert" OnClick="lbtnInsertAuthor_Click" Visible="False"/>
                <asp:LinkButton runat="server"  ID="lbtnCancelInsertAuthor" CausesValidation="False" CssClass="CommandButton" Text="Cancel" OnClick="lbtnCancelInsertAuthor_Click" Visible="False"/>
            </td>
          </tr>
          <tr><td><asp:ValidationSummary ID="vsmGrid" runat="server" ForeColor="Red"/></td></tr>
        </table>        
			</asp:Panel>
      
      <asp:SqlDataSource ID="SqlDataSourceAuthors" runat="server" 
          ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>" 
          SelectCommand="SELECT *, CASE Sex WHEN 'True' THEN 'Female' ELSE 'Male' END AS SexText FROM [Author] ORDER BY [LastName], [FirstName]" 
          UpdateCommand="UPDATE Author SET FirstName = @FirstName, LastName = @LastName, Sex = @Sex, DateOfBirth = @DateOfBirth WHERE AuthorID = @AuthorID"
          InsertCommand="INSERT INTO Author(FirstName, LastName, Sex, DateOfBirth) VALUES (@FirstName, @LastName, @Sex, @DateOfBirth); SELECT @AuthorID = SCOPE_IDENTITY()">
          <UpdateParameters>
              <asp:Parameter Type="Int16" Name="AuthorID"></asp:Parameter>
              <asp:Parameter Type="String" Name="FirstName"></asp:Parameter>
              <asp:Parameter Type="String" Name="LastName"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Sex"></asp:Parameter>
              <asp:Parameter Type="DateTime" Name="DateOfBirth"></asp:Parameter>
          </UpdateParameters>
          <InsertParameters>
              <asp:Parameter Type="String" Name="FirstName"></asp:Parameter>
              <asp:Parameter Type="String" Name="LastName"></asp:Parameter>
              <asp:Parameter Type="Boolean" Name="Sex"></asp:Parameter>
              <asp:Parameter Type="DateTime" Name="DateOfBirth"></asp:Parameter>
              <asp:Parameter Name="AuthorID" Direction="Output" Type="Int16" DefaultValue="0" />
          </InsertParameters>
      </asp:SqlDataSource>

</asp:Content>
