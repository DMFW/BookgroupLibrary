using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using SharedCode;

namespace BookgroupLibraryCode
{
    [DataObjectAttribute]
    public class PersonBookMarks
    {
        private System.Configuration.ConnectionStringSettings _ConnectionSettings;
        private int _BookID;
        private bool _EditMode;
        private PersonBookMarksViewState _ViewState;

        #region Constructors

        public PersonBookMarks(int bookID, string sortDirection)
        {
            _EditMode = false; // This contructor is used for display mode only (with sorting)

            _BookID = bookID;
            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");
            _ViewState = new PersonBookMarksViewState(); // Not actually saved to the true view state when used with edit mode false

            LoadPersonMarkTable(_ConnectionSettings.ConnectionString);
            UpdateFooterRowsSortFields(sortDirection); 
        }

        public PersonBookMarks(int bookID, PersonBookMarksViewState editViewState)
        {
            _EditMode = true; // This constructor is used for editing (without sorting)

            _BookID = bookID;
            _ConnectionSettings = Config.GetDBConnectionSettings("SouthendBookgroupConnectionString");

            if (editViewState != null)
            {
                _ViewState = editViewState;
            }
            else
            {
                _ViewState = new PersonBookMarksViewState();
                LoadPersonMarkTable(_ConnectionSettings.ConnectionString);
            }
        }

        #endregion

        public int BookID
        {
            get { return _BookID; }
        }

        public PersonBookMarksViewState CurrentViewState
        {
            get { return _ViewState; }
        }

        public string ColumnHeaderText(string ColumnHeaderName)
        {
            if (_ViewState.ColumnHeadings.ContainsKey(ColumnHeaderName))
            {
                return _ViewState.ColumnHeadings[ColumnHeaderName];
            }
            else
            {
                /* No remap so use the ColumnHeaderName name */
                return ColumnHeaderName;
            }
        }

        [DataObjectMethodAttribute(DataObjectMethodType.Select, true)]
        public System.Collections.IEnumerable SelectPersonBookMarks()
        {
            var PersonBookmarksEnumerable = (_ViewState.BookMarks as IListSource).GetList();
            return PersonBookmarksEnumerable;
        }

        [DataObjectMethodAttribute(DataObjectMethodType.Insert, true)]
        public void InsertPersonBookMarks(int GroupMemberID)
        {
            // This mechanism handles logical updates on the memory structure which can be rolled back
            // or commited by explicit external request.

            DataRow row = _ViewState.BookMarks.NewRow();
            row["GroupMemberID"] = GroupMemberID;
            row["GroupMember"] = GroupMemberDisplayName(GroupMemberID);

            _ViewState.BookMarks.Rows.Add(row);
        }

        [DataObjectMethodAttribute(DataObjectMethodType.Update, true)]
        public void UpdatePersonBookMarks(int RowIndex, int GroupMemberID, float? Plot, float? Characters, float? Scenery, float? Writing, float? Philosophy, float? Unputdown, float? Overall)
        {
            // This mechanism handles logical updates on the memory structure which can be rolled back
            // or commited by explicit external request.

            DataRow EditRow = _ViewState.BookMarks.Rows[RowIndex];

            EditRow["GroupMemberID"] = GroupMemberID;
            EditRow["GroupMember"] = GroupMemberDisplayName(GroupMemberID);

            EditRow["Plot"] = Plot;
            EditRow["Characters"] = Characters;
            EditRow["Scenery"] = Scenery;
            EditRow["Writing"] = Writing;
            EditRow["Philosophy"] = Philosophy;
            EditRow["Unputdown"] = Unputdown;
            EditRow["Overall"] = Overall;

        }

        [DataObjectMethodAttribute(DataObjectMethodType.Delete, true)]
        public void DeletePersonBookMarks(int RowIndex)
        {
            // This mechanism handles logical updates on the memory structure which can be rolled back
            // or commited by explicit external request.

            _ViewState.BookMarks.Rows.RemoveAt(RowIndex);
        }

