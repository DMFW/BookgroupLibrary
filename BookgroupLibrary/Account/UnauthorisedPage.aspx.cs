using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SharedCode;

namespace BookgroupLibrary.Account
{
    public partial class UnauthorisedPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String redirectionCode = Request.QueryString["RedirectionError"];
            Label lblSpecificRedirectionError = (Label)ControlExtensions.FindControlRecursive(Page,"lblSpecificRedirectionError");

            if (redirectionCode != null)
            {
                switch (redirectionCode)
                {
                    case "01":
                        lblSpecificRedirectionError.Text = "Change password page not invoked correctly through group maintenance [Error = 01].";
                        break;
                    case "02":
                        lblSpecificRedirectionError.Text = "Change password page intercept with invalid query parameters [Error = 02].";
                        break;
                    case "03":
                        lblSpecificRedirectionError.Text = "Change password page may only be used for the signed on user [Error = 03].";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}