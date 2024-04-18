using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 收货人类型
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// 收票地址
        /// </summary>
        [Description("收票地址")]
        Invoice=1,

        /// <summary>
        /// 收货地址
        /// </summary>
        [Description("收货地址")]
        Consignee=2,

        /// <summary>
        /// 交货地址
        /// </summary>
        [Description("交货地址")]
        Consignor=3,

        /// <summary>
        /// 办公地址
        /// </summary>
        [Description("办公地址")]
        Working=4,

        /// <summary>
        /// 研发地址
        /// </summary>
        [Description("研发地址")]
        Devloping=5,

        /// <summary>
        /// 生产地址
        /// </summary>
        [Description("生产地址")]
        Produce=6,
    }

}
