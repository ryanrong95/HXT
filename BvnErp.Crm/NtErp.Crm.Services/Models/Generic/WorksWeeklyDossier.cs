using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Generic
{
    public class WorksWeeklyDossier
    {
        public WorksWeekly WorksWeekly
        {
            get; set;
        }

        public File[] Files { get; set; }

        public Reply[] Replies { get; set; }

    }
}
