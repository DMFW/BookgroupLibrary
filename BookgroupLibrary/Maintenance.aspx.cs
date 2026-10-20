using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookgroupLibrary
{
    public partial class Maintenance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGroupMembers_Click(object sender, EventArgs e)
        {
            Server.Transfer("Maintenance/MaintainGroup.aspx", true);
        }

        protected void btnAuthors_Click(object sender, EventArgs e)
        {
            Server.Transfer("Maintenance/MaintainAuthors.aspx", true);
        }

        protected void btnBooks_Click(object sender, EventArgs e)
        {
            // Set book as zero to imply the maintenance function is addition
            Server.Transfer("Maintenance/MaintainBooks.aspx?SelectedBookID=0", true);
        }
    }
}