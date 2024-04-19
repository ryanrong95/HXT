using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 价格条款
    /// </summary>
    public enum PriceClause
    {
        /// <summary>
        /// 到岸价格[成本加保险费、运费]
        /// </summary>
        [Description("到港价")]
        CIF = 1,
        /// <summary>
        /// 离岸价格
        /// </summary>
        [Description("离港价")]
        FOB = 2
    }
}
