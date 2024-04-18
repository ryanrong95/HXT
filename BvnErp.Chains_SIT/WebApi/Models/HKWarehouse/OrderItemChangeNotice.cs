using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 到货通知 Request
    /// </summary>
    public class DeliveryNoticeRequest
    {
        //public string TinyOrderID { get; set; } = string.Empty;

        public string VastOrderID { get; set; }
    }

    /// <summary>
    /// 到货通知 Reponse
    /// </summary>
    public class DeliveryNoticeReponse : BaseApiResponse
    {

    }




    /// <summary>
    /// 产品变更 Request
    /// </summary>
    public class ItemChangeRequest
    {
        /// <summary>
        /// tongz
        /// </summary>
        public List<ItemChangeNotices> Notices{ get; set; }
    }

    /// <summary>
    /// 产品变更 Request
    /// </summary>
    public class ItemChangeNotices
    {
        /// <summary>
        /// d订单项ID  OrderItemID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 库房分拣员（变更产地/品牌）
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 旧的值 产地写 CHN USA等
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 新的值
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 品牌变更/产地变更
        /// </summary>
        public Needs.Ccs.Services.Enums.OrderItemChangeType Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

    }

    /// <summary>
    /// 拆项 Request
    /// </summary>
    public class SplitChangeRequest
    {
        /// <summary>
        /// 通知
        /// </summary>
        public List<SplitOrderItemNotice> Notices { get; set; }
    }

    /// <summary>
    /// 订单拆项变更
    /// </summary>
    public class SplitOrderItemNotice
    {
        /// <summary>
        /// 
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ItemID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TinyOrderID { get; set; }
        /// <summary>
        /// 库房分拣员（变更产地/品牌）
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }
    }
}