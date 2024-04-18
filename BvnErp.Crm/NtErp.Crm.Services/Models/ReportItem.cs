using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class ReportItem: Needs.Underly.Document
    {  
        public string FollowUpMethod
        {
            get
            {
                return this[nameof(FollowUpMethod)];
            }
            set
            {
                this[nameof(FollowUpMethod)] = value;
            }
        }
        public string FollowUpMethodName
        {
            get
            {
                return this[nameof(FollowUpMethodName)];
            }
            set
            {
                this[nameof(FollowUpMethodName)] = value;
            }
        }
        public DateTime FollowUpDate
        {
            get
            {
                return this[nameof(FollowUpDate)];
            }
            set
            {
                this[nameof(FollowUpDate)] = value;
            }
        }

        public DateTime NextFollowUpDate
        {
            get
            {
                return this[nameof(NextFollowUpDate)];
            }
            set
            {
                this[nameof(NextFollowUpDate)] = value;
            }
        }

        public string FollowUpAdminID
        {
            get
            {
                return this[nameof(FollowUpAdminID)];
            }
            set
            {
                this[nameof(FollowUpAdminID)] = value;
            }
        }

        public string FollowUpAdminName
        {
            get
            {
                return this[nameof(FollowUpAdminName)];
            }
            set
            {
                this[nameof(FollowUpAdminName)] = value;
            }
        }

        public string FollowUpContent
        {
            get
            {
                return this[nameof(FollowUpContent)];
            }
            set
            {
                this[nameof(FollowUpContent)] = value;
            }
        }
    }
}
