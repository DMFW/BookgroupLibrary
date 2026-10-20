using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BookgroupLibraryCode;
using SharedCode;

namespace BookgroupLibrary
{
    public partial class MaintainBooks : System.Web.UI.Page
    {

        private Account.SimpleUserStructure _SignedOnUser;

        public enum SelectionType { BBCTop100 = 1, MemberChoice = 2, Agreement = 3 };

        int _SelectedBookID;
        DataTable _dtBookAuthors;
        DataTable _dtBookDetails;
        private System.Configuration.ConnectionStringSettings _ConnectionSettings;

        BookgroupLibraryCode.PersonBookMarks _myMarks;
        
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["SignedOnUser"] != null)
            {
                _SignedOnUser = (Account.SimpleUserStructure)Session["SignedOnUser"];
            }
            else
            {
                // We shouldn't be on this page so however we got here, reject
                Server.Transfer("Account/UnauthorisedPage.aspx", true);
            }

            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");

            Label lblScoreMaintenanceInformation = (Label)ControlExtensions.FindControlRecursive(Page,"lblScoreMaintenanceInformation");
            Panel pnlBookScores = (Panel)ControlExtensions.FindControlRecursive(Page, "pnlBookScores");

            if (Page.IsPostBack)
            {
                _SelectedBookID = Int32.Parse(ViewState["SelectedBookID"].ToString());
                _dtBookAuthors = (DataTable)ViewState["BookAuthors"];
                _dtBookDetails = (DataTable)ViewState["BookDetails"];

                _dtBookDetails.TableNewRow += new DataTableNewRowEventHandler(_dtBookDetails_NewRow);
            }
            else
            {

                ViewState["RefUrl"] = Request.UrlReferrer.ToString();
                ViewState.Add("SelectedBookID", Request.QueryString["SelectedBookID"].ToString());
                _SelectedBookID = Int32.Parse(ViewState["SelectedBookID"].ToString());

                if (_SelectedBookID != 0)
                {
                    // Add a lock for the editing user 
                    if (AppState.BookEditLocks.Keys.Contains(_SelectedBookID))
                    {
                        // This will only be a lock we already had. Remove it and re-add it to renew the lock.
                        AppState.BookEditLocks.Remove(_SelectedBookID);
                    }
                    AppState.BookEditLocks.Add(_SelectedBookID, new BookLock(_SelectedBookID, _SignedOnUser.GroupMemberID, _SignedOnUser.FirstName + " " + _SignedOnUser.LastName));
                }

                ddlSelectionMethod.DataBind();

                SetInitialAuthorsRow(); 
                SetInitialBookDetails();
                
                if (_SelectedBookID == 0)
                {
                    dtvBook.ChangeMode(DetailsViewMode.Insert);
                    lblMaintenanceMode.Text = "Add Book";
                    int memberChoice = (int)SelectionType.MemberChoice;
                    ddlSelectionMethod.SelectedValue = memberChoice.ToString(); // Most useful default
                    btnSave.Text = "Add Book";
                    lblScoreMaintenanceInformation.Visible = true;
                    pnlBookScores.Visible = false;
                }
                else
                {
                    dtvBook.ChangeMode(DetailsViewMode.Edit);
                    LoadBookAuthors(); 
                    LoadBookDetails(); 
                    lblMaintenanceMode.Text = "Edit Book";
                    btnSave.Text = "Update Book";
                    lblScoreMaintenanceInformation.Visible = false;
                    pnlBookScores.Visible = true;
                }
            }

