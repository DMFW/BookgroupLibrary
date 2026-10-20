using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookgroupLibrary
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Web;

    // Variables in here are pan application i.e. they span sessions
    // so we can check if another user is locking a book for edit.

    public static class AppState
    {
        static Dictionary<int,BookLock> _bookEditLocks = new Dictionary<int,BookLock>();

        public static Dictionary<int, BookLock> BookEditLocks
        {
            get
            {
                return _bookEditLocks;
            }
        }
    }
}