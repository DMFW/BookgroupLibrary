using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.SessionState;
using SharedCode;

namespace BookgroupLibrary.Account
{
    public sealed class BookgroupMembershipProvider : MembershipProvider
    {

    private int    newPasswordLength = 8;
    private string eventSource = "SqlMembershipProvider";
    private string eventLog = "Application";
    private string exceptionMessage = "An exception occurred. Please check the Event Log.";
    private string connectionString;

    //
    // Used when determining encryption key values.
    //

    private MachineKeySection machineKey;

    //
    // If false, exceptions are thrown to the caller. If true,
    // exceptions are written to the event log.
    //

    private bool pWriteExceptionsToEventLog;

    public bool WriteExceptionsToEventLog
    {
      get { return pWriteExceptionsToEventLog; }
      set { pWriteExceptionsToEventLog = value; }
    }


    //
    // System.Configuration.Provider.ProviderBase.Initialize Method
    //

    public override void Initialize(string name, NameValueCollection config)
    {
      //
      // Initialize values from web.config.
      //

      if (config == null)
        throw new ArgumentNullException("config");

      if (name == null || name.Length == 0)
        name = "SiteSecurity";

      // Initialize the abstract base class.
      base.Initialize(name, config);

      pApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
      pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
      pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
      pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonalphanumericCharacters"], "0"));
      pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
      pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
      pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
      pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
      pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
      pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "false"));
      pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

      string temp_format = config["passwordFormat"];
      if (temp_format == null)
      {
        temp_format = "Hashed";
      }

      switch (temp_format)
      {
        case "Hashed":
          pPasswordFormat = MembershipPasswordFormat.Hashed;
          break;
        case "Encrypted":
          pPasswordFormat = MembershipPasswordFormat.Encrypted;
          break;
        case "Clear":
          pPasswordFormat = MembershipPasswordFormat.Clear;
          break;
        default:
          throw new ProviderException("Password format not supported.");
      }

    
      ConnectionStringSettings ConnectionStringSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");

      if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
      {
        throw new ProviderException("Connection string cannot be blank.");
      }

      connectionString = ConnectionStringSettings.ConnectionString;

      // Get encryption and decryption key information from the configuration.
      Configuration cfg =
        WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
      machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

