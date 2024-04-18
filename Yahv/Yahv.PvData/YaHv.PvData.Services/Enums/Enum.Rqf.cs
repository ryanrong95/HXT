using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// [询报价]客户业务类型
    /// </summary>
    public enum RqfBussinessType
    {
        /// <summary>
        /// 传统贸易
        /// </summary>
        [Description("传统贸易")]
        Tradition = 1,
        /// <summary>
        /// 代理推广
        /// </summary>
        [Description("代理推广")]
        Agent = 2,
        /// <summary>
        /// 线上推广
        /// </summary>
        [Description("线上推广")]
        Online = 3
    }

    /// <summary>
    /// 数量说明
    /// </summary>
    public enum QuantityRemark
    {
        /// <summary>
        /// 不限、未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 实际采购
        /// </summary>
        [Description("实际采购")]
        Purchase = 1,
        /// <summary>
        /// 季度用量
        /// </summary>
        [Description("季度用量")]
        Quarterly = 2
    }

    /// <summary>
    /// 合同盖章类型
    /// </summary>
    public enum SealType
    {
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 客户先盖
        /// </summary>
        [Description("客户先盖")]
        CustomerFirst = 1,
        /// <summary>
        /// 我方先盖
        /// </summary>
        [Description("我方先盖")]
        CompanyFirst = 2,
        /// <summary>
        /// 无需盖章
        /// </summary>
        [Description("无需盖章")]
        NoNeed = 3,
    }

    /// <summary>
    /// 付款方式
    /// </summary>
    public enum DyjPayMethord
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unkonwn = 0,
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 1,
        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 2,
        /// <summary>
        /// 现金电汇
        /// </summary>
        [Description("现金电汇")]
        CashWireTransfer = 3,
        /// <summary>
        /// 帐号电汇
        /// </summary>
        [Description("帐号电汇")]
        AccountWireTransfer = 4,
        /// <summary>
        /// 美金
        /// </summary>
        [Description("美金")]
        Doller = 5,
    }
}
