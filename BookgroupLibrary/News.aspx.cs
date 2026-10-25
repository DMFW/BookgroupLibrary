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
	public partial class News : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["SignedOnUser"] != null)
			{
				// Allowed to edit so enable the editing controls
				grdNews.Columns[0].Visible = true;
				btnAddNews.Visible = true;
			}
		}

		protected void grdNews_DataBound(Object grid, GridViewRowEventArgs e)
		{
		
		}

		protected void grdNews_SelectedIndexChanged(Object sender, EventArgs e)
		{

			// Selected index changed is triggered by the "Edit" hyperlink 

			pnlEditingArea.Visible = true;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Edit Existing Link";

			btnAddNews.Enabled = false;

			btnUpdate.Text = "Update";
			dtvNews.ChangeMode(DetailsViewMode.Edit);
			dtvNews.DataBind();

			// We are editing and existing row so to keep thing simple stop other edits and deletes until we are finished
			grdNews.Columns[0].Visible = false;

		}

		protected void btnAddNews_Click(object sender, EventArgs e)
		{
			pnlEditingArea.Visible = true;
			btnAddNews.Enabled = false;

			Label lblEditType = (Label)pnlEditingArea.FindControl("lblEditType");
			lblEditType.Text = "Add A News Item";

			btnUpdate.Text = "Insert";
			dtvNews.ChangeMode(DetailsViewMode.Insert);
			dtvNews.DataBind();

			// We are adding new data so to keep thing simple stop edits and deletes until we are finished
			grdNews.Columns[0].Visible = false;

		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			dtvNews.DataBind();
			btnAddNews.Enabled = true;
			pnlEditingArea.Visible = false;
			// Go back to allowing edits and deletes again
			grdNews.Columns[0].Visible = true;
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			Page.Validate();

			if (Page.IsValid)
			{
				if (dtvNews.CurrentMode == DetailsViewMode.Insert)
				{
					dtvNews.InsertItem(true);
				}

				if (dtvNews.CurrentMode == DetailsViewMode.Edit)
				{
					dtvNews.UpdateItem(true);
				}

				if (Page.IsValid)
				{
					// To ensure that we display errors if the update failed, only return when page valid.
					grdNews.DataBind();
					btnAddNews.Enabled = true;
					pnlEditingArea.Visible = false;
					// Go back to allowing edits and deletes again
					grdNews.Columns[0].Visible = true;
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

		protected void dtvNews_PreRender(object sender, EventArgs e)
		{

		}

		protected void dtvNews_ItemInserting(object sender, EventArgs e)
		{
		}

		protected void dtvNews_ItemUpdating(object sender, EventArgs e)
		{

			Label lblLinkID = (Label)dtvNews.FindControl("lblNewsID");
			SqlDataSourceNewsEntry.UpdateParameters["NewsID"].DefaultValue = lblLinkID.Text;

			TextBox txtLinkText = (TextBox)dtvNews.FindControl("txtNewsText");
			if (txtLinkText.Text != "")
			{
				SqlDataSourceNewsEntry.UpdateParameters["NewsText"].DefaultValue = txtLinkText.Text;
			}

			TextBox txtNewsText = (TextBox)dtvNews.FindControl("txtNewsText");
			if (txtNewsText.Text != "")
			{
				SqlDataSourceNewsEntry.UpdateParameters["NewsText"].DefaultValue = txtNewsText.Text;
			}
		}

		protected void SqlDataSourceNews_Deleting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceNewsEntry_Inserting(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceNewsEntry_Updating(Object source, SqlDataSourceCommandEventArgs e)
		{

		}

		protected void SqlDataSourceNewsEntry_Inserted(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be added for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
			}
		}

		protected void SqlDataSourceNewsEntry_Updated(object sender, SqlDataSourceStatusEventArgs e)
		{
			if (e.Exception != null)
			{
				AddValidationMessage("The record could not be updated for the technical reason '" + e.Exception.Message + "'", "Meeting");
				// make the error handled so it does not take over the whole page
				e.ExceptionHandled = true;
			}
		}

		protected void valDatePosted_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			CustomValidator cv = (CustomValidator)source;
			TextBox txtDatePosted = (TextBox)dtvNews.FindControl("txtDatePosted");

			CultureInfo culture;
			DateTimeStyles styles;
			DateTime dteDatePosted;

			culture = CultureInfo.CurrentCulture;
			styles = DateTimeStyles.AssumeLocal;

			if (txtDatePosted.Text == null)
			{
				// This is valid
			}
			else
			{
				txtDatePosted.Text.Trim();
				if (txtDatePosted.Text == "")
				{
					// This is valid
				}
				else
				{
					if (!DateTime.TryParse(txtDatePosted.Text, culture, styles, out dteDatePosted))
					{
						cv.ErrorMessage = "The date posted is not a valid date.";
						args.IsValid = false;
					}
				}
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