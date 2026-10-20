using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.SqlClient;
using SharedCode;

namespace BookgroupLibrary.Account
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        Account.SimpleUserStructure _SignedOnUser;
        private int _SelectedGroupMemberID;
        private System.Configuration.ConnectionStringSettings _ConnectionSettings;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["SignedOnUser"] != null)
            {
                _SignedOnUser = (Account.SimpleUserStructure)Session["SignedOnUser"];
            }
            else
            {
                // We shouldn't be on this page so however we got here, reject
                Server.Transfer("Account/UnauthorisedPage.aspx", true);
            }

            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");

            if (Request.QueryString["GroupMemberID"] == "")
            {
                // Someone is trying to hack the page and they don't realise it needs a query string....
                Server.Transfer("Account/UnauthorisedPage.aspx?RedirectionError=01", true);
            }

            bool requestedGroupMemberNumeric;
            requestedGroupMemberNumeric = Int32.TryParse(Request.QueryString["GroupMemberID"].ToString(), out _SelectedGroupMemberID);

            if (!requestedGroupMemberNumeric)
            {
                // Someone is trying to hack the page and they can't get the format of the query string right....
                Server.Transfer("Account/UnauthorisedPage.aspx?RedirectionError=02", true);
            }

            if (_SelectedGroupMemberID != _SignedOnUser.GroupMemberID)
            {
                // Someone is trying to hack the page and they aren't in agreement with the signed on user.
                Server.Transfer("Account/UnauthorisedPage.aspx?RedirectionError=03", true);      
            }

            if (!Page.IsPostBack)
            {
                SetPageLayout();
            }
        }

        private void SetPageLayout()
        {
            Label lblPasswordIdentity = (Label)ControlExtensions.FindControlRecursive(Page, "lblPasswordIdentity");
            lblPasswordIdentity.Text = _SignedOnUser.LoginName + " [" + _SignedOnUser.FirstName + " " + _SignedOnUser.LastName + "]";
        }

        protected void ChangeUserPassword_Cancel(object sender, EventArgs args)
        {
            Server.Transfer("~/Maintenance/MaintainGroup.aspx", true);
        }

   }
}
