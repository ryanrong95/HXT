using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    /// <summary>
    /// 供应商类型CrmPlus
    /// </summary>
    public enum SupplierType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,

        /// <summary>
        /// 生产商
        /// </summary>
        [Description("生产商")]
        Manufacturer = 1,
        /// <summary>
        /// 授权代理商
        /// </summary>
        [Description("授权代理商")]
        AuthorizedAgent = 2,
        /// <summary>
        /// 终端用户
        /// </summary>
        [Description("终端用户")]
        TerminalUser = 3,
        /// <summary>
        /// 贸易商
        /// </summary>
        [Description("贸易商")]
        Traders = 4,
        /// <summary>
        /// 后勤
        /// </summary>
        [Description("后勤")]
        Logistics = 5,

        /// <summary>
        /// 货代公司
        /// </summary>
        [Description("货代公司")]
        FreightForwarder = 6,

        #region  兼容旧版SupplierNature（陈经理确认） zhangbin 2021/06/02 

        /// <summary>
        /// 国际供应商
        /// </summary>
        [Description("国际供应商")]
        International = 11,
        /// <summary>
        /// 国内供应商
        /// </summary>
        [Description("国内供应商")]
        Domestic = 12,
        /// <summary>
        /// 市场供应商
        /// </summary>
        [Description("市场供应商")]
        Market = 13

        #endregion
    }

}
