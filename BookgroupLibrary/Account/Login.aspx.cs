using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using SharedCode;

namespace BookgroupLibrary.Account
{
    public struct SimpleUserStructure
    {
        public int GroupMemberID;
        public string LoginName;
        public string FirstName;
        public string LastName; 
        public bool IsAdministrator;
    }

    public partial class Login : System.Web.UI.Page
    {

        BookgroupMembershipUser _LoggedOnUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            BookgroupMembershipProvider SiteSecurity = (BookgroupMembershipProvider)Membership.Providers["SiteSecurity"];

            TextBox UserName = (TextBox)ControlExtensions.FindControlRecursive(Page,"UserName");
            TextBox Password = (TextBox)ControlExtensions.FindControlRecursive(Page, "Password");

            try
            {
                if (SiteSecurity.LogOn(UserName.Text, Password.Text))
                {
                    _LoggedOnUser = (BookgroupMembershipUser)SiteSecurity.GetUser(UserName.Text, true);
                    SimpleUserStructure signedOnUser = new SimpleUserStructure();
                    signedOnUser.GroupMemberID = Int32.Parse(_LoggedOnUser.ProviderUserKey.ToString());
                    signedOnUser.LoginName = _LoggedOnUser.UserName;
                    signedOnUser.FirstName = _LoggedOnUser.FirstName;
                    signedOnUser.LastName = _LoggedOnUser.LastName;
                    signedOnUser.IsAdministrator = _LoggedOnUser.IsAdministrator;

                    Session["SignedOnUser"] = signedOnUser;
                }
            }
            catch (Exception error)
            {
                AddValidationMessage(error.Message, "LoginUserValidationGroup");
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