      if (machineKey.ValidationKey.Contains("AutoGenerate"))
        if (PasswordFormat != MembershipPasswordFormat.Clear)
          throw new ProviderException("Hashed or Encrypted passwords " +
                                      "are not supported with auto-generated keys.");
    }


    //
    // A helper function to retrieve config values from the configuration file.
    //

    private string GetConfigValue(string configValue, string defaultValue)
    {
      if (String.IsNullOrEmpty(configValue))
        return defaultValue;

      return configValue;
    }


    //
    // System.Web.Security.MembershipProvider properties.
    //


    private string pApplicationName;
    private bool   pEnablePasswordReset;
    private bool   pEnablePasswordRetrieval;
    private bool   pRequiresQuestionAndAnswer;
    private bool   pRequiresUniqueEmail;
    private int    pMaxInvalidPasswordAttempts;
    private int    pPasswordAttemptWindow;
    private MembershipPasswordFormat pPasswordFormat;

    public override string ApplicationName
    {
      get { return pApplicationName; }
      set { pApplicationName = value; }
    } 

    public override bool EnablePasswordReset
    {
      get { return pEnablePasswordReset; }
    }


    public override bool EnablePasswordRetrieval
    {
      get { return pEnablePasswordRetrieval; }
    }


    public override bool RequiresQuestionAndAnswer
    {
      get { return pRequiresQuestionAndAnswer; }
    }


    public override bool RequiresUniqueEmail
    {
      get { return pRequiresUniqueEmail; }
    }


    public override int MaxInvalidPasswordAttempts
    {
      get { return pMaxInvalidPasswordAttempts; }
    }


    public override int PasswordAttemptWindow
    {
      get { return pPasswordAttemptWindow; }
    }


    public override MembershipPasswordFormat PasswordFormat
    {
      get { return pPasswordFormat; }
    }

    private int pMinRequiredNonAlphanumericCharacters;

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { return pMinRequiredNonAlphanumericCharacters; }
    }

    private int pMinRequiredPasswordLength;

    public override int MinRequiredPasswordLength
    {
      get { return pMinRequiredPasswordLength; }
    }

    private string pPasswordStrengthRegularExpression;

    public override string PasswordStrengthRegularExpression
    {
      get { return pPasswordStrengthRegularExpression; }
    }

    //
    // System.Web.Security.MembershipProvider methods.
    //

    //
    // MembershipProvider.ChangePassword
    //

    public override bool ChangePassword(string username, string oldPwd, string newPwd)
    {
        SimpleUserStructure signedOnUser;

        signedOnUser = (SimpleUserStructure)System.Web.HttpContext.Current.Session["SignedOnUser"];
     
        if (!signedOnUser.IsAdministrator)
        {
            if (!ValidateUser(username, oldPwd))
                return false;
        }

        ValidatePasswordEventArgs args = 
        new ValidatePasswordEventArgs(username, newPwd, true);

        OnValidatingPassword(args);

        if (args.Cancel)
        if (args.FailureInformation != null)
            throw args.FailureInformation;
        else
            throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

        string salt = CreateSalt(128, 128);

        SqlConnection conn = new SqlConnection(connectionString);
        SqlCommand cmd = new SqlCommand("UPDATE GroupMember " +
                " SET Password = @Password, Salt = @Salt, LastPasswordChangedDate = @LastPasswordChangedDate," +
                " FailedPasswordAttemptCount = 0" +
                " WHERE LoginName = @LoginName", conn);

        cmd.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = EncodePassword(newPwd, salt);
        cmd.Parameters.Add("@Salt", SqlDbType.VarChar, 128).Value = salt;
        cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
        cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      
        int rowsAffected = 0;

        try
        {
        conn.Open();

        rowsAffected = cmd.ExecuteNonQuery();
        }
        catch (SqlException e)
        {
        if (WriteExceptionsToEventLog)
        {
            WriteToEventLog(e, "ChangePassword");

            throw new ProviderException(exceptionMessage);
        }
        else
        {
            throw e;
        }
        }
        finally
        {
        conn.Close();
        }

        if (rowsAffected > 0)
        {
        return true;
        }

        return false;
    }

    private static string CreateSalt(int rawsize, int cookedsize)
    {
        // This class will create a new salt everytime the password changes.
        // Arguagly it is overkill (in fact arguagbly hashing and salting the passwords for this
        // site at all is overkill) but I am trying to get into good habits....

        string saltSprinkled;

        //Generate a cryptographic random number.
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] buff = new byte[rawsize];
        rng.GetBytes(buff);

        string fullSaltShaker = Convert.ToBase64String(buff);

        if (fullSaltShaker.Length >= cookedsize)
        {
            saltSprinkled = fullSaltShaker.Substring(0, cookedsize);
        }
        else
        {
            saltSprinkled = fullSaltShaker + fullSaltShaker.Substring(0, fullSaltShaker.Length - cookedsize);
        }

        Debug.Assert(saltSprinkled.Length == cookedsize);

        // Return a Base64 string representation of the random number at the cookedsiz
        return saltSprinkled;
    }

    private string ReadSalt(string LoginName)
    {
        string queryString = "Select Salt from GroupMember where LoginName = @LoginName";
        string salt;

        using (SqlConnection connection =
                      new SqlConnection(connectionString))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            salt = reader[0].ToString();
            reader.Close();
        }
        return salt;        
    }

    //
    // MembershipProvider.ChangePasswordQuestionAndAnswer
    //

    public override bool ChangePasswordQuestionAndAnswer(string username,
                  string password,
                  string newPwdQuestion,
                  string newPwdAnswer)
    {
        
      if (!ValidateUser(username, password))
          return false;

      string salt = ReadSalt(username);

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("UPDATE GroupMember " +
              " SET PasswordQuestion = @PwdQuestion, PasswordAnswer = @PwdAnswer" +
              " WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@Question", SqlDbType.VarChar, 255).Value = newPwdQuestion;
      cmd.Parameters.Add("@Answer", SqlDbType.VarChar, 255).Value = EncodePassword(newPwdAnswer,salt);
      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = pApplicationName;


      int rowsAffected = 0;

      try
      {
        conn.Open();

        rowsAffected = cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }

      if (rowsAffected > 0)
      {
        return true;
      }

      return false;
    }



    //
    // MembershipProvider.CreateUser
    //

    public override MembershipUser CreateUser(string username,
             string password,
             string email,
             string passwordQuestion,
             string passwordAnswer,
             bool isApproved,
             object providerUserKey,
             out MembershipCreateStatus status)
    {
      ValidatePasswordEventArgs args = 
        new ValidatePasswordEventArgs(username, password, true);

      OnValidatingPassword(args);

      if (args.Cancel)
      {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }

      if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
      {
        status = MembershipCreateStatus.DuplicateEmail;
        return null;
      }

      MembershipUser u = GetUser(username, false);

      if (u == null)
      {
        DateTime createDate = DateTime.Now;

        if (providerUserKey == null)
        {
          providerUserKey = Guid.NewGuid();
        }
        else
        {
          if ( !(providerUserKey is Guid) )
          {
            status = MembershipCreateStatus.InvalidProviderUserKey;
            return null;
          }
        }

        string salt = CreateSalt(128, 128);

        SqlConnection conn = new SqlConnection(connectionString);
        SqlCommand cmd = new SqlCommand("INSERT INTO GroupMember " +
              " (GroupMemberID, Sex, EmailAddress, Administrator," +
              " LoginName, Password, Salt, EmailAddress, PasswordQuestion," +
              " PasswordAnswer, LastPasswordChangedDate, " +
              " ApplicationName, IsLockedOut, LastLockedOutDate," +
              " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " +
              " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" +
              " Values(@GroupMemberID, @Sex, @EmailAddress, @Administrator, @LoginName, @Password, @Salt," + 
                        "@PasswordQuestion, @PasswordAnswer," +
                        "@LastPasswordChangedDate, @IsLockedOut, @LastLockedOutDate," + 
                        "@FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart," +
                        "@FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart", conn);

        // The custom parts of the table not including nullable fields such as true names.

        cmd.Parameters.Add("@GroupMemberID", SqlDbType.UniqueIdentifier).Value = providerUserKey;
        cmd.Parameters.Add("@Sex", SqlDbType.Bit).Value = 0; // Cannot be null so default to female
        cmd.Parameters.Add("@EmailAddress", SqlDbType.VarChar, 128).Value = email;
        cmd.Parameters.Add("@Administrator", SqlDbType.Bit).Value = 0; // Default to not an administrator
        cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
   
        // The standard parts of the table

        cmd.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = EncodePassword(password, salt);
        cmd.Parameters.Add("@Salt", SqlDbType.VarChar, 128).Value = salt;
        cmd.Parameters.Add("@PasswordQuestion", SqlDbType.VarChar, 255).Value = passwordQuestion;
        cmd.Parameters.Add("@PasswordAnswer", SqlDbType.VarChar, 255).Value = EncodePassword(passwordAnswer, salt);
        cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = createDate;
        cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = false;
        cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = createDate;
        cmd.Parameters.Add("@FailedPasswordAttemptCount", SqlDbType.Int).Value = 0;
        cmd.Parameters.Add("@FailedPasswordAttemptWindowStart", SqlDbType.DateTime).Value = createDate;
        cmd.Parameters.Add("@FailedPasswordAnswerAttemptCount", SqlDbType.Int).Value = 0;
        cmd.Parameters.Add("@FailedPasswordAnswerAttemptWindowStart", SqlDbType.DateTime).Value = createDate;

        try
        {
          conn.Open();

          int recAdded = cmd.ExecuteNonQuery();

          if (recAdded > 0)
          {
            status = MembershipCreateStatus.Success;
          }
          else
          {
            status = MembershipCreateStatus.UserRejected;
          }
        }
        catch (SqlException e)
        {
          if (WriteExceptionsToEventLog)
          {
            WriteToEventLog(e, "CreateUser");
          }

          status = MembershipCreateStatus.ProviderError;
        }
        finally
        {
          conn.Close();
        }


        return GetUser(username, false);      
      }        
      else
      {
        status = MembershipCreateStatus.DuplicateUserName;
      }


      return null;
    }


    //
    // MembershipProvider.DeleteUser
    //

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      
      // Deleting users will NOT be supported in the application but the method is provided
      // for illustrative purposes only. It would need expanding anyway to remove relations.
  
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("DELETE FROM GroupMember " +
              " WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      
      int rowsAffected = 0;

      try
      {
        conn.Open();

        rowsAffected = cmd.ExecuteNonQuery();

        if (deleteAllRelatedData)
        {
          // Process commands to delete all data for the user in the database.
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "DeleteUser");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }

      if (rowsAffected > 0)
        return true;

      return false;
    }


    //
    // MembershipProvider.GetAllUsers
    //

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM GroupMembers", conn);

      MembershipUserCollection users = new MembershipUserCollection();

      SqlDataReader reader = null;
      totalRecords = 0;

      try
      {
        conn.Open();
        totalRecords = (int)cmd.ExecuteScalar();

        if (totalRecords <= 0) { return users; }

        cmd.CommandText = "SELECT GroupMemberID, LoginName, EmailAddress, PasswordQuestion," +
                 " IsLockedOut, CreationDate, LastLoginDate," +
                 " LastPasswordChangedDate, LastLockedOutDate, IsApproved " +
                 " FROM GroupMember " +
                 " ORDER BY LoginName Asc";

        reader = cmd.ExecuteReader();

        int counter = 0;
        int startIndex = pageSize * pageIndex;
        int endIndex = startIndex + pageSize - 1;

        while (reader.Read())
        {
          if (counter >= startIndex)
          {
            MembershipUser u = GetUserFromReader(reader);
            users.Add(u);
          }

          if (counter >= endIndex) { cmd.Cancel(); }

          counter++;
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetAllUsers ");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }
         conn.Close();
      }

      return users;
    }


    //
    // MembershipProvider.GetNumberOfUsersOnline
    //

    public override int GetNumberOfUsersOnline()
    {

      TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
      DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM GroupMember " +
              " WHERE LastLogOn > @CompareDate", conn);

      cmd.Parameters.Add("@CompareDate", SqlDbType.DateTime).Value = compareTime;
     
      int numOnline = 0;

      try
      {
        conn.Open();

        numOnline = (int)cmd.ExecuteScalar();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetNumberOfUsersOnline");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }

      return numOnline;
    }

    //
    // MembershipProvider.GetPassword
    //

    public override string GetPassword(string username, string answer)
    {
      if (!EnablePasswordRetrieval)
      {
        throw new ProviderException("Password Retrieval Not Enabled.");
      }

      if (PasswordFormat == MembershipPasswordFormat.Hashed)
      {
        throw new ProviderException("Cannot retrieve Hashed passwords.");
      }

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Password, Salt, PasswordAnswer, IsLockedOut FROM GroupMember " +
            " WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
     
      string password = "";
      string salt = "";
      string passwordAnswer = "";
      SqlDataReader reader = null;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

        if (reader.HasRows)
        {
          reader.Read();

          if ((bool)reader["IsLockedOut"])
            throw new MembershipPasswordException("The supplied user is locked out.");

          password = reader["Password"].ToString();
          salt = reader["Salt"].ToString();
          passwordAnswer = reader["PasswordAnswer"].ToString();
        }
        else
        {
          throw new MembershipPasswordException("The supplied user name is not found.");
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetPassword");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }
        conn.Close();
      }


      if (RequiresQuestionAndAnswer && !CheckPassword(answer, salt, passwordAnswer))
      {
        UpdateFailureCount(username, "passwordAnswer");

        throw new MembershipPasswordException("Incorrect password answer.");
      }


      if (PasswordFormat == MembershipPasswordFormat.Encrypted)
      {
        password = UnEncodePassword(password);
      }

      return password;
    }



    //
    // MembershipProvider.GetUser(string, bool)
    //

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
        SqlConnection conn = new SqlConnection(connectionString);
        SqlCommand cmd = new SqlCommand("SELECT GroupMemberID, LoginName, EmailAddress, PasswordQuestion," +
            " IsLockedOut, LastLogOn, LastPasswordChangedDate, LastLockedOutDate, IsApproved," +
            " FirstName, LastName, Administrator" +
            " FROM GroupMember WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
     
      BookgroupMembershipUser u = null;
      SqlDataReader reader = null;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
          reader.Read();
          u = GetUserFromReader(reader);
        }

      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUser(String, Boolean)");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }

        conn.Close();
      }

      return u;      
    }


    //
    // MembershipProvider.GetUser(object, bool)
    //

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT GroupMemberID, LoginName, Email, PasswordQuestion," +
            " IsLockedOut, CreationDate, LastLoginDate," +
            " LastPasswordChangedDate, LastLockedOutDate, IsApproved," +
            " FirstName, LastName, Administrator " +
            " FROM GroupMember WHERE GroupMemberID = @GroupMemberID", conn);

      cmd.Parameters.Add("@GroupMemberID", SqlDbType.UniqueIdentifier).Value = providerUserKey;

      BookgroupMembershipUser u = null;
      SqlDataReader reader = null;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
          reader.Read();
          u = GetUserFromReader(reader);
        }

      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUser(Object, Boolean)");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }

        conn.Close();
      }

      return u;      
    }


    //
    // GetUserFromReader
    //    A helper function that takes the current row from the OdbcDataReader
    // and hydrates a MembershiUser from the values. Called by the 
    // MembershipUser.GetUser implementation.
    //

    private BookgroupMembershipUser GetUserFromReader(SqlDataReader reader)
    {
      object providerUserKey = reader["GroupMemberID"];
      string username = reader["LoginName"].ToString();
      string email = reader["EmailAddress"].ToString();

      string passwordQuestion = "";
      if (reader["PasswordQuestion"] != DBNull.Value)
          passwordQuestion = reader["PasswordQuestion"].ToString();

      DateTime lastLoginDate = new DateTime();
      if (reader["LastLogOn"] != DBNull.Value)
          lastLoginDate = (DateTime)reader["LastLogOn"];

      DateTime lastPasswordChangedDate = new DateTime();
      if (reader["LastPasswordChangedDate"] != DBNull.Value)
          lastPasswordChangedDate = (DateTime)reader["LastPasswordChangedDate"];

      DateTime lastLockedOutDate = new DateTime();
      if (reader["LastLockedOutDate"] != DBNull.Value)
        lastLockedOutDate = (DateTime)reader["LastLockedOutDate"];

      bool isLockedOut = (bool)reader["IsLockedOut"];
      bool isApproved = (bool)reader["IsApproved"];

      // Parameters in the core security model we just aren't bothering with in this implementation
      DateTime creationDate = DateTime.MinValue;
      DateTime lastActivityDate = DateTime.MinValue;
      string comment = "";

      // Extension parameters for the inhereting class which are not in the core model.
      string firstName = "";
      string lastName = "";
      bool isAdministrator = false;

      if (reader["FirstName"] != DBNull.Value)
          firstName = (string)reader["FirstName"];

      if (reader["LastName"] != DBNull.Value)
          lastName = (string)reader["LastName"];

      if (reader["Administrator"] != DBNull.Value)
          isAdministrator = (bool)reader["Administrator"];  

      BookgroupMembershipUser u = new BookgroupMembershipUser(this.Name,
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
                                            lastLockedOutDate,
                                            firstName,
                                            lastName,
                                            isAdministrator);

      return u;
    }

    //
    // MembershipProvider.UnlockUser
    //

    public override bool UnlockUser(string username)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("UPDATE GroupMember " +
                                        " SET IsLockedOut = False, LastLockedOutDate = @LastLockedOutDate " +
                                        " WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      
      int rowsAffected = 0;

      try
      {
        conn.Open();

        rowsAffected = cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UnlockUser");
          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }

      if (rowsAffected > 0)
        return true;

      return false;      
    }


    //
    // MembershipProvider.GetUserNameByEmail
    //

    public override string GetUserNameByEmail(string email)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT LoginName" +
            " FROM GroupMember WHERE EmailAddress = @EmailAddress", conn);

      cmd.Parameters.Add("@EmailAddress", SqlDbType.VarChar, 128).Value = email;
      
      string username = "";

      try
      {
        conn.Open();

        username = (string)cmd.ExecuteScalar();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetUserNameByEmail");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }

      if (username == null)
        username = "";

      return username;
    }




    //
    // MembershipProvider.ResetPassword
    //

    public override string ResetPassword(string username, string answer)
    {
      if (!EnablePasswordReset)
      {
        throw new NotSupportedException("Password reset is not enabled.");
      }

      if (answer == null && RequiresQuestionAndAnswer)
      {
        UpdateFailureCount(username, "passwordAnswer");

        throw new ProviderException("Password answer required for password reset.");
      }

      string newPassword = 
        System.Web.Security.Membership.GeneratePassword(newPasswordLength,MinRequiredNonAlphanumericCharacters);


      ValidatePasswordEventArgs args = 
        new ValidatePasswordEventArgs(username, newPassword, true);

      OnValidatingPassword(args);

      if (args.Cancel)
        if (args.FailureInformation != null)
          throw args.FailureInformation;
        else
          throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT PasswordAnswer, Salt, IsLockedOut FROM GroupMember WHERE LoginName = @loginName", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
     
      int rowsAffected = 0;
      string passwordAnswer = "";
      string salt = "";
      SqlDataReader reader = null;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

        if (reader.HasRows)
        {
          reader.Read();

          if ((bool)reader["IsLockedOut"])
            throw new MembershipPasswordException("The supplied user is locked out.");

          salt = reader["Salt"].ToString();
          passwordAnswer = reader["PasswordAnswer"].ToString();
        }
        else
        {
          throw new MembershipPasswordException("The supplied user name is not found.");
        }

        if (RequiresQuestionAndAnswer && !CheckPassword(answer, salt, passwordAnswer))
        {
          UpdateFailureCount(username, "passwordAnswer");

          throw new MembershipPasswordException("Incorrect password answer.");
        }

        SqlCommand updateCmd = new SqlCommand("UPDATE GroupMember " +
            " SET Password = @Password, LastPasswordChangedDate = @LastPasswordChangeDate" +
            " WHERE Username = @LoginName AND IsLockedOut = False", conn);

        updateCmd.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = EncodePassword(newPassword, salt);
        updateCmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
        updateCmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
        
        rowsAffected = updateCmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ResetPassword");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }
        conn.Close();
      }

      if (rowsAffected > 0)
      {
        return newPassword;
      }
      else
      {
        throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
      }
    }


    //
    // MembershipProvider.UpdateUser
    //

    public override void UpdateUser(MembershipUser user)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("UPDATE GroupMember " +
              " SET EmailAddress = @EmailAddress, " +
              " WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@EmailAddress", SqlDbType.VarChar, 128).Value = user.Email;
      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = user.UserName;
      
      try
      {
        conn.Open();

        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UpdateUser");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        conn.Close();
      }
    }

    public bool LogOn(string username, string password)
    {

        if (ValidateUser(username, password))
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand updateCmd = new SqlCommand("UPDATE GroupMember SET LastLogOn = @LastLogOn " +
                                                      "WHERE LoginName = @LoginName", conn);

            updateCmd.Parameters.Add("@LastLogOn", SqlDbType.DateTime).Value = DateTime.Now;
            updateCmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;

            updateCmd.ExecuteNonQuery();
            conn.Close();
            return true;
        }
        else
            return false;
    }

    //
    // MembershipProvider.ValidateUser
    //

    public override bool ValidateUser(string username, string password)
    {
      bool isValid = false;

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Password, Salt, IsApproved FROM GroupMember " +
              " WHERE LoginName = @LoginName AND IsLockedOut = 'false'", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      
      SqlDataReader reader = null;
      string pwd = "";
      string salt = ""; 
      bool isApproved = false;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

        if (reader.HasRows)
        {
          reader.Read();
          pwd = reader["Password"].ToString();
          salt = reader["Salt"].ToString();
          isApproved = (bool)reader["IsApproved"];
        }
        else
        {
          return false;
        }

        reader.Close();

        if (CheckPassword(password, salt, pwd))
        {
          if (isApproved)
          {
            isValid = true;
          }
        }
        else
        {
          conn.Close();

          UpdateFailureCount(username, "password");
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ValidateUser");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }
        conn.Close();
      }

      return isValid;
    }


    //
    // UpdateFailureCount
    //   A helper method that performs the checks and updates associated with
    // password failure tracking.
    //

    private void UpdateFailureCount(string username, string failureType)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT FailedPasswordAttemptCount, " +
                                        "  FailedPasswordAttemptWindowStart, " +
                                        "  FailedPasswordAnswerAttemptCount, " +
                                        "  FailedPasswordAnswerAttemptWindowStart " + 
                                        "  FROM GroupMember " +
                                        "  WHERE LoginName = @LoginName", conn);

      cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
      
      SqlDataReader reader = null;
      DateTime windowStart = new DateTime();
      int failureCount = 0;

      try
      {
        conn.Open();

        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

        if (reader.HasRows)
        {
          reader.Read();

          if (failureType == "password")
          {
            failureCount = reader.GetInt16(0);
            if (reader[1] != DBNull.Value)
            {
                windowStart = reader.GetDateTime(1);
            }
            else
            {
                windowStart = (DateTime)SqlDateTime.MinValue;
            }
          }

          if (failureType == "passwordAnswer")
          {
            failureCount = reader.GetInt16(2);
            if (reader[3] != DBNull.Value)
            {
                windowStart = reader.GetDateTime(3);
            }
            else
            {
                windowStart = (DateTime)SqlDateTime.MinValue;
            }
          }
        }

        reader.Close();

        DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

        if (failureCount == 0 || DateTime.Now > windowEnd)
        {
          // First password failure or outside of PasswordAttemptWindow. 
          // Start a new password failure count from 1 and a new window starting now.

          if (failureType == "password")
            cmd.CommandText = "UPDATE GroupMember " +
                              "  SET FailedPasswordAttemptCount = @Count, " +
                              "      FailedPasswordAttemptWindowStart = @WindowStart " +
                              "  WHERE LoginName = @LoginName";

          if (failureType == "passwordAnswer")
              cmd.CommandText = "UPDATE GroupMember " +
                              "  SET FailedPasswordAnswerAttemptCount = @Count, " +
                              "      FailedPasswordAnswerAttemptWindowStart = @WindowStart " +
                               "  WHERE LoginName = @LoginName";

          cmd.Parameters.Clear();

          cmd.Parameters.Add("@Count", SqlDbType.Int).Value = 1;
          cmd.Parameters.Add("@WindowStart", SqlDbType.DateTime).Value = DateTime.Now;
          cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
          
          if (cmd.ExecuteNonQuery() < 0)
            throw new ProviderException("Unable to update failure count and window start.");
        }
        else
        {
          if (failureCount++ >= MaxInvalidPasswordAttempts)
          {
            // Password attempts have exceeded the failure threshold. Lock out
            // the user.

            cmd.CommandText = "UPDATE GroupMember " +
                              "  SET IsLockedOut = @IsLockedOut, LastLockedOutDate = @LastLockedOutDate " +
                              "  WHERE LoginName = @LoginName";

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
            
            if (cmd.ExecuteNonQuery() < 0)
              throw new ProviderException("Unable to lock out user.");
          }
          else
          {
            // Password attempts have not exceeded the failure threshold. Update
            // the failure counts. Leave the window the same.

            if (failureType == "password")
              cmd.CommandText = "UPDATE GroupMember " +
                                "  SET FailedPasswordAttemptCount = @FailedPasswordAttemptCount" +
                                "  WHERE LoginName = @LoginName";

            if (failureType == "passwordAnswer")
              cmd.CommandText = "UPDATE GroupMember " +
                                "  SET FailedPasswordAnswerAttemptCount = @FailedPasswordAttemptCount" +
                                "  WHERE LoginName = @LoginName";

             cmd.Parameters.Clear();

             cmd.Parameters.Add("@FailedPasswordAttemptCount", SqlDbType.Int).Value = failureCount;
             cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 255).Value = username;
             
             if (cmd.ExecuteNonQuery() < 0)
               throw new ProviderException("Unable to update failure count.");
          }
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "UpdateFailureCount");
          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }
        conn.Close();
      }       
    }


    //
    // CheckPassword
    //   Compares password values based on the MembershipPasswordFormat.
    //

    private bool CheckPassword(string password, string salt, string dbpassword)
    {
      string pass1 = password;
      string pass2 = dbpassword;

      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Encrypted:
          pass2 = UnEncodePassword(dbpassword);
          break;
        case MembershipPasswordFormat.Hashed:
          pass1 = EncodePassword(password, salt);
          break;
        default:
          break;
      }

      if (pass1 == pass2)
      {
        return true;
      }

      return false;
    }


    //
    // EncodePassword
    //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
    //

    private string EncodePassword(string password, string salt)
    {
      string encodedPassword = password;

      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          encodedPassword = 
            Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
          break;
        case MembershipPasswordFormat.Hashed:
          HMACSHA1 hash = new HMACSHA1();
          hash.Key = HexToByte(machineKey.ValidationKey);
          encodedPassword = 
            Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password + salt)));
          break;
        default:
          throw new ProviderException("Unsupported password format.");
      }

      return encodedPassword;
    }


    //
    // UnEncodePassword
    //   Decrypts or leaves the password clear based on the PasswordFormat.
    //

    private string UnEncodePassword(string encodedPassword)
    {
      string password = encodedPassword;

      switch (PasswordFormat)
      {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          password = 
            Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
          break;
        case MembershipPasswordFormat.Hashed:
          throw new ProviderException("Cannot unencode a hashed password.");
        default:
          throw new ProviderException("Unsupported password format.");
      }

      return password;
    }

    //
    // HexToByte
    //   Converts a hexadecimal string to a byte array. Used to convert encryption
    // key values from the configuration.
    //

    private byte[] HexToByte(string hexString)
    {
      byte[] returnBytes = new byte[hexString.Length / 2];
      for (int i = 0; i < returnBytes.Length; i++)
        returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
      return returnBytes;
    }


    //
    // MembershipProvider.FindUsersByName
    //

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM GroupMember " +
                "WHERE LoginName LIKE @LoginNameSearch", conn);

      cmd.Parameters.Add("@LoginNameSearch", SqlDbType.VarChar, 255).Value = usernameToMatch;
      
      MembershipUserCollection users = new MembershipUserCollection();

      SqlDataReader reader = null;

      try
      {
        conn.Open();
        totalRecords = (int)cmd.ExecuteScalar();

        if (totalRecords <= 0) { return users; }

        cmd.CommandText = "SELECT GroupMemberID, LoginName, EmailAddress, PasswordQuestion," +
          " IsLockedOut, LastLoginDate," +
          " LastPasswordChangedDate, LastLockedOutDate, IsApproved" +
          " FROM GroupMember " +
          " WHERE LoginName LIKE @LoginNameSearch" +
          " ORDER BY LoginName Asc";

        reader = cmd.ExecuteReader();

        int counter = 0;
        int startIndex = pageSize * pageIndex;
        int endIndex = startIndex + pageSize - 1;

        while (reader.Read())
        {
          if (counter >= startIndex)
          {
            MembershipUser u = GetUserFromReader(reader);
            users.Add(u);
          }

          if (counter >= endIndex) { cmd.Cancel(); }

          counter++;
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "FindUsersByName");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }

        conn.Close();
      }

      return users;
    }

    //
    // MembershipProvider.FindUsersByEmail
    //

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM GroupMember " +
                                        "WHERE EmailAddress LIKE @EmailSearch", conn);
      
      cmd.Parameters.Add("@EmailSearch", SqlDbType.VarChar, 255).Value = emailToMatch;
      MembershipUserCollection users = new MembershipUserCollection();

      SqlDataReader reader = null;
      totalRecords = 0;

      try
      {
        conn.Open();
        totalRecords = (int)cmd.ExecuteScalar();

        if (totalRecords <= 0) { return users; }

        cmd.CommandText = "SELECT GroupMemberID, Username, Email, PasswordQuestion," +
                 " IsLockedOut, LastLoginDate," +
                 " LastPasswordChangedDate, LastLockedOutDate, IsApproved " +
                 " FROM GroupMember " +
                 " WHERE Email LIKE @EmailSearch " +
                 " ORDER BY LoginName Asc";

        reader = cmd.ExecuteReader();

        int counter = 0;
        int startIndex = pageSize * pageIndex;
        int endIndex = startIndex + pageSize - 1;

        while (reader.Read())
        {
          if (counter >= startIndex)
          {
            MembershipUser u = GetUserFromReader(reader);
            users.Add(u);
          }

          if (counter >= endIndex) { cmd.Cancel(); }

          counter++;
        }
      }
      catch (SqlException e)
      {
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "FindUsersByEmail");

          throw new ProviderException(exceptionMessage);
        }
        else
        {
          throw e;
        }
      }
      finally
      {
        if (reader != null) { reader.Close(); }

        conn.Close();
      }

      return users;
    }


    //
    // WriteToEventLog
    //   A helper function that writes exception detail to the event log. Exceptions
    // are written to the event log as a security measure to avoid private database
    // details from being returned to the browser. If a method does not return a status
    // or boolean indicating the action succeeded or failed, a generic exception is also 
    // thrown by the caller.
    //

    private void WriteToEventLog(Exception e, string action)
    {
      EventLog log = new EventLog();
      log.Source = eventSource;
      log.Log = eventLog;

      string message = "An exception occurred communicating with the data source.\n\n";
      message += "Action: " + action + "\n\n";
      message += "Exception: " + e.ToString();

      log.WriteEntry(message);
    }

  }
}
