using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum JobType
    {
    
        

   /// <summary>
   /// jobType
   /// </summary>
        [Description("销售职务")]
        Sales = 1,
        /// <summary>
        /// Ptm
        /// </summary>
        [Description("Ptm职务")]
        Ptm = 2,
        /// <summary>
        /// Fac
        /// </summary>
        [Description("Fac职务")]
        Fac = 3,
        /// <summary>
        /// PM
        /// </summary>
        [Description("PM职务")]
        PM = 4,
        /// <summary>
        /// TPM
        /// </summary>
        [Description("TPM职务")]
        TPM = 5,



    }
}
