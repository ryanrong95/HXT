using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Underly
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum SelfStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        Auditing = 100,
        /// <summary>
        /// 已否决
        /// </summary>
        Vetoed = 104,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,
        ///<summary>
        /// 客户已发货
        ///</summary>
        BuyerShipMent = 205,
        /// <summary>
        /// 平台已收货
        /// </summary>
        SellerReceiving = 210,
        /// <summary>
        /// 卖方已验收
        /// </summary>
        SellerCheck = 215,
        /// <summary>
        /// 平台已发货
        /// </summary>
        SellerShipment = 220,
        /// <summary>
        /// 客户已收货
        /// </summary>
        BuyerReceiving = 225,
        /// <summary>
        /// 平台已退款
        /// </summary>
        WaitRefund = 230,
        /// <summary>
        /// 申请完成
        /// </summary>
        ApplyFinished = 235,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 400,
        /// <summary>
        /// 超级数据
        /// </summary>
        Super = 10000
    }
}
