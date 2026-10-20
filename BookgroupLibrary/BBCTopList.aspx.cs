using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookgroupLibrary
{
    public partial class BBCTopListForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdBBCList_SelectedIndexChanged(object sender, EventArgs e)
        {

            int rowIndex = grdBBCList.SelectedIndex;
            string selectedBookID = grdBBCList.DataKeys[rowIndex].Value.ToString();
            Server.Transfer("~/BookDetail.aspx?SelectedBookID=" + selectedBookID, true);
        }
    }
}