            switch ((SelectionType)Enum.Parse(typeof(SelectionType),ddlSelectionMethod.SelectedValue))
            {
                case SelectionType.BBCTop100:
                    lblSelectionValueText.Text = "Position : ";
                    ddlGroupMembers.Visible = false;
                    txtTop100Position.Visible = true;
                    break;
                case SelectionType.MemberChoice:
                    lblSelectionValueText.Text = "Chosen By : ";
                    ddlGroupMembers.Visible = true;
                    txtTop100Position.Visible = false;
                    break;
                case SelectionType.Agreement:
                    lblSelectionValueText.Text = "";
                    ddlGroupMembers.Visible = false;
                    txtTop100Position.Visible = false;
                    break;
            }
        }

        private void SetInitialAuthorsRow()
        {
            _dtBookAuthors = new DataTable();
            _dtBookAuthors.Columns.Add(new DataColumn("AuthorID", typeof(int)));
            _dtBookAuthors.Columns.Add(new DataColumn("FirstName", typeof(string)));
            _dtBookAuthors.Columns.Add(new DataColumn("LastName", typeof(string)));
            
            //Store the DataTable in ViewState
            ViewState["BookAuthors"] = _dtBookAuthors;

            grdBookAuthors.DataSource = _dtBookAuthors;
            grdBookAuthors.DataBind();
        }

        private void SetInitialBookDetails()
        {
            _dtBookDetails = new DataTable();
            _dtBookDetails.Columns.Add(new DataColumn("BookID", typeof(int)));
            _dtBookDetails.Columns.Add(new DataColumn("Title", typeof(string)));
            _dtBookDetails.Columns.Add(new DataColumn("YearOfPublication", typeof(DateTime)));
            _dtBookDetails.Columns.Add(new DataColumn("DateRead", typeof(DateTime)));

            ViewState["BookDetails"] = _dtBookDetails;

            dtvBook.DataSource = _dtBookDetails;
            dtvBook.DataBind();

        }

        protected void btnAddAuthor_Click(object sender, EventArgs e)
        {
            DropDownList ddlAuthors;

            if (_dtBookAuthors.Rows.Count == 0)
            {
                // There are no authors yet so add use the grid empty template list.
                // Obviously in a half decent object model where there was some sort of consistency you'd have a find control
                // method on the emmpty template object just like you do on the grid itself and on the footer row. 
                // But that would just be too simple wouldn't it? Instead of wrapping this, we have to find the implied first row
                // in the implied nested grid which is how the empty template is implemented and somehow we have to guess this...
                // (not to mention assume that the implementation doesn't ever change). Major object orientation fail...

                GridViewRow dr = (GridViewRow)grdBookAuthors.Controls[0].Controls[0];
                ddlAuthors = (DropDownList)dr.FindControl("ddlFirstAuthor");
            }
            else
            {
                // Use the drop down in the grid footer as we have at least one author already
                ddlAuthors = (DropDownList)grdBookAuthors.FooterRow.FindControl("ddlMultipleAuthors");
            }

            int AuthorID = Convert.ToInt32(ddlAuthors.SelectedValue);

            if (AuthorID == 0)
            {
                AddValidationMessage("Please select an author before clicking the 'Add Author' button.", "Authors");
            }
            else
            {
                AddAuthor(AuthorID);
            }
        }

        private void AddAuthor(int AuthorID)
        {

            string FirstName;
            string LastName;

            string queryString = "SELECT FirstName, LastName FROM Author Where AuthorID = @AuthorID;";

            using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@AuthorID", AuthorID));

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Debug.Assert(reader.HasRows);

                reader.Read();
                FirstName = (string)reader["FirstName"];
                LastName = (string)reader["LastName"];
                reader.Close();
            }

            DataRow drBookAuthors = _dtBookAuthors.NewRow();
            drBookAuthors["AuthorID"] = AuthorID;
            drBookAuthors["FirstName"] = FirstName;
            drBookAuthors["LastName"] = LastName;
            _dtBookAuthors.Rows.Add(drBookAuthors);

            grdBookAuthors.DataSource = _dtBookAuthors;
            grdBookAuthors.DataBind();

        }

        protected void grdBookAuthors_RowDelete(object sender, EventArgs e)
        {

            GridViewDeleteEventArgs RowDeleteParameters = (GridViewDeleteEventArgs)e;

            Label lblAuthorID = (Label)grdBookAuthors.Rows[RowDeleteParameters.RowIndex].FindControl("lblAuthorID");

            int AuthorID = Int32.Parse(lblAuthorID.Text);

            foreach (DataRow drBookAuthors in _dtBookAuthors.Rows)
            {
                if (Int32.Parse(drBookAuthors["AuthorID"].ToString()) == AuthorID)
                {
                    drBookAuthors.Delete();
                    grdBookAuthors.DataSource = _dtBookAuthors;
                    grdBookAuthors.DataBind();
                    return;
                }
            }
        }

        protected void ddlListRequiringUnselectedItem_DataBound(object sender, EventArgs e)
        {
            DropDownList ddlListRequiringUnselectedItem = (DropDownList)sender;
            ddlListRequiringUnselectedItem.Items.Insert(0, new ListItem("--Select---", "0")); 
        }

        private void LoadBookAuthors()
        {
            string queryString = "SELECT * FROM [BookAuthor] WHERE [BookAuthor].[BookID] = @BookID;";

            using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@BookID", _SelectedBookID));

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Debug.Assert(reader.HasRows);

                while (reader.Read())
                {
                    // I know this design would not be efficient if there were "lots"
                    // of authors but as even two authors will be a rarety I think
                    // we can take the hit and benefit from some simple reuse of the function below 
                    // originally designed for the insert function.
                    AddAuthor(Int32.Parse(reader["AuthorID"].ToString()));
                }

                reader.Close();
            }

        }
        
        private void LoadBookDetails()
        {

            // Used when editing a book to initialise the controls

            _dtBookDetails.Rows.Clear();

            string queryString = "SELECT * FROM BookDetail WHERE BookID = @BookID;";

            using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@BookID", _SelectedBookID));

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Debug.Assert(reader.HasRows);

                reader.Read();

                ddlSelectionMethod.SelectedValue = reader["SelectionMethodType"].ToString();
                txtTop100Position.Text = reader["BBCTopBookPosition"].ToString();

                if (reader["GroupMemberID"] != null)
                {
                    if (reader["GroupMemberID"].ToString() != "")
                    {
                        ddlGroupMembers.SelectedValue = reader["GroupMemberID"].ToString();
                    }
                }

                // Populate the Detail view controls

                DataRow drBookDetails = _dtBookDetails.NewRow();
                drBookDetails["BookID"] = reader["BookID"].ToString();
                drBookDetails["Title"] = reader["Title"].ToString();

                DateTime YearOfPublication;
                DateTime DateRead;

                bool validYearOfPublication = DateTime.TryParse(reader["YearOfPublication"].ToString(), out YearOfPublication);
                bool validDateRead = DateTime.TryParse(reader["DateRead"].ToString(), out DateRead);

                if (validYearOfPublication) drBookDetails["YearOfPublication"] = YearOfPublication;
                if (validDateRead) drBookDetails["DateRead"] = DateRead;

                _dtBookDetails.Rows.Add(drBookDetails);

                dtvBook.DataSource = _dtBookDetails;
                dtvBook.DataBind();

                txtSynopsis.Text = reader["Synopsis"].ToString();
                reader.Close();
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (_SelectedBookID != 0)
            {
                // Remove the editing lock
                BookgroupLibrary.AppState.BookEditLocks.Remove(_SelectedBookID);
            }
            object refUrl = ViewState["RefUrl"];
            if (refUrl != null) Response.Redirect((string)refUrl);       
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool saveData = false;
            bool formValid = FormValidate();
            if (formValid)
            {
                if (_SelectedBookID == 0)
                {
                    saveData = SaveNewBookData();
                }
                else
                {
                    saveData = UpdateBookData();
                }
                if (saveData)
                {
                    if (_SelectedBookID != 0)
                    {
                        // Remove the editing lock
                        BookgroupLibrary.AppState.BookEditLocks.Remove(_SelectedBookID);
                    }
                    object refUrl = ViewState["RefUrl"];
                    if (refUrl != null) Response.Redirect((string)refUrl);
                }
            }
        }

        protected bool FormValidate()
        {
            bool bFormValid = true;

            #region SelectionMethodSpecific

            switch ((SelectionType)Enum.Parse(typeof(SelectionType), ddlSelectionMethod.SelectedValue))
            {
                case SelectionType.BBCTop100:

                    int top100Position;
                    bool validInteger;
                    validInteger = Int32.TryParse(txtTop100Position.Text, out top100Position);
                    if (validInteger)
                    {
                        if ((top100Position < 1) | (top100Position > 100))
                        {
                            AddValidationMessage("Top 100 position must be between 1 and 100.", "SelectionMethod");
                            bFormValid = false;
                        }
                    }
                    else
                    {
                        AddValidationMessage("Top 100 position must be a valid integer.", "SelectionMethod");
                        bFormValid = false;
                    }
                    break;

                case SelectionType.MemberChoice:

                    if (ddlGroupMembers.SelectedIndex == 0)
                    {
                        AddValidationMessage("Please pick the group member chosing this book or use another selection method.", "SelectionMethod");
                        bFormValid = false;
                    }
                    break;

                case SelectionType.Agreement:
                   
                    // Nothing to validate here
                    break;
            }

            #endregion SelectionMethodSpecific

            #region AuthorDropDownState

            // Some courtesy (not strictly essential) validation on the author, in case the user meant to add their selection and misunderstood the interface.

            DropDownList ddlAuthors;

            if (_dtBookAuthors.Rows.Count == 0)
            {
                GridViewRow dr = (GridViewRow)grdBookAuthors.Controls[0].Controls[0];
                ddlAuthors = (DropDownList)dr.FindControl("ddlFirstAuthor");
            }
            else
            {
                // Use the drop down in the grid footer as we have at least one author already
                ddlAuthors = (DropDownList)grdBookAuthors.FooterRow.FindControl("ddlMultipleAuthors");
            }

            int AuthorID = Convert.ToInt32(ddlAuthors.SelectedValue);

            if (AuthorID != 0)
            {
                AddValidationMessage("This author has not been added to the book. Either click 'Add Author' or de-select before saving the book.", "Authors");
                bFormValid = false;
            }

            #endregion AuthorDropDownState

            #region DetailView

            // I'm taking full manual control of the detail view validation so it can be integrated
            // with the non-detail validation in one unified pass

            TextBox txtTitle = (TextBox)dtvBook.FindControl("txtTitle");

            if (txtTitle != null)
            {
                if (txtTitle.Text.Trim() == "")
                {
                    AddValidationMessage("The book must have a title.", "Title");
                    bFormValid = false;
                }
            }

            TextBox txtYearOfPublication = (TextBox)dtvBook.FindControl("txtYearOfPublication");
            int yearOfPublication;

            if (txtYearOfPublication != null)
            {
                if (!Int32.TryParse(txtYearOfPublication.Text.ToString(), out yearOfPublication))
                {
                    if (txtYearOfPublication.Text.Trim() != "")
                    {
                        AddValidationMessage("Please enter a valid year in the form YYYY or blank.", "YearOfPublication");
                        bFormValid = false;
                    }
                }
                else
                {
                    if (yearOfPublication > DateTime.Now.Year)
                    {
                        AddValidationMessage("Year of publication should not be in the future.", "YearOfPublication");
                        bFormValid = false;
                    }
                }
            }

            TextBox txtDateRead = (TextBox)dtvBook.FindControl("txtDateRead");
            DateTime dteDateRead;

            if (txtDateRead != null)
            {
                if (!DateTime.TryParse(txtDateRead.Text.ToString(), out dteDateRead))
                {
                    if (txtDateRead.Text.Trim() != "")
                    {
                        AddValidationMessage("Please enter a valid date or blank.", "DateRead");
                        bFormValid = false;
                    }
                }
            }

            if (bFormValid)
            {
                switch ((DetailsViewMode)dtvBook.CurrentMode)
                {
                    case DetailsViewMode.Insert:
                        dtvBook.InsertItem(false);
                        break;
                    case DetailsViewMode.Edit:
                        dtvBook.UpdateItem(false);
                        break;
                }
            }

            #endregion DetailView

            return bFormValid;
        }

        protected void dtvBook_ItemInserting(Object sender, DetailsViewInsertEventArgs e)
        {
            // Insert manually into the data row because it is not a true data source
            // so the detail view (even if bound it it) will not insert automatically

            // We only want one row in the data set after insert and there should always be zero before.
            Debug.Assert (_dtBookDetails.Rows.Count == 0);

            DataRow drBookDetails = _dtBookDetails.NewRow();
            object physicalSafeDataField;

            // Book ID is "read only" and not in the parameters. 
            // It will be auto calculated in the physical insert but put a place holder here for completion.

            e.Values["BookID"] = "0";

           
            foreach (string key in e.Values.Keys)
            {

                // Apply any required modifcations to strict physical data storage requirements
                // e.g. year of publication validation is against a valid year but the database needs a full date.

                if (e.Values[key] == null)
                {
                    physicalSafeDataField = DBNull.Value;
                }
                else
                {
                    switch (key)
                    {
                        case "YearOfPublication":
                            if (e.Values[key].ToString().Trim() == "")
                                physicalSafeDataField = ""; // in fact this won't save but that's Ok because it will force a null
                            else
                                physicalSafeDataField = "01/01/" + e.Values[key];
                            break;
                        default:
                            physicalSafeDataField = e.Values[key].ToString();
                            break;
                    }
                }
                // This may well involve type converstion but previous logic should have ensured it will always succeed.
                drBookDetails[key] = physicalSafeDataField;
            }

            _dtBookDetails.Rows.Add(drBookDetails);
        }

        protected void dtvBook_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            // Update the data row manually because it is not a true data source
            // so the detail view (even if bound it it) will not update automatically

            // We only expect one row in the data set before an update....

            Debug.Assert(_dtBookDetails.Rows.Count == 1);

            DataRow drBookDetails = _dtBookDetails.Rows[0];
            object physicalSafeDataField;

            // Book ID is "read only" and not in the parameters. 
            // It will be auto calculated in the physical insert but put a place holder here for completion.

            e.NewValues["BookID"] = _SelectedBookID;

            foreach (string key in e.NewValues.Keys)
            {

                // Apply any required modifcations to strict physical data storage requirements
                // e.g. year of publication validation is against a valid year but the database needs a full date.

                if (e.NewValues[key] == null)
                {
                    physicalSafeDataField = DBNull.Value;
                }
                else
                {
                    switch (key)
                    {
                        case "YearOfPublication":
                            if (e.NewValues[key].ToString().Trim() == "")
                                physicalSafeDataField = ""; // in fact this won't save but that's Ok because it will force a null
                            else
                                physicalSafeDataField = "01/01/" + e.NewValues[key];
                            break;
                        default:
                            physicalSafeDataField = e.NewValues[key].ToString();
                            break;
                    }
                }
                // This may well involve type converstion but previous logic should have ensured it will always succeed.
                drBookDetails[key] = physicalSafeDataField;
            }

            ViewState["BookDetails"] = _dtBookDetails;
        }

        protected void _dtBookDetails_NewRow(object sender, DataTableNewRowEventArgs e)
        {
            // In fact nothing we need to do in this event at present but it was useful in debugging....
        }

        protected bool SaveNewBookData()
        {

            bool saveResult;
            SqlConnection con = new SqlConnection(_ConnectionSettings.ConnectionString);

            SqlTransaction bookTrans;
            
            con.Open();
            bookTrans = con.BeginTransaction();
            
            try
            {

                #region InsertBook

                SqlCommand cmd = new SqlCommand("INSERT INTO" +
                                                " Book(Title,YearOfPublication,DateRead,Synopsis,SelectionMethodType)" +
                                                " VALUES" +
                                                " (@Title,@YearOfPublication,@DateRead,@Synopsis,@SelectionMethodType)" +
                                                ";SELECT SCOPE_IDENTITY();", con);
                cmd.Transaction = bookTrans;

                DataRow drBookDetails = _dtBookDetails.Rows[0];

                cmd.Parameters.Add(new SqlParameter("@Title", drBookDetails["Title"]));

                DateTime yearOfPublication;
                if (DateTime.TryParse(drBookDetails["YearOfPublication"].ToString(), out yearOfPublication))
                {
                    cmd.Parameters.Add(new SqlParameter("@YearOfPublication", yearOfPublication));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@YearOfPublication", DBNull.Value));
                }

                DateTime dateRead;
                if (DateTime.TryParse(drBookDetails["DateRead"].ToString(), out dateRead))
                {
                    cmd.Parameters.Add(new SqlParameter("@DateRead", dateRead));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@DateRead", DBNull.Value));
                }

                cmd.Parameters.Add(new SqlParameter("@Synopsis", txtSynopsis.Text));
                cmd.Parameters.Add(new SqlParameter("@SelectionMethodType", ddlSelectionMethod.SelectedValue));

                object newBookID = cmd.ExecuteScalar();
                int bookID = Int32.Parse(newBookID.ToString());

                #endregion InsertBook

                saveResult = ChangeBookAncillaryData(bookID, con, bookTrans);

                if (saveResult)
                {
                    bookTrans.Commit();
                    saveResult = true;
                }
            }
            catch(Exception e)
            {
                bookTrans.Rollback();
                AddValidationMessage("Database save failed : " + e.Message, "Save");
                saveResult = false;
            }
            finally
            {
                con.Close();
            }

            _dtBookDetails.Clear(); // So we do a fresh insert with any changed parameters in the retry (unless exiting function)
            return saveResult;

        }

        protected bool UpdateBookData()
        {
            bool saveResult;
            SqlConnection con = new SqlConnection(_ConnectionSettings.ConnectionString);
            SqlTransaction trans;
            con.Open();
            trans = con.BeginTransaction();

            try
            {

                #region UpdateBook

                SqlCommand cmd = new SqlCommand("UPDATE Book SET Title = @Title, YearOfPublication = @YearOfPublication, " +
                                                "DateRead = @DateRead, Synopsis = @Synopsis, SelectionMethodType = @SelectionMethodType " +
                                                "WHERE BookID = " + _SelectedBookID, con);
                                              
                cmd.Transaction = trans;

                DataRow drBookDetails = _dtBookDetails.Rows[0];

                cmd.Parameters.Add(new SqlParameter("@Title", drBookDetails["Title"]));

                DateTime yearOfPublication;
                if (DateTime.TryParse(drBookDetails["YearOfPublication"].ToString(), out yearOfPublication))
                {
                    cmd.Parameters.Add(new SqlParameter("@YearOfPublication", yearOfPublication));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@YearOfPublication", DBNull.Value));
                }

                DateTime dateRead;
                if (DateTime.TryParse(drBookDetails["DateRead"].ToString(), out dateRead))
                {
                    cmd.Parameters.Add(new SqlParameter("@DateRead", dateRead));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@DateRead", DBNull.Value));
                }

                cmd.Parameters.Add(new SqlParameter("@Synopsis", txtSynopsis.Text));
                cmd.Parameters.Add(new SqlParameter("@SelectionMethodType", ddlSelectionMethod.SelectedValue));

                cmd.ExecuteNonQuery();

                #endregion UpdateBook

                saveResult = ChangeBookAncillaryData(_SelectedBookID, con, trans);

                if (saveResult)
                {
                    trans.Commit();
                    saveResult = true;
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                AddValidationMessage("Database save failed : " + e.Message, "Save");
                saveResult = false;
            }
            finally
            {
                con.Close();
            }

            return saveResult;
        }

        private bool ChangeBookAncillaryData(int bookID, SqlConnection con, SqlTransaction trans)
        {
            bool saveResult;

            try
            {
                #region ChangeAuthors

                DeleteExistingBookAuthors(bookID, con, trans);

                SqlCommand cmd = new SqlCommand("INSERT INTO BookAuthor(BookID,AuthorID)" +
                                                " VALUES (@BookID, @AuthorID)", con);
                cmd.Transaction = trans;

                cmd.Parameters.Add(new SqlParameter("@BookID", bookID));
                SqlParameter sqpAuthor = new SqlParameter("@AuthorID", 0);
                cmd.Parameters.Add(sqpAuthor);

                foreach (DataRow drBookAuthor in _dtBookAuthors.Rows)
                {
                    sqpAuthor.Value = (int)drBookAuthor["AuthorID"];
                    cmd.ExecuteNonQuery();
                }

                #endregion ChangeAuthors

                #region ChangeSelectionParameters

                DeleteExistingSelectionData(bookID, con, trans);

                switch ((SelectionType)Enum.Parse(typeof(SelectionType), ddlSelectionMethod.SelectedValue))
                {
                    case SelectionType.BBCTop100:

                        cmd = new SqlCommand("INSERT INTO BBCTopBooks(BookID,TopBookPosition)" +
                                                " VALUES (@BookID, @TopBookPosition)", con);
                        cmd.Transaction = trans;
                        cmd.Parameters.Add(new SqlParameter("@BookID", bookID));
                        cmd.Parameters.Add(new SqlParameter("@TopBookPosition", Int32.Parse(txtTop100Position.Text)));
                        cmd.ExecuteNonQuery();

                        break;
                    case SelectionType.MemberChoice:

                        cmd = new SqlCommand("INSERT INTO GroupChoiceBook(BookID,GroupMemberID)" +
                                                " VALUES (@BookID, @GroupMemberID)", con);
                        cmd.Transaction = trans;
                        cmd.Parameters.Add(new SqlParameter("@BookID", bookID));
                        cmd.Parameters.Add(new SqlParameter("@GroupMemberID", Int32.Parse(ddlGroupMembers.SelectedValue)));
                        cmd.ExecuteNonQuery();

                        break;
                    case SelectionType.Agreement:
                        // Nothing to do
                        break;
                }

                saveResult = true;
                #endregion ChangeSelectionParameters
            }
            catch (Exception e)
            {
                trans.Rollback();
                AddValidationMessage("Saving book ancillary data failed : " + e.Message, "Save");
                saveResult = false;
            }
            return saveResult;
        }

        private void DeleteExistingBookAuthors(int BookID, SqlConnection con, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand("DELETE BookAuthor WHERE BookID = " + BookID + "; ", con);
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
        }

        private void DeleteExistingSelectionData(int BookID, SqlConnection con, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand("DELETE BBCTopBooks WHERE BookID = " + BookID + "; " +
                                            "DELETE GroupChoiceBook where BookID = " + BookID, con);
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
        }

        private void AddValidationMessage(string validationMessage, string validationGroup)
        {
            CustomValidator cv = new CustomValidator();
            cv.ValidationGroup = validationGroup;
            cv.IsValid = false;
            cv.ErrorMessage = validationMessage;
            this.Page.Validators.Add(cv);
        }

        protected void objBookMarks_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            int bookID;
            bookID = int.Parse(ViewState["SelectedBookID"].ToString());

            if (ViewState["PersonBookMarksEditState"] != null)
            {
                PersonBookMarksViewState editViewState = (PersonBookMarksViewState)ViewState["PersonBookMarksEditState"];
                _myMarks = new BookgroupLibraryCode.PersonBookMarks(bookID, editViewState : editViewState);
            }
            else
            {
                _myMarks = new BookgroupLibraryCode.PersonBookMarks(bookID, editViewState: null);
            }

            ViewState["PersonBookMarksEditState"] = _myMarks.CurrentViewState;
            e.ObjectInstance = _myMarks;
        }

        protected void grdBookMarks_RowDataBound(Object grid, GridViewRowEventArgs e)
        {

            // apply custom formatting to the header cells
            if (e.Row.RowType == DataControlRowType.Header)
            {

                int i = 0;
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Text = _myMarks.ColumnHeaderText(cell.Text);
                    cell.Wrap = true;
                    i++;
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                // Note that cell zero is the edit / delete control cell

                if (e.Row.RowState != DataControlRowState.Edit)
                {
                    if (!_SignedOnUser.IsAdministrator)
                    {
                        LinkButton lnkButtonEdit = (LinkButton)e.Row.FindControl("lnkButtonEdit");
                        LinkButton lnkButtonDelete = (LinkButton)e.Row.FindControl("lnkButtonDelete");
                        Label lblGroupMemberID;
                        lblGroupMemberID = e.Row.FindControl("lblGroupMemberID") as Label;

                        if (Int32.Parse(lblGroupMemberID.Text) != _SignedOnUser.GroupMemberID)
                        {
                            if (lnkButtonEdit != null) { lnkButtonEdit.Visible = false; }
                            if (lnkButtonDelete != null) { lnkButtonDelete.Visible = false; }
                        }
                    }
                }

                // set formatting for the member name column
                TableCell cell = e.Row.Cells[1];
                cell.Width = new Unit("500px");

                // set formatting for value cells
                for (int i = 2; i < e.Row.Cells.Count; i++)
                {
                    cell = e.Row.Cells[i];

                    // right-align each of the column cells after the first
                    // and set the width
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    cell.Width = new Unit("200px");
                    
                }
            }

            if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
            {
                GridViewRow currentEditRow = e.Row;

                DropDownList ddlScorer;
                ddlScorer = currentEditRow.FindControl("ddlScorer") as DropDownList;
                Label lblGroupMemberID;
                lblGroupMemberID = currentEditRow.FindControl("lblGroupMemberID") as Label;

                if ((ddlScorer != null) & (lblGroupMemberID != null))
                {
                    ddlScorer.SelectedValue = lblGroupMemberID.Text;
                }
            }

        }

        protected void grdBookMarks_DataBound(Object grid, EventArgs e)
        {

            Label lblHeader;

            if (grdBookMarks.HeaderRow != null)
            {
                foreach (TableCell cell in grdBookMarks.HeaderRow.Cells)
                {
                    if (cell.Controls.Count > 0)
                    {
                        cell.Controls.Clear(); 
                    }

                    lblHeader = new Label();
                    lblHeader.Text = _myMarks.ColumnHeaderText(cell.Text);
                    cell.Controls.Add(lblHeader);
                    
                }
            }
        }

        protected void grdBookMarks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Parameter rowIndex = new Parameter("RowIndex", DbType.Int32, e.RowIndex.ToString());
            objBookMarks.DeleteParameters.Add(rowIndex);
        }

        protected void grdBookMarks_RowDeleted(Object sender, GridViewDeletedEventArgs e)
        {
            ViewState["PersonBookMarksEditState"] = _myMarks.CurrentViewState;
            grdBookMarks.DataBind();
        }

        protected void grdBookMarks_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            Page.Validate();

            if (Page.IsValid)
            {
                /* Add the only two properties that won't bind automatically. */

                Parameter RowIndex = new Parameter("RowIndex", DbType.Int32, e.RowIndex.ToString());
                objBookMarks.UpdateParameters.Add(RowIndex);

                DropDownList ddlScorer = (DropDownList)ControlExtensions.FindControlRecursive(grdBookMarks.Rows[e.RowIndex], "ddlScorer");
                Parameter GroupMemberID = new Parameter("GroupMemberID", DbType.Int32, ddlScorer.SelectedValue);
                objBookMarks.UpdateParameters.Add(GroupMemberID);
            }
            else
            {
                e.Cancel = true;
            }

        }

        protected void ConfirmFormattingAsDouble(object source, ServerValidateEventArgs args)
        {
            // We have to do this because manually typed spaces fail later but they pass the compare
            // validator against type double (as does null). Nulls are fine but I don't want spaces.
            // We need to block blanking out by typing spaces and make sure the field is really null.

            double score;

            if (args.Value != null)
            {
                if (args.Value != String.Empty)
                {
                    if (Double.TryParse(args.Value, out score) == false)
                    {
                        args.IsValid = false;
                    }
                }
            }

        }

        protected void grdBookMarks_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            ViewState["PersonBookMarksEditState"] = _myMarks.CurrentViewState;
            grdBookMarks.DataBind();
        }

        protected void btnAddScorer_Click(object sender, EventArgs e)
        {
            Button addButton;
            DropDownList ddlScorer;

            addButton = (Button)sender;

            if (addButton.ID == "btnAddFirstScorer")
            {
                ddlScorer = (DropDownList)ControlExtensions.FindControlRecursive(Page, "ddlFirstScorer");
            }
            else
            {
                ddlScorer = (DropDownList)grdBookMarks.FooterRow.FindControl("ddlNewScorer");
            }

            // We must find the matching drop down list for either add button (empty grid or grid footer).
            Debug.Assert (ddlScorer != null); 

            int GroupMemberID = Convert.ToInt32(ddlScorer.SelectedValue);

            if (GroupMemberID == 0)
            {
                AddValidationMessage("Please select a group member before clicking the add button", "SaveScores");
            }
            else
            {
               
                Parameter groupMemberID = new Parameter("GroupMemberID", DbType.Int32, ddlScorer.SelectedValue);

                objBookMarks.InsertParameters.Add(groupMemberID);
                objBookMarks.Insert();
                ViewState["PersonBookMarksEditState"] = _myMarks.CurrentViewState;
                grdBookMarks.DataBind();
            }
        }

        protected void btnRevertScores_Click(object sender, EventArgs e)
        {
            ViewState["PersonBookMarksEditState"] = null;
            grdBookMarks.DataBind();
            vsmUpdateScores.ForeColor = System.Drawing.Color.Green;
            AddValidationMessage("Scores reverted to the last saved version.", "SaveScores");
        }

        protected void btnSaveScores_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> dctScorers = new Dictionary<int, string>();

            // Validate the cross grid relationships (persons are unique)

            vsmUpdateScores.ForeColor = System.Drawing.Color.Red;
            foreach (GridViewRow rowToAdd in grdBookMarks.Rows)
            {
                Label lblGroupMemberID = (Label)rowToAdd.FindControl("lblGroupMemberID");
                Label lblGroupMember = (Label)rowToAdd.FindControl("lblGroupMember");

                if (lblGroupMember == null)
                {
                    AddValidationMessage("Please update or cancel each changed row of scores before saving.", "SaveScores");
                }
                else
                { 
                    int groupMemberID = Int32.Parse(lblGroupMemberID.Text);
                    string groupMember = lblGroupMember.Text;

                    if (dctScorers.ContainsKey(groupMemberID))
                    {
                        AddValidationMessage("Each group member should only have one score row. " + groupMember + " is repeated.", "SaveScores");
                    }
                    else
                    {
                        dctScorers.Add(groupMemberID, groupMember);
                    }
                }
            }

            if (Page.IsValid)
            {
                string saveError;
                if (SaveScores(out saveError))
                {
                    // Revert, rebind and report success
                    ViewState["PersonBookMarksEditState"] = null;
                    grdBookMarks.DataBind();
                    vsmUpdateScores.ForeColor = System.Drawing.Color.Green;
                    AddValidationMessage("Scores successfully updated to the database.", "SaveScores");
                }
                else
                {
                    // No need to rebind as we have failed. Report the failure.
                    vsmUpdateScores.ForeColor = System.Drawing.Color.Red;
                    AddValidationMessage(saveError, "SaveScores");
                }
            }
        }

        private bool SaveScores(out string saveError)
        {
            // Create the marks class out of our view state as would be done before editing, but this time save the results. 
            _myMarks = new PersonBookMarks(_SelectedBookID, (PersonBookMarksViewState)ViewState["PersonBookMarksEditState"]);
            bool saveValid = _myMarks.TrueSave(out saveError);
            return saveValid;
        }

        protected void ddlScorer_Load(object sender, EventArgs e)
        {
            // If we are a normal (non administrative) user we are only allowed to add and change our own scores.
            // In the case of inserts and updates this is achieved by restricting the value of allowed users
            // in the drop down list to the signed on user.

            DropDownList ddlScorer = (DropDownList)sender;

            if (!_SignedOnUser.IsAdministrator)
            {
                ddlScorer.DataSourceID = "SqlMyGroupMembership";
                SqlMyGroupMembership.SelectParameters["GroupMemberID"].DefaultValue = _SignedOnUser.GroupMemberID.ToString();

                // Remove the event handler that adds the null selection as we won't need it.
                ddlScorer.DataBound -= ddlListRequiringUnselectedItem_DataBound;
            }
           
        }
    }

}