using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace BookgroupLibraryCode
{
    [Serializable]
    public class PersonBookMarksViewState
    {
        private DataTable _PersonBookMarksTable;
        private Dictionary<string, string> _dctColumnHeadings;
        private Dictionary<int, int> _dctCategoryColumnIndex;
        private Dictionary<string, int> _dctCategoryIDIndex;

        public PersonBookMarksViewState()
        {
            // Encapsulates everything the PersonBookMark class may want to save and recover in a view state.
            // Note that we really only do this for editing; viewing doesn't bother to save this class
            // in the view state, but it still uses the same structure, just refreshing it fully on every page
            // load instead of preserving it and modifying it in edit functions.
        }

        public DataTable BookMarks
        {
            get { return _PersonBookMarksTable; }
            set { _PersonBookMarksTable = value; }
        }

        public Dictionary<string, string> ColumnHeadings
        {
            // Key = "Category Field Name"; Value = "Category Field Description" used in headings

            get { return _dctColumnHeadings; }
            set { _dctColumnHeadings = value; }
        }

        public Dictionary<int, int> CategoryColumnIndex
        {
            // Key = "Category ID"; Value = Column index we are mapping to in this specific view display

            get { return _dctCategoryColumnIndex; }
            set { _dctCategoryColumnIndex = value; }
        }

        public Dictionary<string, int> CategoryIDIndex
        {
            // Key = "Category Field Name"; Value = "Category ID"
            // This is a simple memory remapped copy of the category table and technically it could be 
            // held in a session variable (rather than view state) for better performance or reread on
            // demand but as it is small and fits conceptuallly with the other dictionaries in this
            // class we will treat it the same.

            get { return _dctCategoryIDIndex; }
            set { _dctCategoryIDIndex = value; }
        }

    }
}