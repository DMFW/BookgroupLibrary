using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookgroupLibrary
{
    public partial class OurChoicesListForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdOurList_SelectedIndexChanged(object sender, EventArgs e)
        {

            int rowIndex = grdOurList.SelectedIndex;
            string selectedBookID = grdOurList.DataKeys[rowIndex].Value.ToString();
            Server.Transfer("~/BookDetail.aspx?SelectedBookID=" + selectedBookID, true);
        }
    }
}