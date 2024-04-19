using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum InvoiceType
    {

       

        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通发票")]
        Plain = 1,
        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税发票")]
        VAT = 2,
    }
}
