using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using SharedCode;

namespace BookgroupLibrary.Account
{
    public class BookgroupMembershipUser : MembershipUser
    {

        private string _FirstName;
        private string _LastName;
        private bool _IsAdministrator;

        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }

        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        public string DisplayName
        {
            get { return _FirstName + " " + _LastName; }
        }


        public bool IsAdministrator
        {
            get { return _IsAdministrator; }
            set { _IsAdministrator = value; }
        }

        public BookgroupMembershipUser(string providername,
                                      string username,
                                      object providerUserKey,
                                      string email,
                                      string passwordQuestion,
                                      string comment,
                                      bool isApproved,
                                      bool isLockedOut,
                                      DateTime creationDate,
                                      DateTime lastLoginDate,
                                      DateTime lastActivityDate,
                                      DateTime lastPasswordChangedDate,
                                      DateTime lastLockedOutDate,
                                      string firstName,
                                      string lastName,
                                      bool isAdministrator) :
                                  base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       comment,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate)
        {
            this.IsAdministrator = isAdministrator;
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}