using SharedCode;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookgroupLibrary
{
    public partial class StatisticsForm : System.Web.UI.Page
    {
			private System.Configuration.ConnectionStringSettings _ConnectionSettings;
			protected void Page_Load(object sender, EventArgs e)
			{
				_ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");
				if (!IsPostBack)
				{
					LoadSimpleStatistics();
				}
			}
			private void LoadSimpleStatistics()
			{
					string queryString = "SELECT COUNT(*) AS TotalBooks, " +
						"MIN(YearOfPublication) AS OldestPublication, MAX(YearOfPublication) AS NewestPublication FROM Book;";
					int totalAuthorCount = 0;

					using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
					{
							SqlCommand command = new SqlCommand(queryString, connection);
							connection.Open();

							SqlDataReader reader = command.ExecuteReader();

							Debug.Assert(reader.HasRows);

							reader.Read();

							lblTotalBooks.Text = reader["TotalBooks"].ToString();
							lblOldestPublication.Text = Convert.ToDateTime(reader["OldestPublication"]).ToString("yyyy");
							lblNewestPublication.Text = Convert.ToDateTime(reader["NewestPublication"]).ToString("yyyy");

							reader.Close();
					}

					queryString = "SELECT Sex, COUNT(*) AS AuthorCount FROM Author GROUP BY Sex;";

					using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
					{
						SqlCommand command = new SqlCommand(queryString, connection);
						connection.Open();

						SqlDataReader reader = command.ExecuteReader();

						Debug.Assert(reader.HasRows);

						while(reader.Read())
						{
							if (reader["Sex"].ToString() == "False")
							{
								lblMaleAuthors.Text = reader["AuthorCount"].ToString();
								totalAuthorCount += Int32.Parse(reader["AuthorCount"].ToString());
							}
							else if (reader["Sex"].ToString() == "True")
							{
								lblFemaleAuthors.Text = reader["AuthorCount"].ToString();
								totalAuthorCount += Int32.Parse(reader["AuthorCount"].ToString());	
							}
						};

						reader.Close();
						lblTotalAuthors.Text = totalAuthorCount.ToString();

			}

		}
	}	
}
