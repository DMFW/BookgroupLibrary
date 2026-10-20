using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using SharedCode;

namespace BookgroupLibraryCode
{
    [DataObjectAttribute]
    public class BookAuthors
    {

        private DataTable _BookAuthorsTable; // Local uncommitted table
        private System.Configuration.ConnectionStringSettings _ConnectionSettings;

        public BookAuthors()
        {

            _BookAuthorsTable = new DataTable();
            _BookAuthorsTable.Columns.Add("ID", typeof(int));
            _BookAuthorsTable.Columns.Add("FirstName", typeof(string));
            _BookAuthorsTable.Columns.Add("LastName", typeof(string));

            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");
           
        }

        [DataObjectMethodAttribute(DataObjectMethodType.Select, true)]
        public System.Collections.IEnumerable SelectBookAuthors()
        {
            var BookAuthorsEnumerable = (BookAuthorsTable as IListSource).GetList();
            return BookAuthorsEnumerable;
        }

        public DataTable BookAuthorsTable
        {
            get { return _BookAuthorsTable; }
        }

        public void AddAuthor(int AuthorID)
        {

            string FirstName;
            string LastName;

            string queryString = "SELECT FirstName, LastName FROM Authors Where AuthorID = @AuthorID;";

            using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(new SqlParameter("@AuthorID", AuthorID));

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                Debug.Assert(reader.HasRows);

                reader.Read();
                FirstName = (string)reader["FirstName"];
                LastName = (string)reader["LastName"];
                reader.Close();
            }

            _BookAuthorsTable.Rows.Add(AuthorID, FirstName, LastName);
        }
    }
} 