using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace BookgroupLibrary
{
    public partial class BookDetailForm : System.Web.UI.Page
    {

        BookgroupLibraryCode.PersonBookMarks _myMarks;
        string _SortDirection;

        protected void Page_Load(object sender, EventArgs e)
        {
            _SortDirection = "ASC";
            ViewState.Add("SelectedBookID", Request.QueryString["SelectedBookID"].ToString());

            if (Session["SignedOnUser"] == null)
            {
                btnEdit.Visible = false;
            }
        }

        protected void objBookMarks_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            int bookID;
            bookID = int.Parse(ViewState["SelectedBookID"].ToString());
            _myMarks = new BookgroupLibraryCode.PersonBookMarks(bookID, _SortDirection);
            e.ObjectInstance = _myMarks;
        }

        protected void grdBookMarks_RowDataBound(Object grid, GridViewRowEventArgs e)
        {

            // apply custom formatting to the header cells
            if (e.Row.RowType == DataControlRowType.Header)
            {

                // Hide all sort columns (which are all in 2nd half of row)
                int i = 0;
                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.Wrap = true;
                    if  (i >= (e.Row.Cells.Count / 2))
                    {
                        cell.Visible = false;
                    }
                    i++;
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                // set formatting for the member name column
                TableCell cell = e.Row.Cells[0];
                cell.Width = new Unit("1000px");
               
                // set formatting for value cells
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    cell = e.Row.Cells[i];

                    if (i >= (e.Row.Cells.Count / 2))
                    {
                        cell.Visible = false;
                    }
                    else
                    {
                        // right-align each of the column cells after the first
                        // and set the width
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        cell.Width = new Unit("200px");
                    }
                }

                if ((e.Row.Cells[0].Text == "Total") | (e.Row.Cells[0].Text == "Mean"))
                {

                    // Would like to use themes and skins here eventually, but one step at a time...
                    // e.Row.SkinID = "grdTotalRow";

                    e.Row.BackColor = System.Drawing.Color.CadetBlue;

                }
            }

        }

        protected void grdBookMarks_Sorting(object sender, GridViewSortEventArgs e)
        {
            /* Redirect ALL display sorts to a corresponding invisible column prefixed "Sort"
             * This allows for correct name sorting (surname first) as well as resolving 
             * numeric as string issues where 10 sorts below 9 */

            e.SortExpression = "Sort" + e.SortExpression;
            e.SortDirection = GetSortDirection(e.SortExpression);

            if (e.SortDirection == SortDirection.Ascending)
            {
                _SortDirection = "ASC";
            }
            else
            {
                _SortDirection = "DESC";
            }
 
        }

        private SortDirection GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            if (sortDirection == "ASC")
                return SortDirection.Ascending;
            else
                return SortDirection.Descending;
        }

        protected void grdBookMarks_DataBound(Object grid, EventArgs e)
        {
            if (grdBookMarks.HeaderRow != null)
            {
                foreach (TableCell cell in grdBookMarks.HeaderRow.Cells)
                {
                    LinkButton lnk = (LinkButton)cell.Controls[0];
                    lnk.Text = _myMarks.ColumnHeaderText(lnk.Text);
                    cell.Controls.Clear();
                    cell.Controls.Add(lnk);
                }
            }
        }

        protected void grdBookMarks_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            bool bookCanBeEdited = true;
            int bookID = int.Parse(ViewState["SelectedBookID"].ToString());

            if (BookgroupLibrary.AppState.BookEditLocks.ContainsKey(bookID))
            {
                BookLock bookLock = BookgroupLibrary.AppState.BookEditLocks[bookID];
                if (bookLock.LockExpired() == true)
                {
                    // There was a lock but it has expired so remove it.
                    BookgroupLibrary.AppState.BookEditLocks.Remove(bookID);
                }
                else 
                {
                    Account.SimpleUserStructure signedOnUer = (Account.SimpleUserStructure)Session["SignedOnUser"];
                    if (bookLock.GroupMemberID != signedOnUer.GroupMemberID)
                    {
                        AddValidationMessage("This book was locked for editing by " + bookLock.GroupMember + " at " + bookLock.LockAcquired + System.Environment.NewLine
                            + "If they have not removed the lock by completing or cancelling the edit it will be removed by the system 15 minutes after lock acquisition." + System.Environment.NewLine
                            + "If you are unable to edit any earlier, please try again in " + bookLock.MinutesToExpiry + " minutes.", "EditState");
                        bookCanBeEdited = false;
                    }
                }
            }

            if (bookCanBeEdited)
            {
                Server.Transfer("Maintenance/MaintainBooks.aspx?SelectedBookID=" + bookID, true);
            }
        }

        private void AddValidationMessage(string validationMessage, string validationGroup)
        {
            CustomValidator cv = new CustomValidator();
            cv.ValidationGroup = validationGroup;
            cv.IsValid = false;
            cv.ErrorMessage = validationMessage;
            this.Page.Validators.Add(cv);
        }
    }
}