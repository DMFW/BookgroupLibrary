using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookgroupLibrary
{
    public class BookLock
    {
        private int _bookID;
        private int _groupMemberID;
        private string _groupMember;
        private DateTime _dteLockAcquired;

        public BookLock(int bookID, int groupMemberID, string groupMember)
        {
            _bookID = bookID;
            _groupMemberID = groupMemberID;
            _groupMember = groupMember;
            _dteLockAcquired = DateTime.Now;
        }

        public int BookID
        {
            get { return _bookID; }
            set { _bookID = value; }
        }

        public int GroupMemberID
        {
            get { return _groupMemberID; }
            set { _groupMemberID = value; }
        }

        public string GroupMember
        {
            get { return _groupMember; }
            set { _groupMember = value; }
        }

        public DateTime LockAcquired
        {
            get { return _dteLockAcquired; }
            set { _dteLockAcquired = value; }
        }

        public int MinutesToExpiry
        {
             get 
             {
                 TimeSpan lockDuration = DateTime.Now - _dteLockAcquired;
                 return 15 - lockDuration.Minutes; 
             }
        }

        public bool LockExpired()
        {
            TimeSpan lockDuration = DateTime.Now - _dteLockAcquired;

            if (lockDuration.Minutes > 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}