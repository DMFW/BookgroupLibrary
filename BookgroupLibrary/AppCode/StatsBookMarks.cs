using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedCode;

namespace BookgroupLibraryCode
{
    [DataObjectAttribute]
    public class StatsBookMarks
    {
        
        private System.Configuration.ConnectionStringSettings _ConnectionSettings;
        private int _PersonID;
        private DataTable _BooksMarksTable;
        private Dictionary<string, string> _columnHeadings;

        public StatsBookMarks(int personID, string sortDirection)
        {
            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");
            _PersonID = personID;
            _BooksMarksTable = LoadBooksMarkTable(_ConnectionSettings.ConnectionString);
            UpdateFooterRowsSortFields(sortDirection);
        }

        public int PersonID
        {
            get { return _PersonID; }
        }

        public DataTable BooksMarksTable
        {
            get { return _BooksMarksTable; }
        }

        public string ColumnHeaderText(string ColumnHeaderName)
        {
            if (_columnHeadings.ContainsKey(ColumnHeaderName))
            {
                return _columnHeadings[ColumnHeaderName];
            }
            else
            {
                /* No remap so use the ColumnHeaderName name */
                return ColumnHeaderName;
            }
        }

        [DataObjectMethodAttribute(DataObjectMethodType.Select, true)]
        public System.Collections.IEnumerable SelectStatsBookMarks()
        {
            var BookmarksEnumerable = (BooksMarksTable as IListSource).GetList();
            return BookmarksEnumerable;
        }

