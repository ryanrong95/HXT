using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 根据这些类型来判断，到货确认应该做的事情
    /// 产品变更要做的事情
    /// 订单变更要做的事情
    /// </summary>
    public enum MatchChangeType
    {
        /// <summary>
        /// 产品变更
        /// </summary>
        [Description("产品变更")]
        ProductChange = 1,

        /// <summary>
        /// 订单变更
        /// </summary>
        [Description("订单变更")]
        OrderChange = 2,

        /// <summary>
        /// 没有变更
        /// </summary>
        [Description("没有变更")]
        NoChange = 3,
    }

    public enum PersistenceType
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Insert = 0,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 2,
    }

    public enum OrderChangeCasuedReason
    {
        [Description("新增订单项")]
        AddOrderItem = 0,

        [Description("删除订单项")]
        DeleteOrderItem = 1,

        [Description("修改数量")]
        ChangeQty =2,

        [Description("重新归类")]
        ReClassify = 3,
    }
}
