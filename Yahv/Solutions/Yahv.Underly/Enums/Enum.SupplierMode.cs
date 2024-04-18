using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 供应商类型CrmPlus
    /// </summary>
    public enum SupplierMode
    {
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
        Logistics = 5
    }

    /// <summary>
    /// /企业性质
    /// </summary>
    public enum EnterpriseNature
    {
        /// <summary>
        /// 国有企业
        /// </summary>
        [Description("国有企业")]
        StateOwned,
        /// <summary>
        /// 国有控股
        /// </summary>
        [Description("国有控股")]
        StateHolding,
        /// <summary>
        /// 外资企业
        /// </summary>
        [Description("外资企业")]
        ForeignFunded,
        /// <summary>
        /// 合资企业
        /// </summary>
        [Description("合资企业")]
        JointVentures,
        /// <summary>
        /// 私营企业
        /// </summary>
        [Description("私营企业")]
        PrivateEnterprises,
        /// <summary>
        /// 上市公司
        /// </summary>
        [Description("私营企业")]
        listed,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other

    }
}
