using System;
using LogApi.App_Code;
using System.Security.Principal;
using static LogApi.App_Code.Utils;

namespace LogApi.Models
{
    /// <summary>
    /// User data model
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Defines the basic functionality of a principal object
        /// </summary>
        public IPrincipal Principal;

        /// <summary>
        /// Authentication token
        /// </summary>
        public string Token;

        /// <summary>
        /// Date and time to which the user is blocked. If the user is not blocked, this value is null
        /// </summary>
        private DateTime? mBlockedTo = null;
        public DateTime? BlockedTo
        {
            get
            {
                if(mBlockedTo!= null)
                {
                    if(DateTime.Now > mBlockedTo)
                    {
                        mBlockedTo = null;
                    }
                }
                return mBlockedTo;
            }
            set
            {
                mBlockedTo = value;
            }
        }

        /// <summary>
        /// Requests ticks fixed sized queue
        /// </summary>
        public FixedSizedQueue<DateTime> RequestTicks = new FixedSizedQueue<DateTime>(RequestsPerMinuteCount);

        /// <summary>
        /// User data object
        /// </summary>
        public UserData()
        {
            RequestTicks.Enqueue(DateTime.Now);
        }

    }
}