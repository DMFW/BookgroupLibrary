using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using SharedCode;

namespace BookgroupLibrary
{
	public partial class Diary : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["SignedOnUser"] != null)
			{
				// Allowed to edit so enable the editing controls
				grdForthcomingMeetings.Columns[0].Visible = true;
				btnAddMeeting.Visible = true;
			}
		}

		protected void grdForthcomingMeetings_DataBound(Object grid, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				DateTime dteMeetingDate;
				Label lblMeetingDate = (Label)e.Row.FindControl("lblMeetingDate");
				if (lblMeetingDate != null)
				{
					if (DateTime.TryParse(lblMeetingDate.Text, out dteMeetingDate))
					{
						int SecondMonday = GetDateForWeekDay(DayOfWeek.Monday, 2, dteMeetingDate.Month, dteMeetingDate.Year);
						int FourthMonday = GetDateForWeekDay(DayOfWeek.Monday, 4, dteMeetingDate.Month, dteMeetingDate.Year);

						lblMeetingDate.CssClass = "datewarningtext";
						if (dteMeetingDate.Day == SecondMonday)
						{
							lblMeetingDate.CssClass = "datemaintext";
						}
						if (dteMeetingDate.Day == FourthMonday)
						{
							lblMeetingDate.CssClass = "datesantatext";
						}
					}
				}
			}

		}

		protected void grdForthcomingMeetings_SelectedIndexChanged(Object sender, EventArgs e)
		{

			// Selected index changed is triggered by the "Edit" hyperlink 

			pnlEditingArea.Visible = true;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Edit Existing Meeting";

			btnAddMeeting.Enabled = false;

			btnUpdate.Text = "Update";
			dtvMeeting.ChangeMode(DetailsViewMode.Edit);
			dtvMeeting.DataBind();

			// We are editing and existing row so to keep thing simple stop other edits and deletes until we are finished
			grdForthcomingMeetings.Columns[0].Visible = false;

		}

		protected void btnAddMeeting_Click(object sender, EventArgs e)
		{
			pnlEditingArea.Visible = true;
			btnAddMeeting.Enabled = false;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Add New Meeting";

			btnUpdate.Text = "Insert";
			dtvMeeting.ChangeMode(DetailsViewMode.Insert);
			dtvMeeting.DataBind();

			// We are adding new data so to keep thing simple stop edits and deletes until we are finished
			grdForthcomingMeetings.Columns[0].Visible = false;

		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			dtvMeeting.DataBind();
			btnAddMeeting.Enabled = true;
			pnlEditingArea.Visible = false;
			// Go back to allowing edits and deletes again
			grdForthcomingMeetings.Columns[0].Visible = true;
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			Page.Validate();

			if (Page.IsValid)
			{
				if (dtvMeeting.CurrentMode == DetailsViewMode.Insert)
				{
					dtvMeeting.InsertItem(true);
				}

				if (dtvMeeting.CurrentMode == DetailsViewMode.Edit)
				{
					dtvMeeting.UpdateItem(true);
				}

				if (Page.IsValid)
				{
					// To ensure that we display errors if the update failed, only return when page valid.
					grdForthcomingMeetings.DataBind();
					btnAddMeeting.Enabled = true;
					pnlEditingArea.Visible = false;
					// Go back to allowing edits and deletes again
					grdForthcomingMeetings.Columns[0].Visible = true;
				}
			}
		}

		#region DataSyncOperations

		protected void dtvMeeting_PreRender(object sender, EventArgs e)
		{
			if (dtvMeeting.CurrentMode == DetailsViewMode.Edit)
			{
				DropDownList ddlLeader;
				ddlLeader = (DropDownList)dtvMeeting.FindControl("ddlLeader");
				Label lblLeaderID;
				lblLeaderID = (Label)dtvMeeting.FindControl("lblLeaderID");

				if ((ddlLeader != null) & (lblLeaderID != null))
				{
					ddlLeader.SelectedValue = lblLeaderID.Text;
				}

				DropDownList ddlBook;
				ddlBook = (DropDownList)dtvMeeting.FindControl("ddlBook");
				Label lblBookID;
				lblBookID = (Label)dtvMeeting.FindControl("lblBookID");

				if ((ddlBook != null) & (lblBookID != null))
				{
					ddlBook.SelectedValue = lblBookID.Text;
				}
			}
		}

		protected void ddlListRequiringUnselectedItem_DataBound(object sender, EventArgs e)
		{
			DropDownList ddlListRequiringUnselectedItem = (DropDownList)sender;
			ddlListRequiringUnselectedItem.Items.Insert(0, new ListItem("--Select---", "0"));
		}

		#endregion DataSyncOperations

		protected void dtvMeeting_ItemInserting(object sender, EventArgs e)
		{

			DropDownList ddlBook = (DropDownList)dtvMeeting.FindControl("ddlBook");

			if (ddlBook.SelectedValue != "0")
			{
				SqlDataSourceDiaryEntry.InsertParameters["BookID"].DefaultValue = ddlBook.SelectedValue;
			}

			DropDownList ddlLeader = (DropDownList)dtvMeeting.FindControl("ddlLeader");

			if (ddlLeader.SelectedValue != "0")
			{
				SqlDataSourceDiaryEntry.InsertParameters["LeaderID"].DefaultValue = ddlLeader.SelectedValue;
			}

		}

		protected void dtvMeeting_ItemUpdating(object sender, EventArgs e)
		{

			Label lblMeetingID = (Label)dtvMeeting.FindControl("lblMeetingID");
			SqlDataSourceDiaryEntry.UpdateParameters["MeetingID"].DefaultValue = lblMeetingID.Text;

			TextBox txtMeetingDate = (TextBox)dtvMeeting.FindControl("txtMeetingDate");
			if (txtMeetingDate.Text != "")
			{
				SqlDataSourceDiaryEntry.UpdateParameters["MeetingDate"].DefaultValue = txtMeetingDate.Text;
			}

			DropDownList ddlBook = (DropDownList)dtvMeeting.FindControl("ddlBook");
			if (ddlBook.SelectedValue != "0")
			{
				SqlDataSourceDiaryEntry.UpdateParameters["BookID"].DefaultValue = ddlBook.SelectedValue;
			}
			else
			{
				SqlDataSourceDiaryEntry.UpdateParameters["BookID"].DefaultValue = ""; // Will be converted to null
			}

			DropDownList ddlLeader = (DropDownList)dtvMeeting.FindControl("ddlLeader");
			if (ddlLeader.SelectedValue != "0")
			{
				SqlDataSourceDiaryEntry.UpdateParameters["LeaderID"].DefaultValue = ddlLeader.SelectedValue;
			}
			else
			{
				SqlDataSourceDiaryEntry.UpdateParameters["LeaderID"].DefaultValue = ""; // Will be converted to null
			}

			TextBox txtComments = (TextBox)dtvMeeting.FindControl("txtComments");
			if (txtMeetingDate.Text != "")
			{
				SqlDataSourceDiaryEntry.UpdateParameters["Comments"].DefaultValue = txtComments.Text;
			}

		}

		protected void valMeetingDate_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			CustomValidator cv = (CustomValidator)source;
			TextBox txtMeetingDate = (TextBox)dtvMeeting.FindControl("txtMeetingDate");

			CultureInfo culture;
			DateTimeStyles styles;
			DateTime dteMeetingDate;

			culture = CultureInfo.CurrentCulture;
			styles = DateTimeStyles.AssumeLocal;

			if (txtMeetingDate.Text == null)
			{
				// This is valid
			}
			else
			{
				txtMeetingDate.Text.Trim();
				if (txtMeetingDate.Text == "")
				{
					// This is valid
				}
				else
				{
					if (!DateTime.TryParse(txtMeetingDate.Text, culture, styles, out dteMeetingDate))
					{
						cv.ErrorMessage = "The meeting date entered is not a valid date.";
						args.IsValid = false;
					}
					else
					{
						if (dteMeetingDate < DateTime.Now)
						{
							cv.ErrorMessage = "The meeting date should be in the future. Past meeting records are retained on the database but are hidden on this page.";
							args.IsValid = false;
						}
					}
				}
			}
		}

		protected void SqlDataSourceDiary_Deleting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceDiaryEntry_Inserting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceDiaryEntry_Updating(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceDiaryEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be added for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
			}
		}

		protected void SqlDataSourceDiaryEntry_Updated(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be updated for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
			}
		}

		private int GetDateForWeekDay(DayOfWeek DesiredDay, int Occurrence, int Month, int Year)
		{
			DateTime dtFirstOfMonth = new DateTime(Year, Month, 1);
			int j = 0;
			if (Convert.ToInt32(DesiredDay) - Convert.ToInt32(dtFirstOfMonth.DayOfWeek) >= 0)
				j = Convert.ToInt32(DesiredDay) - Convert.ToInt32(dtFirstOfMonth.DayOfWeek) + 1;
			else
				j = (7 - Convert.ToInt32(dtFirstOfMonth.DayOfWeek)) + (Convert.ToInt32(DesiredDay) + 1);

			return j + (Occurrence - 1) * 7;
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