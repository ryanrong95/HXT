using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 是否收取入仓费
    /// </summary>
    public enum ChargeWHType
    {
        /// <summary>
        /// 收取
        /// </summary>
        [Description("收取")]
        Charge = 1,

        /// <summary>
        /// 不收取
        /// </summary>
        [Description("不收取")]
        NotCharge = 2,
    }


    /// <summary>
    /// 收取方式
    /// </summary>
    public enum ChargeType
    {
        /// <summary>
        /// 收取
        /// </summary>
        [Description("国内")]
        Domestic = 1,

        /// <summary>
        /// 不收取
        /// </summary>
        [Description("香港")]
        HK = 2,
    }
}