        private void LoadPersonMarkTable(string connectionString)
        {

            /* Create the object data table used for selecting and editing */

            DataTable table = new DataTable();

            _ViewState.ColumnHeadings = new Dictionary<string, string>();
            _ViewState.CategoryColumnIndex = new Dictionary<int, int>();
            _ViewState.CategoryIDIndex = new Dictionary<string, int>();

            int visibleColumnCount;

            /* Load column headings */

            #region LoadColumnHeadings

            table.Columns.Add("GroupMember"); 
            _ViewState.ColumnHeadings.Add("GroupMember", "Group Member");

            string queryString = "SELECT CategoryID, DataTableFieldName, CategoryDescription FROM MarkingCategory";

            using (SqlConnection connection =
                      new SqlConnection(connectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    table.Columns.Add(reader[1].ToString().Trim());
                    _ViewState.CategoryColumnIndex.Add(Int32.Parse(reader[0].ToString()), table.Columns.Count - 1);
                    _ViewState.ColumnHeadings.Add(reader[1].ToString().Trim(), reader[2].ToString().Trim());
                    _ViewState.CategoryIDIndex.Add(reader[1].ToString().Trim(), Int32.Parse(reader[0].ToString()));
                }
                reader.Close();

                if (_EditMode == false)
                {
                    table.Columns.Add("MemberTotal");
                    _ViewState.ColumnHeadings.Add("MemberTotal", "Total");

                    table.Columns.Add("MemberMean");
                    _ViewState.ColumnHeadings.Add("MemberMean", "Mean");
                }

                visibleColumnCount = table.Columns.Count;

                 /* This is the end of the displayed columns, now add conrol columns */

                if (_EditMode == true)
                {
                    // When editing we have an invisible ID column used to bind to the drop down list on row edit
                    table.Columns.Add("GroupMemberID");
                }
                else
                {
                    // When not editing we have a mirrored invisible column for each sortable column.
                    for (int i = 0; i < visibleColumnCount; i++)
                    {
                        table.Columns.Add("Sort" + table.Columns[i].ToString());
                    }
                }

            }
            #endregion

            /* Load person marks (note despite ordering by suname and forename within book inside view, only ordering here is applied) */

            #region LoadTableBody

            queryString =
                "SELECT * FROM BookMarks where BookID = " + BookID + " Order By LastName, FirstName;";

            string currentPerson = "";
            int groupMemberIDOrdinal;
            int lastNameOrdinal;
            int firstNameOrdinal;
            int personOrdinal;
            int markOrdinal;
            int categoryOrdinal;
            double memberTotal = 0;
            double memberMean = 0;
            int memberSortTotalIndex; /* Not an ordinal because not on the result set */
            int memberSortMeanIndex; /* Not an ordinal because not on the result set */
            int personPopulatedColumnCount = 0; /* How many categories the person voted in for this book (almost always 7 but not always) */

            using (SqlConnection connection =
                       new SqlConnection(connectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                groupMemberIDOrdinal = reader.GetOrdinal("GroupMemberID");
                lastNameOrdinal = reader.GetOrdinal("LastName");
                firstNameOrdinal = reader.GetOrdinal("FirstName");
                personOrdinal = reader.GetOrdinal("GroupMember"); 
                markOrdinal = reader.GetOrdinal("Mark");
                categoryOrdinal = reader.GetOrdinal("CategoryID");

                if (_EditMode == true)
                {
                    memberSortTotalIndex = 0;
                    memberSortMeanIndex = 0;
                }
                else
                {
                    memberSortTotalIndex = table.Columns.Count - 2;
                    memberSortMeanIndex = table.Columns.Count - 1;
                }
                
                DataRow row;
                row = table.NewRow(); /* Done purely to allow use of an 'uninitialised' (but not on any real routes) local variable */             
                
                // Call Read before accessing data.
                while (reader.Read())
                {
                    
                    if (reader[personOrdinal].ToString() != currentPerson)
                    /* This will always be true on 1st time thru so row will always be recreated */
                    {
                        if (_EditMode == false)
                        {
                            row[visibleColumnCount - 2] = memberTotal; /* Populate last total into penultimate visible column */
                            row[memberSortTotalIndex] = memberTotal.ToString("000.0");
                            memberMean = (double)memberTotal / (double)(personPopulatedColumnCount);
                            row[visibleColumnCount - 1] = memberMean.ToString("#0.00"); /* Populate last total into last visible column */
                            row[memberSortMeanIndex] = memberMean.ToString("000.00");
                            memberTotal = 0;
                            personPopulatedColumnCount = 0;
                        }
                        row = table.NewRow();
                        table.Rows.Add(row);
                        currentPerson = reader[personOrdinal].ToString();
                        row[0] = currentPerson;

                        /* Populate the first invisible column with either the group member ID key (editing) or the person sort data (display) */
                        /* Remember that the row is zero indexed so visibleColumnCount = visible columns + 1 = first invisible column */

                        if (_EditMode == true)
                        {
                            row[visibleColumnCount] = reader[groupMemberIDOrdinal].ToString();
                        }
                        else
                        { 
                            row[visibleColumnCount] = reader[lastNameOrdinal].ToString() + " " + reader[firstNameOrdinal].ToString(); 
                        }
                    }

                    int catgeoryColumn = _ViewState.CategoryColumnIndex[int.Parse(reader[categoryOrdinal].ToString())];
                    row[catgeoryColumn] = reader[markOrdinal].ToString();

                    if (_EditMode == false)
                    {
                        // Populate an invisible column for sorting but we won't have this column if we are editing
                        row[int.Parse(reader[categoryOrdinal].ToString()) + visibleColumnCount] = double.Parse(reader[markOrdinal].ToString()).ToString("00.0");
                        memberTotal = memberTotal + double.Parse(reader[markOrdinal].ToString());
                    }
                    
                    personPopulatedColumnCount++;

                }
                // Call Close when done reading.
                reader.Close();

                if (_EditMode == false)
                {
                    row[visibleColumnCount - 2] = memberTotal; /* Populate last total from last row into penultimate visible column */
                    row[memberSortTotalIndex] = memberTotal.ToString("000.0");
                    memberMean = (double)memberTotal / (double)(personPopulatedColumnCount);
                    row[visibleColumnCount - 1] = memberMean.ToString("#0.00"); /* Populate last total into last visible column */
                    row[memberSortMeanIndex] = memberMean.ToString("000.00");
                }
            }

            #endregion

            /* Now add total rows if there is any point in having them */

            if (table.Rows.Count > 1)
            {
                if (_EditMode == false)
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
                        populatedRowsPerColumn[i] = 0;

                        for (int j = 0; j < table.Rows.Count; j++)
                        {
                            validColumnValue = double.TryParse(table.Rows[j][i].ToString(), out workingColumn);
                            if (validColumnValue)
                            {
                                populatedRowsPerColumn[i]++;
                                workingTotal += workingColumn;
                            }
                        }
                        totalRow[i] = workingTotal.ToString(columnFormat(table.Columns[i].ColumnName.ToString()));
                    }

                    trueTotal = double.Parse(totalRow[visibleColumnCount - 2].ToString());
                    table.Rows.Add(totalRow);

                    DataRow meanRow = table.NewRow();
                    double workingMean;

                    meanRow[0] = "Mean";

                    for (int i = 1; i < visibleColumnCount; i++)
                    {
                        workingMean = double.Parse(table.Rows[table.Rows.Count - 1][i].ToString()) / (double)populatedRowsPerColumn[i];
                        meanRow[i] = workingMean.ToString("##0.00");

                        if (i < (visibleColumnCount - 2))
                        {
                            trueMarkCount += populatedRowsPerColumn[i];
                        }
                    }

                    // Replace the value calculated above with one correct, even when some marks are missing
                    meanRow[visibleColumnCount - 1] = (trueTotal / (double)trueMarkCount).ToString("##0.00");
                    table.Rows.Add(meanRow);

                    #endregion
                }
            }

