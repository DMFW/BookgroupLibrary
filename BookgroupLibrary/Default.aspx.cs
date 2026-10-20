using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace BookgroupLibrary
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Logout"] == "True")
            {
                Session["SignedOnUser"] = null;
            }
            else
            {
                if ((HttpContext.Current.User.Identity.Name != "") & (Session["SignedOnUser"] == null))
                {
                    // The security provider is happy we are signed on but the session state has lost the
                    // little local simple structure we like to port around, so go recover it and we are signed on...

                    Account.BookgroupMembershipProvider SiteSecurity = (Account.BookgroupMembershipProvider)Membership.Providers["SiteSecurity"];
                    Account.SimpleUserStructure signedOnUser;

                    Account.BookgroupMembershipUser LoggedOnUser = (Account.BookgroupMembershipUser)SiteSecurity.GetUser(HttpContext.Current.User.Identity.Name, true);
                    signedOnUser = new Account.SimpleUserStructure();
                    signedOnUser.GroupMemberID = Int32.Parse(LoggedOnUser.ProviderUserKey.ToString());
                    signedOnUser.LoginName = LoggedOnUser.UserName;
                    signedOnUser.FirstName = LoggedOnUser.FirstName;
                    signedOnUser.LastName = LoggedOnUser.LastName;
                    signedOnUser.IsAdministrator = LoggedOnUser.IsAdministrator;

                    Session["SignedOnUser"] = signedOnUser;
                }
            }
        }
    }
}