        private DataTable LoadBooksMarkTable(string connectionString)
        {

            DataTable table = new DataTable();
            _columnHeadings = new Dictionary<string,string>();

            int visibleColumnCount;
            int displayTotalColumn = -1;
            int displayVoterCountColumn = -1;
            int displayMeanColumn = -1;
            /* Variables below hold indes values to invisible sort columns */
            int bookSortTotalIndex = -1;
            int bookSortVoterCountIndex = -1; 
            int bookSortMeanIndex = -1;
            int categoryCount = 0;

            /* Load column headings */

            #region LoadColumnHeadings

            table.Columns.Add("BookTitle");
                _columnHeadings.Add("BookTitle", "Title");

            string queryString =
               "SELECT Distinct CategoryID, CategoryDescription FROM MarkingCategory;";

            using (SqlConnection connection =
                      new SqlConnection(connectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                // Call Read before accessing data.
                while (reader.Read())
                {
                    table.Columns.Add(reader[0].ToString());
                    _columnHeadings.Add(reader[0].ToString(), reader[1].ToString());
                    categoryCount++;
                }
                // Call Close when done reading.
                reader.Close();

                 table.Columns.Add("BookTotal");
                _columnHeadings.Add("BookTotal", "Total");

                if (_PersonID == 0)
                {
                    table.Columns.Add("VoterCount");
                    _columnHeadings.Add("VoterCount", "Voters");
                }

                table.Columns.Add("BookMean");
                _columnHeadings.Add("BookMean", "Mean");

                visibleColumnCount = table.Columns.Count;

                /* Now duplicate all fields for Sorting purposes (these will be made invisible but used for sorting) */

                for (int i = 0; i < visibleColumnCount; i++)
                {
                    table.Columns.Add("Sort" + table.Columns[i].ToString());
                }

                /* Set indexes for special end of grid columns which differ depending on whether we are showing individual or average scores */

                if (_PersonID == 0)
                {
                    displayTotalColumn = visibleColumnCount - 3;
                    displayVoterCountColumn = visibleColumnCount - 2;
                    displayMeanColumn = visibleColumnCount - 1;

                    bookSortTotalIndex = table.Columns.Count - 3;
                    bookSortVoterCountIndex = table.Columns.Count - 2;
                    bookSortMeanIndex = table.Columns.Count - 1;
                }
                else
                {
                    /* There is no voter count on this route */
                    displayTotalColumn = visibleColumnCount - 2;
                    displayMeanColumn = visibleColumnCount - 1;
                    
                    bookSortTotalIndex = table.Columns.Count - 2;
                    bookSortMeanIndex = table.Columns.Count - 1;
                }

            }
            #endregion

            /* Load book marks */

            #region LoadTableBody

            string displayMarkField;
            string sourceTotalField;

            if (_PersonID == 0)
            {
                /* Aggregate of all group members requested */
                queryString =
                    "SELECT * from AggregateBookMarks Order by BookID";
                displayMarkField = "MeanMark";
                sourceTotalField = "TotalMark"; // Find the total for the category and total that to derive a grand total;
            }
            else
            {
                queryString =
                    "SELECT * FROM BookMarks inner join Book on BookMarks.BookID = Book.BookID where GroupMemberID = " + _PersonID + " Order By BookMarks.BookID;";
                displayMarkField = "Mark";
                sourceTotalField = "Mark"; // Total the marks just as you see them....
            }

            string currentTitle = "";
            int titleOrdinal;
            int displayMarkOrdinal;
            int markCountOrdinal = -1;
            int sourceTotalOrdinal = -1;
            int categoryOrdinal;
            double bookTotal = 0;
            int maxMarkCount = 0;
            double bookMean = 0;
            int votesCast = 0;
            Boolean jaggedMatrixAverage = false;
            int personPopulatedColumnCount = 0; /* How many categories the person voted in for this book (almost always 7 but not always) */
           
            using (SqlConnection connection =
                       new SqlConnection(connectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                titleOrdinal = reader.GetOrdinal("Title");
                displayMarkOrdinal = reader.GetOrdinal(displayMarkField);
                sourceTotalOrdinal = reader.GetOrdinal(sourceTotalField);
                categoryOrdinal = reader.GetOrdinal("CategoryID");

                if (_PersonID == 0)
                {
                    markCountOrdinal = reader.GetOrdinal("MarkCount");
                }
                
                DataRow row;
                row = table.NewRow(); /* Done purely to allow use of an 'uninitialised' (but not on any real routes) local variable */             
                
                // Call Read before accessing data.
                while (reader.Read())
                {

                    if (reader[titleOrdinal].ToString() != currentTitle)
                    /* This will always be true on 1st time thru so row will always be recreated */
                    {
                        if (displayTotalColumn > 0)
                        {
                            row[displayTotalColumn] = bookTotal; /* Populate last total into penultimate visible column */
                            row[bookSortTotalIndex] = bookTotal.ToString("000");
                        }
                         if (displayVoterCountColumn > 0)
                        {
                            string maxMarkDisplay;
                            if (jaggedMatrixAverage) { maxMarkDisplay = "~" + maxMarkCount.ToString(); } else {maxMarkDisplay = maxMarkCount.ToString(); }
                            row[displayVoterCountColumn] = maxMarkDisplay; /* The most people who voted (should usually be the same in all categories but allow for missing category votes) */
                            if (jaggedMatrixAverage) { row[bookSortVoterCountIndex] = maxMarkCount.ToString("00") + "~"; } else { row[bookSortVoterCountIndex] = maxMarkCount.ToString("00"); }
                        }       
                        if (displayMeanColumn > 0)
                        {
                            if (_PersonID != 0)
                            {
                                bookMean = (double)bookTotal / (double)(personPopulatedColumnCount);
                            }
                            else
                            {
                                bookMean = (double)bookTotal / (double)(votesCast);
                            }

                            row[displayMeanColumn] = bookMean.ToString("0.00"); /* Populate last total into last visible column */
                            row[bookSortMeanIndex] = bookMean.ToString("00.00");
                        }

                        bookTotal = 0;
                        maxMarkCount = 0;
                        votesCast = 0;
                        personPopulatedColumnCount = 0;
                        jaggedMatrixAverage = false;
                        row = table.NewRow();
                        table.Rows.Add(row);
                        currentTitle = reader[titleOrdinal].ToString();
                        row[0] = currentTitle;
                        row[visibleColumnCount] = currentTitle;
                    }

                    /* Marks for a category are offset by +1 column because they are a 1 based index off a zero based grid but there are 2 leading columns (one hidden)
                     * so category index 1 has to start at grid index 2 */

                    if (_PersonID == 0)
                    {
                        row[int.Parse(reader[categoryOrdinal].ToString())] = double.Parse(reader[displayMarkOrdinal].ToString()).ToString("#0.00");
                        votesCast += int.Parse(reader[markCountOrdinal].ToString());
                        if (int.Parse(reader[markCountOrdinal].ToString()) != maxMarkCount)
                        {
                            if (maxMarkCount != 0) { jaggedMatrixAverage = true; } // Data not clean. Averages will still be correct but not calculable by inspection on the grid so set warning state
                            if (int.Parse(reader[markCountOrdinal].ToString()) > maxMarkCount)
                            {
                                maxMarkCount = int.Parse(reader[markCountOrdinal].ToString());
                            }
                        }
                    }
                    else
                    {
                        personPopulatedColumnCount++;
                        row[int.Parse(reader[categoryOrdinal].ToString())] = reader[displayMarkOrdinal].ToString();
                    }
                    row[int.Parse(reader[categoryOrdinal].ToString()) + visibleColumnCount] = double.Parse(reader[displayMarkOrdinal].ToString()).ToString("00.00");

                    bookTotal = bookTotal + double.Parse(reader[sourceTotalOrdinal].ToString());
                }
                // Call Close when done reading.
                reader.Close();

                /* Populate the special end grid columns for the last row */

                if (displayTotalColumn > 0)
                {
                    row[displayTotalColumn] = bookTotal; /* Populate last total into penultimate visible column */
                    row[bookSortTotalIndex] = bookTotal.ToString("000");
                }
                if (displayVoterCountColumn > 0)
                {
                    string maxMarkDisplay;
                    if (jaggedMatrixAverage) { maxMarkDisplay = "~" + maxMarkCount.ToString(); } else { maxMarkDisplay = maxMarkCount.ToString(); }
                    row[displayVoterCountColumn] = maxMarkDisplay; /* The most people who voted (should usually be the same in all categories but allow for missing category votes) */
                    if (jaggedMatrixAverage) { row[bookSortVoterCountIndex] = maxMarkCount.ToString("00") + "~"; } else { row[bookSortVoterCountIndex] = maxMarkCount.ToString("00"); }
                }
                if (displayMeanColumn > 0)
                {
                    if (_PersonID != 0)
                    {
                        bookMean = (double)bookTotal / (double)(personPopulatedColumnCount);
                    }
                    else
                    {
                        bookMean = (double)bookTotal / (double)(votesCast);
                    }

                    row[displayMeanColumn] = bookMean.ToString("0.00"); /* Populate last total into last visible column */
                    row[bookSortMeanIndex] = bookMean.ToString("00.00");
                }
            }

            #endregion

            /* Now add total rows if there is any point in having them */

            if (table.Rows.Count > 1)
            {

                #region LoadTableTotalRows

                DataRow totalRow = table.NewRow();
                double workingTotal;
                double workingColumn;
                bool validColumnValue;
                int[] populatedRowsPerColumn;
                double trueTotal = 0; // This is just the aggregate of every mark 
                double trueMarkCount = 0; // This is the correct number of marks given

                populatedRowsPerColumn = new int[visibleColumnCount];

                totalRow[0] = "Total";

                for (int i = 1; i < visibleColumnCount; i++)
                {
                    workingTotal = 0.00;
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        validColumnValue = double.TryParse(table.Rows[j][i].ToString(),out workingColumn);
                        if (validColumnValue)
                        {
                            populatedRowsPerColumn[i]++;
                            workingTotal += workingColumn;
                        }
                    }
                    totalRow[i] = workingTotal.ToString(columnFormat(table.Columns[i].ColumnName.ToString()));
                }

                trueTotal = double.Parse(totalRow[displayTotalColumn].ToString());
                table.Rows.Add(totalRow);

                DataRow meanRow = table.NewRow();
                double workingMean;

                meanRow[0] = "Mean";

                for (int i = 1; i < visibleColumnCount; i++)
                {
                    workingMean = double.Parse(table.Rows[table.Rows.Count - 1][i].ToString()) / (double)populatedRowsPerColumn[i];
                    meanRow[i] = workingMean.ToString("##0.00");
                    if (i < (displayTotalColumn))
                    {
                        trueMarkCount += populatedRowsPerColumn[i];
                    }
                }

                // Replace the value calculated above with one correct, even when some marks are missing
                meanRow[displayMeanColumn] = (trueTotal / (double)trueMarkCount).ToString("##0.00");
                table.Rows.Add(meanRow);

                #endregion
            }

            return table;
        }

        private void UpdateFooterRowsSortFields(string sortDirection)
        {
            string sortPenultimate;
            string sortUltimate;
            string sortKey;

            if (sortDirection == "ASC")
            {
                sortPenultimate = "Z";
                sortUltimate = "ZZ";
            }
            else
            {
                sortPenultimate = "##";
                sortUltimate = "#";
            }

            foreach (DataRow Row in _BooksMarksTable.Rows)
            {
                sortKey = "";
                if (Row["BookTitle"].ToString() == "Mean") sortKey = sortUltimate;
                if (Row["BookTitle"].ToString() == "Total") sortKey = sortPenultimate;
                if (sortKey != "")
                {
                    foreach (DataColumn Col in _BooksMarksTable.Columns)
                    {
                        if (Col.ColumnName.ToString().Length >= 4)
                        {
                            if (string.Compare(Col.ColumnName.ToString().Substring(0, 4), "Sort") == 0)
                            {
                                Row[Col] = sortKey;
                            }
                        }
                    }
                }
            }
        }

        private string columnFormat(string columnName)
        {
            string columnFormatString;

            switch (columnName)
            {
                case "MemberMean":
                    columnFormatString = "##0.00";
                    break;
                default:
                    columnFormatString = "##0";
                    break;
            }

            if (_PersonID == 0)
            {
                // Override the column format for averages to ensure everything except vote count
                // and total score is diplayed to two decimal places.

                switch (columnName)
                {
                    case "BookTotal":
                    case "VoterCount":
                        columnFormatString = "###0";
                        break;
                    default:
                        columnFormatString = "##0.00";
                        break;
                }
                
            }

            return columnFormatString;
        }
    }
}
