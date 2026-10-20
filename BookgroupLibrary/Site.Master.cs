using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SharedCode;

namespace BookgroupLibrary
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (MenuItem menuItem in NavigationMenu.Items)
            {
                menuItem.Enabled = true;

                if (menuItem.NavigateUrl.Contains("Maintenance.aspx"))
                {
                    if (Session["SignedOnUser"] != null)
                    {
                        menuItem.Enabled = true;
                    }
                    else
                    {
                        menuItem.Enabled = false;
                    }
                }   
            }
        }

        protected void Page_PreRender(object Sender, EventArgs e)
        {
            foreach (MenuItem mnuItem in NavigationMenu.Items)
            {
                if (mnuItem.Enabled == false)
                {
                    mnuItem.Text = "<span style='color:Gray'>" + mnuItem.Text + "</span>";
                }

                foreach (MenuItem subMenuItem in mnuItem.ChildItems)
                {
                    if (subMenuItem.Enabled == false)
                    subMenuItem.Text = "<span style='color:Gray'>" + subMenuItem.Text + "</span>";
                }
            }

            Label lblHeadLoginName = (Label)ControlExtensions.FindControlRecursive(Page.Master, "lblHeadLoginName");
            if (lblHeadLoginName != null)
            {
                if (Session["SignedOnUser"] != null)
                {
                    Account.SimpleUserStructure LoggedOnUser = (Account.SimpleUserStructure)Session["SignedOnUser"];
                    lblHeadLoginName.Text = LoggedOnUser.FirstName;
                }
            }
        }

        protected void NavigationMenu_MenuItemClick(object sender, MenuEventArgs e)
        {

        }

    }
}
