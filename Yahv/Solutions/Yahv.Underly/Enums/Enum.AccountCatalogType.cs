using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 收付款类型
    /// </summary>
    public enum AccountCatalogType
    {
        /// <summary>
        /// 收款类型
        /// </summary>
        [Description("收款类型")]
        Input = 1,

        /// <summary>
        /// 付款类型
        /// </summary>
        [Description("付款类型")]
        Output = 2
    }

    /// <summary>
    /// 收付款类型业务
    /// </summary>
    public enum AccountCatalogConduct
    {
        /// <summary>
        /// 商品销售业务
        /// </summary>
        [Description("商品销售业务")]
        Sale,
        /// <summary>
        /// 供应链业务
        /// </summary>
        [Description("供应链业务")]
        SupplyChain,
        /// <summary>
        /// 服务业务
        /// </summary>
        [Description("服务业务")]
        Service,
        /// <summary>
        /// 综合业务
        /// </summary>
        [Description("综合业务")]
        Integrat
    }
}