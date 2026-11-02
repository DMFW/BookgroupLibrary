using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookgroupLibrary
{
	public partial class SecretSantasListForm : System.Web.UI.Page
	{

		private bool _useAlternateColour = false;
		private string _previousGroupValue = string.Empty;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ViewState["CurrentSortField"] = "ChristmasYear";
			}
		}

		private string PreviousGroupValue
		{
			get { return ViewState["PreviousGroupValue"] as string ?? ""; }
			set { ViewState["PreviousGroupValue"] = value; }
		}

		private bool AlternateGroup
		{
			get { return (bool?)ViewState["AlternateGroup"] ?? false; }
			set { ViewState["AlternateGroup"] = value; }
		}

		protected void grdSantasList_Sorting(object sender, GridViewSortEventArgs e)
		{
			ViewState["CurrentSortField"] = e.SortExpression;
		}

		protected void grdSantasList_SelectedIndexChanged(object sender, EventArgs e)
		{

			int rowIndex = grdSantasList.SelectedIndex;
			string selectedBookID = grdSantasList.DataKeys[rowIndex].Value.ToString();
			Server.Transfer("~/BookDetail.aspx?SelectedBookID=" + selectedBookID, true);
		}

		protected void grdSantasList_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType != DataControlRowType.DataRow)
				return;

			string sortField =
					ViewState["CurrentSortField"]?.ToString()
					?? "ChristmasYear";

			string currentValue =
					DataBinder.Eval(e.Row.DataItem, sortField)
					?.ToString() ?? "";

			string cssClass = "";

			if (currentValue != PreviousGroupValue)
			{
				AlternateGroup = !AlternateGroup;
				PreviousGroupValue = currentValue;

				cssClass += " bookGridGroupStart";
			}

			cssClass += AlternateGroup
					? " bookGridGroupA"
					: " bookGridGroupB";

			e.Row.CssClass += cssClass;

		}


	}
}