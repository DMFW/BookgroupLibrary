<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MaintainBooks.aspx.cs" Inherits="BookgroupLibrary.MaintainBooks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <asp:Label ID="lblMaintenanceMode" runat="server" Text="" />
    </h1>
    <asp:Panel ID="pnlSelectionMethod" runat="server" CssClass="selection-method-panel">
        <asp:Label ID="lblSelectionMethod" runat="server" Text="Selection Method"></asp:Label>
        <asp:DropDownList ID="ddlSelectionMethod" runat="server" AutoPostBack="True"
            DataSourceID="SqlDataSourceSelectionMethod" DataTextField="SelectionMethod"
            DataValueField="SelectionMethodType">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSourceSelectionMethod" runat="server"
            ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
            SelectCommand="SELECT [SelectionMethodType], [SelectionMethod] FROM [SelectionMethod]"></asp:SqlDataSource>
        <asp:Panel ID="pnlBBCTop100Position" runat="server" CssClass="selection-method-extension">
            <asp:Label ID="lblBBCTop100Position" runat="server" Text="Top 100 Position"></asp:Label>
            <asp:TextBox ID="txtTop100Position" runat="server"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="pnlGroupSelection" runat="server" CssClass="selection-method-extension">
            <asp:Label ID="lblGroupSelection" runat="server" Text="Chosen By: "></asp:Label>
            <asp:DropDownList ID="ddlGroupChoiceMembers" runat="server"
                DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
            </asp:DropDownList>
        </asp:Panel>
        <asp:Panel ID="pnlSecretSantaSelection" runat="server" CssClass="selection-method-extension">
            <asp:Label ID="lblChristmasYear" runat="server" Text="Christmas Year: "></asp:Label>
            <asp:TextBox ID="txtChristmasYear" runat="server"></asp:TextBox>
            <asp:Label ID="lblGiftedBy" runat="server" Text="Gifted By: "></asp:Label>
            <asp:DropDownList ID="ddlSantaGiftedBy" runat="server"
                DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
            </asp:DropDownList>
            <asp:Label ID="lblSantaGiftedTo" runat="server" Text="Gifted To: "></asp:Label>
            <asp:DropDownList ID="ddlSantaGiftedTo" runat="server"
                DataSourceID="SqlAllGroupMembers" DataTextField="DisplayName" DataValueField="GroupMemberID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
            </asp:DropDownList>
        </asp:Panel>
    </asp:Panel>
    <asp:ValidationSummary ID="vsmSelectionMethod" ValidationGroup="SelectionMethod" runat="server" ForeColor="Red" />
    <asp:Label ID="lblAuthors" runat="server" Text="Author(s)"></asp:Label>
    <asp:GridView ID="grdBookAuthors" runat="server" AutoGenerateColumns="False" ShowFooter="true"
        HeaderStyle-HorizontalAlign="Left" CellPadding="4" GridLines="None"
        OnRowDeleting="grdBookAuthors_RowDelete" CssClass="defaultgrid">
        <HeaderStyle CssClass="defaultgridHeaderStyle" />
        <FooterStyle CssClass="defaultgridFooterStyle" />
        <PagerStyle HorizontalAlign="Center" CssClass="defaultgridPagerStyle" />
        <RowStyle CssClass="defaultgridRowStyle" />
        <AlternatingRowStyle CssClass="defaultgridRowAltStyle" />
        <EditRowStyle CssClass="defaultgridEditRowStyle" />
        <SelectedRowStyle CssClass="defaultgridSelectedRowStyle" />
        <SortedAscendingCellStyle CssClass="defaultgridSortedAscendingCellStyle" />
        <SortedAscendingHeaderStyle CssClass="defaultgridSortedAscendingHeaderStyle" />
        <SortedDescendingCellStyle CssClass="defaultgridSortedDescendingCellStyle" />
        <SortedDescendingHeaderStyle CssClass="defaultgridSortedDescendingHeaderStyle" />
        <EmptyDataTemplate>
            <asp:Button ID="btnAddFirstAuthor" runat="server" Text="Add Author" OnClick="btnAddAuthor_Click" />
            <asp:DropDownList ID="ddlFirstAuthor" runat="server" DataSourceID="SqlDataSourceAllAuthors"
                DataTextField="DisplayName" DataValueField="AuthorID" AppendDataBoundItems="true" OnDataBound="ddlListRequiringUnselectedItem_DataBound">
            </asp:DropDownList>
        </EmptyDataTemplate>
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
                    <asp:Button ID="btnAddAuthor" runat="server" Text="Add Author" OnClick="btnAddAuthor_Click" />
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
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSourceAllAuthors" runat="server"
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
        SelectCommand="SELECT [AuthorID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [Author] ORDER BY [LastName], [FirstName]"></asp:SqlDataSource>
    <asp:ValidationSummary ID="vsmSelectAuthor" ValidationGroup="Authors" runat="server" CssClass="validationSummaryStyle" />
    <asp:DetailsView ID="dtvBook" runat="server" Height="50px" Width="100%"
        AutoGenerateRows="False" CellPadding="4" GridLines="None"
        OnItemInserting="dtvBook_ItemInserting" OnItemUpdating="dtvBook_ItemUpdating" CssClass="bookdetailView">
        <FieldHeaderStyle CssClass="bookdetailViewFieldHeaderStyle" />
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
                    <asp:ValidationSummary ID="vsmTitle" ValidationGroup="Title" runat="server" CssClass="validationSummaryStyle" />
                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title") %>' Width="100%"></asp:TextBox>
                    <asp:ValidationSummary ID="vsmTitle" ValidationGroup="Title" runat="server" CssClass="validationSummaryStyle" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Year of Publication">
                <InsertItemTemplate>
                    <asp:TextBox ID="txtYearOfPublication" runat="server" Text='<%# Bind("YearOfPublication") %>'></asp:TextBox>
                    <asp:ValidationSummary ID="vsmYearOfPublication" ValidationGroup="YearOfPublication" runat="server" CssClass="validationSummaryStyle" />
                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtYearOfPublication" runat="server" Text='<%# Bind("YearOfPublication", "{0:yyyy}") %>'></asp:TextBox>
                    <asp:ValidationSummary ID="vsmYearOfPublication" ValidationGroup="YearOfPublication" runat="server" CssClass="validationSummaryStyle" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date Read">
                <InsertItemTemplate>
                    <asp:TextBox ID="txtDateRead" runat="server" Text='<%# Bind("DateRead", "{0:d}") %>'></asp:TextBox>
                    <asp:ValidationSummary ID="vsmDateRead" ValidationGroup="DateRead" runat="server" CssClass="validationSummaryStyle" />
                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtDateRead" runat="server" Text='<%# Bind("DateRead", "{0:d}") %>'></asp:TextBox>
                    <asp:ValidationSummary ID="vsmDateRead" ValidationGroup="DateRead" runat="server" CssClass="validationSummaryStyle" />
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
            <td>
                <asp:Button ID="btnSave" runat="server" Text="" OnClick="btnSave_Click" /></td>
            <td>
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" /></td>
        </tr>
    </table>
    <asp:ValidationSummary ID="vsmUpdateDatabase" ValidationGroup="Save" runat="server" ForeColor="Red" />

    <hr />

    <h2>
        <asp:Label ID="lblScores" runat="server" Text="Our Scores"></asp:Label></h2>
    <asp:Label ID="lblScoreMaintenanceInformation" runat="server" Text="Group member scores can be entered for a book once it has been added by selecting it for editing." />
    <asp:Panel ID="pnlBookScores" runat="server">
        <asp:GridView ID="grdBookMarks" runat="server" DataSourceID="objBookMarks" ShowFooter="True"
            BackColor="White" BorderStyle="Ridge" BorderWidth="2px"
            CellPadding="3" CellSpacing="1" GridLines="None"
            OnDataBound="grdBookMarks_DataBound"
            OnRowDataBound="grdBookMarks_RowDataBound"
            OnRowUpdating="grdBookMarks_RowUpdating"
            OnRowUpdated="grdBookMarks_RowUpdated"
            OnRowDeleting="grdBookMarks_RowDeleting"
            OnRowDeleted="grdBookMarks_RowDeleted"
            AutoGenerateColumns="False" CssClass="markingGrid">
            <HeaderStyle CssClass="markingGridHeaderStyle" />
            <FooterStyle CssClass="markingGridFooterStyle" />
            <PagerStyle HorizontalAlign="Right" CssClass="markingGridPagerStyle" />
            <RowStyle CssClass="markingGridRowStyle" />
            <AlternatingRowStyle CssClass="markingGridRowAltStyle" />
            <EditRowStyle CssClass="markingGridRowEditStyle" />
            <SelectedRowStyle CssClass="markingGridRowSelectedStyle" />
            <SortedAscendingCellStyle CssClass="markingGridSortedAscendingCellStyle" />
            <SortedAscendingHeaderStyle CssClass="markingGridSortedAscendingHeaderStyle" />
            <SortedDescendingCellStyle CssClass="markingGridSortedDescendingCellStyle" />
            <SortedDescendingHeaderStyle CssClass="markingGridSortedDescendingHeaderStyle" />
            <EmptyDataTemplate>
                <asp:Button ID="btnAddFirstScorer" runat="server" Text="Add Scorer" OnClick="btnAddScorer_Click" />
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
                        <asp:Button ID="btnAddScorer" runat="server" Text="Add Scorer" OnClick="btnAddScorer_Click" />
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
                        <asp:TextBox ID="txt1" runat="server" Text='<%# Bind("Plot") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvPlot" ControlToValidate="txt1" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Plot score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Characters">
                    <ItemTemplate>
                        <asp:Label ID="lbl2" runat="server" Text='<%# Eval("Characters") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt2" runat="server" Text='<%# Bind("Characters") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvCharacter" ControlToValidate="txt2" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Characterisation score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Scenery">
                    <ItemTemplate>
                        <asp:Label ID="lbl3" runat="server" Text='<%# Eval("Scenery") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt3" runat="server" Text='<%# Bind("Scenery") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvScenery" ControlToValidate="txt3" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Scenery score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Writing">
                    <ItemTemplate>
                        <asp:Label ID="lbl4" runat="server" Text='<%# Eval("Writing") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt4" runat="server" Text='<%# Bind("Writing") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvWriting" ControlToValidate="txt4" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Writing Style & Structure score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Philosophy">
                    <ItemTemplate>
                        <asp:Label ID="lbl5" runat="server" Text='<%# Eval("Philosophy") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt5" runat="server" Text='<%# Bind("Philosophy") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvPhilisophy" ControlToValidate="txt5" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Philosophy score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Unputdown">
                    <ItemTemplate>
                        <asp:Label ID="lbl6" runat="server" Text='<%# Eval("Unputdown") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt6" runat="server" Text='<%# Bind("Unputdown") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
                        <asp:CustomValidator ID="ctvUnputdown" ControlToValidate="txt6" runat="server" OnServerValidate="ConfirmFormattingAsDouble" ErrorMessage="The Unputdownability score must either be numeric or all values deleted (do not type spaces) if it should not be set." Display="None" ValidationGroup="UpdateScoreGridRow" ValidateEmptyText="True"></asp:CustomValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" HeaderText="Overall">
                    <ItemTemplate>
                        <asp:Label ID="lbl7" runat="server" Text='<%# Eval("Overall") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txt7" runat="server" Text='<%# Bind("Overall") %>' Width="40px" Style="text-align: right;"></asp:TextBox>
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
            OnObjectCreating="objBookMarks_ObjectCreating">
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
                <td>
                    <asp:ValidationSummary ID="vsmUpdateScoreRow" ValidationGroup="UpdateScoreGridRow" runat="server" CssClass="validationSummaryStyle" />
                </td>
            </tr>
            <tr>
                <td style="text-align: left">
                    <asp:Button ID="btnSaveScores" runat="server" OnClick="btnSaveScores_Click"
                        Text="Update Scores" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnRevertScores" runat="server" OnClick="btnRevertScores_Click"
                        Text="Revert Scores" />
                </td>
            </tr>
        </table>

    </asp:Panel>

    <asp:SqlDataSource ID="SqlAllGroupMembers" runat="server"
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
        SelectCommand="SELECT [GroupMemberID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [GroupMember] ORDER BY [LastName], [FirstName]"></asp:SqlDataSource>

    <asp:SqlDataSource ID="SqlMyGroupMembership" runat="server"
        ConnectionString="<%$ ConnectionStrings:SouthendBookgroupConnectionString %>"
        SelectCommand="SELECT [GroupMemberID], [LastName], [FirstName], [LastName] + ', ' + [FirstName] AS DisplayName FROM [GroupMember] WHERE [GroupMemberID] = @GroupMemberID">
        <SelectParameters>
            <asp:Parameter Name="GroupMemberID" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:ValidationSummary ID="vsmUpdateScores" ValidationGroup="SaveScores" runat="server" ForeColor="Red" />

</asp:Content>
