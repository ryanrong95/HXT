using System;
using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 企业类型
    /// </summary>
    [Flags]
    public enum EnterpriseAccountType
    {
        /// <summary>
        /// 内部公司
        /// </summary>
        [Description("内部公司")]
        Company = 1,

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Client = 2,

        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Supplier = 4,
    }
}