            // Note : - The final line below is not saving the result in the view state because
            // that is not available in this class. It is meterly posting it in an exposed 
            // variable which the page may choose to post into the view state if it wants to. 
            // Editing will do this but viewing does not need to.

            _ViewState.BookMarks = table;
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

            foreach (DataRow Row in _ViewState.BookMarks.Rows)
            {
                sortKey = "";
                if (Row["GroupMember"].ToString() == "Mean") sortKey = sortUltimate;
                if (Row["GroupMember"].ToString() == "Total") sortKey = sortPenultimate;
                if (sortKey != "")
                {
                    foreach (DataColumn Col in _ViewState.BookMarks.Columns)
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
                    columnFormatString = "##0.##";
                    break;
            }

            return columnFormatString;
        }

        private string GroupMemberDisplayName(int GroupMemberID)
        {
            string queryString = "SELECT FirstName, LastName FROM GroupMember where GroupMemberID = " + GroupMemberID;
            string GroupMemberDisplayName;

            using (SqlConnection connection = new SqlConnection(_ConnectionSettings.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                GroupMemberDisplayName = reader[0].ToString() + ' ' + reader[1].ToString();
                connection.Close();
            }
            return GroupMemberDisplayName;
        }

        public bool TrueSave(out string statusMessage)
        {

            // Physically save to the database all our workings in the data table.
            // Assume it is going to work and set the two variables below differently if it doesn't

            bool saveResult = true;
            statusMessage = "";

            SqlConnection con = new SqlConnection(_ConnectionSettings.ConnectionString);

            SqlTransaction scoresTrans;
            
            con.Open();
            scoresTrans = con.BeginTransaction();

            try
            {

                // Delete all existing scrores

                string deleteQueryString = "DELETE FROM BookMark WHERE BookID = " + _BookID;
                SqlCommand cmdDel = new SqlCommand(deleteQueryString, con, scoresTrans);
                cmdDel.ExecuteNonQuery();

                // Now add the new ones

                string insertQueryString = "INSERT INTO BookMark (BookID, MemberID, CategoryID, Mark)" +
                                              " VALUES (@BookID, @MemberID, @CategoryID, @Mark)";
                SqlCommand cmdIns = new SqlCommand(insertQueryString, con, scoresTrans);

                SqlParameter pBookID = new SqlParameter("@BookID", SqlDbType.Int);
                pBookID.Value = _BookID;
                cmdIns.Parameters.Add(pBookID);

                SqlParameter pMemberID = new SqlParameter("@MemberID", SqlDbType.Int);
                cmdIns.Parameters.Add(pMemberID);
                SqlParameter pCategoryID = new SqlParameter("@CategoryID", SqlDbType.Int);
                cmdIns.Parameters.Add(pCategoryID);
                SqlParameter pMark = new SqlParameter("@Mark", SqlDbType.Float);
                cmdIns.Parameters.Add(pMark);

                foreach (DataRow currentScoreRow in _ViewState.BookMarks.Rows)
                {
                    pMemberID.Value = Int32.Parse(currentScoreRow["GroupMemberID"].ToString());

                    foreach (KeyValuePair<string, int> potentialScoreRecord in _ViewState.CategoryIDIndex)
                    {
                        if (currentScoreRow[potentialScoreRecord.Key] != null)
                        {
                            pCategoryID.Value = Int32.Parse(potentialScoreRecord.Value.ToString());
                            pMark.Value = Double.Parse(currentScoreRow[potentialScoreRecord.Key].ToString());
                            cmdIns.ExecuteNonQuery();
                        }
                    }
                }
                scoresTrans.Commit();
            }
            catch (Exception e)
            {
                scoresTrans.Rollback();
                statusMessage = "Database save failed : " + e.Message;
                saveResult = false;
            }
            finally
            {
                con.Close();
            }

            return saveResult;
        }

    }
}