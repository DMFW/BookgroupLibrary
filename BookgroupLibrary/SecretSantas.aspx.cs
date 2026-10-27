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
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdSantasList_SelectedIndexChanged(object sender, EventArgs e)
        {

            int rowIndex = grdSantasList.SelectedIndex;
            string selectedBookID = grdSantasList.DataKeys[rowIndex].Value.ToString();
            Server.Transfer("~/BookDetail.aspx?SelectedBookID=" + selectedBookID, true);
        }
    }
}