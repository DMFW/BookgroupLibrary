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
	public partial class Links : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["SignedOnUser"] != null)
			{
				// Allowed to edit so enable the editing controls
				grdLinks.Columns[0].Visible = true;
				btnAddLink.Visible = true;
			}
		}

		protected void grdLinks_DataBound(Object grid, GridViewRowEventArgs e)
		{
		
		}

		protected void grdLinks_SelectedIndexChanged(Object sender, EventArgs e)
		{

			// Selected index changed is triggered by the "Edit" hyperlink 

			pnlEditingArea.Visible = true;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Edit Existing Link";

			btnAddLink.Enabled = false;

			btnUpdate.Text = "Update";
			dtvLink.ChangeMode(DetailsViewMode.Edit);
			dtvLink.DataBind();

			// We are editing and existing row so to keep thing simple stop other edits and deletes until we are finished
			grdLinks.Columns[0].Visible = false;

		}

		protected void btnAddLink_Click(object sender, EventArgs e)
		{
			pnlEditingArea.Visible = true;
			btnAddLink.Enabled = false;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Add New Link";

			btnUpdate.Text = "Insert";
			dtvLink.ChangeMode(DetailsViewMode.Insert);
			dtvLink.DataBind();

			// We are adding new data so to keep thing simple stop edits and deletes until we are finished
			grdLinks.Columns[0].Visible = false;

		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			dtvLink.DataBind();
			btnAddLink.Enabled = true;
			pnlEditingArea.Visible = false;
			// Go back to allowing edits and deletes again
			grdLinks.Columns[0].Visible = true;
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			Page.Validate();

			if (Page.IsValid)
			{
				if (dtvLink.CurrentMode == DetailsViewMode.Insert)
				{
					dtvLink.InsertItem(true);
				}

				if (dtvLink.CurrentMode == DetailsViewMode.Edit)
				{
					dtvLink.UpdateItem(true);
				}

				if (Page.IsValid)
				{
					// To ensure that we display errors if the update failed, only return when page valid.
					grdLinks.DataBind();
					btnAddLink.Enabled = true;
					pnlEditingArea.Visible = false;
					// Go back to allowing edits and deletes again
					grdLinks.Columns[0].Visible = true;
				}
			}
		}

		#region DataSyncOperations

		protected void dtvLink_PreRender(object sender, EventArgs e)
		{
			
		}

		protected void ddlListRequiringUnselectedItem_DataBound(object sender, EventArgs e)
		{
			DropDownList ddlListRequiringUnselectedItem = (DropDownList)sender;
			ddlListRequiringUnselectedItem.Items.Insert(0, new ListItem("--Select---", "0"));
		}

		#endregion DataSyncOperations

		protected void dtvLink_ItemInserting(object sender, EventArgs e)
		{
		}

		protected void dtvLink_ItemUpdating(object sender, EventArgs e)
		{

			Label lblLinkID = (Label)dtvLink.FindControl("lblLinkID");
			SqlDataSourceLinkEntry.UpdateParameters["LinkID"].DefaultValue = lblLinkID.Text;

			TextBox txtLinkText = (TextBox)dtvLink.FindControl("txtLinkText");
			if (txtLinkText.Text != "")
			{
				SqlDataSourceLinkEntry.UpdateParameters["LinkText"].DefaultValue = txtLinkText.Text;
			}

			TextBox txtLinkURL = (TextBox)dtvLink.FindControl("txtLinkURL");
			if (txtLinkURL.Text != "")
			{
				SqlDataSourceLinkEntry.UpdateParameters["LinkURL"].DefaultValue = txtLinkURL.Text;
			}

			TextBox txtDisplayRank = (TextBox)dtvLink.FindControl("txtDisplayRank");
			if (txtDisplayRank.Text != "")
			{
				SqlDataSourceLinkEntry.UpdateParameters["DisplayRank"].DefaultValue = txtDisplayRank.Text;
			}

			TextBox txtLinkDescription = (TextBox)dtvLink.FindControl("txtLinkDescription");
			if (txtLinkDescription.Text != "")
			{
				SqlDataSourceLinkEntry.UpdateParameters["LinkDescription"].DefaultValue = txtLinkDescription.Text;
			}
		}

		protected void SqlDataSourceLinks_Deleting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceLinkEntry_Inserting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceLinkEntry_Updating(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceLinkEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be added for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
			}
		}

		protected void SqlDataSourceLinkEntry_Updated(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be updated for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
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