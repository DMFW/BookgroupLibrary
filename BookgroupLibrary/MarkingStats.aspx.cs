using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using SharedCode;

namespace BookgroupLibrary
{
	public partial class MarkingStatsForm : System.Web.UI.Page
	{
		private System.Configuration.ConnectionStringSettings _ConnectionSettings;
		BookgroupLibraryCode.StatsBookMarks _myMarks;
		string _SortDirection;

		protected void Page_Load(object sender, EventArgs e)
		{
			_ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");
			if (Page.IsPostBack != true)
			{
				_SortDirection = "ASC";
				LoadMemberList();
			}
		}

		protected void objStatsBookMarks_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
		{
			int personID;
			personID = int.Parse(Request.QueryString["SelectedPerson"].ToString());
			_myMarks = new BookgroupLibraryCode.StatsBookMarks(personID, _SortDirection);
			e.ObjectInstance = _myMarks;
		}

		protected void grdStatsBookMarkView_RowDataBound(Object grid, GridViewRowEventArgs e)
		{

			// apply custom formatting to the header cells
			if (e.Row.RowType == DataControlRowType.Header)
			{

				// Hide all sort columns (which are all in 2nd half of row)
				int i = 0;
				foreach (TableCell cell in e.Row.Cells)
				{
					cell.Wrap = true;
					if (i >= (e.Row.Cells.Count / 2))
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

				if ((e.Row.Cells[0].Text == "Total") || (e.Row.Cells[0].Text == "Mean"))
				{
					e.Row.CssClass = "markingGridRowTotalStyle";
				}
			}

		}

		protected void grdStatsBookMarkView_Sorting(object sender, GridViewSortEventArgs e)
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

		protected void grdStatsBookMarkView_DataBound(Object grid, EventArgs e)
		{
			if (grdStatsBookMarkView.HeaderRow != null)
			{
				foreach (TableCell cell in grdStatsBookMarkView.HeaderRow.Cells)
				{
					LinkButton lnk = (LinkButton)cell.Controls[0];
					lnk.Text = _myMarks.ColumnHeaderText(lnk.Text);
					cell.Controls.Clear();
					cell.Controls.Add(lnk);
				}
			}
		}

		protected void grdStatsBookMarkView_SelectedIndexChanged(object sender, EventArgs e)
		{

		}


		private void LoadMemberList()
		{
			string queryString =
				"SELECT GroupMemberID, FirstName, LastName, FirstName + ' ' + LastName as FullName FROM GroupMember Order By LastName, FirstName;";

			int personID;
			int initialIndex = 0;
			personID = int.Parse(Request.QueryString["SelectedPerson"].ToString());

			ddlMemberID.Items.Clear();

			ListItem groupAverage = new ListItem("Averages", "0");
			ddlMemberID.Items.Add(groupAverage);

			using (SqlConnection connection =
								new SqlConnection(_ConnectionSettings.ConnectionString))
			{
				SqlCommand command =
						new SqlCommand(queryString, connection);
				connection.Open();

				SqlDataReader reader = command.ExecuteReader();

				// Call Read before accessing data.
				while (reader.Read())
				{
					ListItem groupMember = new ListItem(reader[3].ToString(), reader[0].ToString());
					ddlMemberID.Items.Add(groupMember);

					if (int.Parse(reader[0].ToString()) == personID)
					{
						initialIndex = ddlMemberID.Items.Count - 1;
					}

				}
				// Call Close when done reading.
				reader.Close();
			}

			ddlMemberID.SelectedIndex = initialIndex;
		}

		protected void ddlMemberID_SelectedIndexChanged(Object sender, EventArgs e)
		{
			Server.Transfer("~/MarkingStats.aspx?SelectedPerson=" + ddlMemberID.SelectedItem.Value);
		}

	}
}