using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 费用类型
    /// </summary>
    public enum FeeType
    {
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        Product = 1,

        /// <summary>
        /// 税款
        /// </summary>
        [Description("税款")]
        Tax = 2,

        /// <summary>
        /// 代理费
        /// </summary>
        [Description("服务费")]
        AgencyFee = 3,

        /// <summary>
        /// 杂费
        /// </summary>
        [Description("杂费")]
        Incidental = 4,
    }

    /// <summary>
    /// 条款类型
    /// </summary>
    public enum ClauseType
    {
        /// <summary>
        /// 代理费率
        /// </summary>
        [Description("服务费率")]
        AgencyFee = 1,

        /// <summary>
        /// 付汇条款
        /// </summary>
        [Description("付汇条款")]
        Exchange = 2,

        /// <summary>
        /// 开票条款
        /// </summary>
        [Description("开票条款")]
        ExchangeRate = 3,

        /// <summary>
        /// 结算方式
        /// </summary>
        [Description("结算方式")]
        Finiancial = 4,

        /// <summary>
        /// 结算汇率
        /// </summary>
        [Description("结算汇率")]
        Invoice = 5,

        /// <summary>
        /// 协议期限
        /// </summary>
        [Description("协议期限")]
        AgreementTerm = 6,
    }
}
