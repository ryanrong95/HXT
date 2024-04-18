using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 协议变更项类型
    /// </summary>
    public enum AgreementChangeType
    {
        /// <summary>
        /// 协议开始日期
        /// </summary>
        [Description("协议开始日期")]
        StartDate = 1,

        /// <summary>
        /// 协议结束日期
        /// </summary>
        [Description("协议结束日期")]
        EndDate = 2,

        /// <summary>
        /// 代理费率
        /// </summary>
        [Description("代理费率")]
        AgencyRate = 3,

        /// <summary>
        /// 最低代理费
        /// </summary>
        [Description("最低代理费")]
        MinAgencyFee = 4,

        /// <summary>
        /// 是否预付款
        /// </summary>
        [Description("是否预付款")]
        IsPrePayExchange = 5,

        /// <summary>
        /// 开票类型
        /// </summary>
        [Description("开票类型")]
        InvoiceType = 6,

        /// <summary>
        /// 开票税点
        /// </summary>
        [Description("开票税点")]
        InvoiceTaxRate = 7,

        /// <summary>
        /// 是否十点汇率
        /// </summary>
        [Description("换汇汇率类型")]
        IsTenType = 8,

        /// <summary>
        /// 税款结算方式
        /// </summary>
        [Description("税款结算方式")]
        TaxPeriodType = 101,

        /// <summary>
        /// 税款约定期限
        /// </summary>
        [Description("税款约定期限")]
        TaxDaysLimit = 102,

        /// <summary>
        /// 税款月结日期
        /// </summary>
        [Description("税款月结日期")]
        TaxMonthlyDay = 103,

        /// <summary>
        /// 税款垫款上限
        /// </summary>
        [Description("税款垫款上限")]
        TaxUpperLimit = 104,

        /// <summary>
        /// 税款汇率类型
        /// </summary>
        [Description("税款汇率类型")]
        TaxExchangeRateType = 105,

        /// <summary>
        /// 税款约定汇率值
        /// </summary>
        [Description("税款约定汇率值")]
        TaxExchangeRateValue = 106,

        /// <summary>
        /// 代理费结算方式
        /// </summary>
        [Description("代理费结算方式")]
        AgencyPeriodType = 201,

        /// <summary>
        /// 代理费约定期限
        /// </summary>
        [Description("代理费约定期限")]
        AgencyDaysLimit = 202,

        /// <summary>
        /// 代理费月结日期
        /// </summary>
        [Description("代理费月结日期")]
        AgencyMonthlyDay = 203,

        /// <summary>
        /// 代理费垫款上限
        /// </summary>
        [Description("代理费垫款上限")]
        AgencyUpperLimit = 204,

        /// <summary>
        /// 代理费汇率类型
        /// </summary>
        [Description("代理费汇率类型")]
        AgencyExchangeRateType = 205,

        /// <summary>
        /// 代理费约定汇率值
        /// </summary>
        [Description("代理费约定汇率值")]
        AgencyExchangeRateValue = 206,

        /// <summary>
        /// 杂费结算方式
        /// </summary>
        [Description("杂费结算方式")]
        OtherPeriodType = 301,

        /// <summary>
        /// 杂费约定期限
        /// </summary>
        [Description("杂费约定期限")]
        OtherDaysLimit = 302,

        /// <summary>
        /// 杂费月结日期
        /// </summary>
        [Description("杂费月结日期")]
        OtherMonthlyDay = 303,

        /// <summary>
        /// 杂费垫款上限
        /// </summary>
        [Description("杂费垫款上限")]
        OtherUpperLimit = 304,

        /// <summary>
        /// 杂费汇率类型
        /// </summary>
        [Description("杂费汇率类型")]
        OtherExchangeRateType = 305,

        /// <summary>
        /// 杂费约定汇率值
        /// </summary>
        [Description("杂费约定汇率值")]
        OtherExchangeRateValue = 306,
    }
}
