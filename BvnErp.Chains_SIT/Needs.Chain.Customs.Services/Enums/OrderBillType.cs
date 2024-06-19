using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum OrderBillType
    {
        /// <summary>
        /// 代理费小于最小代理费，按照最小代理费收，否则是多少就收多少
        /// </summary>
        [Description("正常收取")]
        Normal = 1,

        /// <summary>
        /// 按最小代理费收
        /// </summary>
        [Description("最低服务费")]
        MinAgencyFee = 2,

        /// <summary>
        /// 按固定金额收
        /// </summary>
        [Description("指定服务费")]
        Pointed = 3
    }
}
