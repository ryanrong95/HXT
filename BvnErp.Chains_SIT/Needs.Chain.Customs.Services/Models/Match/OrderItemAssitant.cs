using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderItemAssitant
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public decimal Quantity { get; set; }
        public string Origin { get; set; }
        public string Unit { get; set; }
        public string Batch { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderID { get; set; }
        public string MatchedOrderItemID { get; set; }
        public string InputID { get; set; }
        /// <summary>
        /// 每个新插入的OrderItem的ClassifyStatus 插入DB的时候是Unclassified
        /// 自动归类成功，是First，没自动归类则不变
        /// 拆分时 Copy的归类结果，则为Done
        /// </summary>
        public Enums.ClassifyStatus ClassifyStatus { get; set; }
        /// <summary>
        /// 是否新增
        /// </summary>
        public Enums.PersistenceType PersistenceType { get; set; }
        /// <summary>
        /// 变更类型
        /// 1、产品变更- 需要重新归类
        /// 2、订单变更- 需要订单重新确认
        /// 3、没有变更
        /// </summary>
        public Enums.MatchChangeType ChangeType { get; set; }
        /// <summary>
        /// 产品变更
        /// 一个项，可能既有产地变更，又有品牌变更等
        /// </summary>
        public List<OrderItemChange> OrderItemChanges { get; set; }

        public OrderItemAssitant()
        {
            OrderItemChanges = new List<OrderItemChange>();
        }
    }

    public class OrderItemChange{
        public Enums.OrderItemChangeType OrderItemChangeType { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }

    public class Post2WarehouseModel
    {
        public string Unique { get; set; }
        public string ItemID { get; set; }
        public string TinyOrderID { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderItemChangeCauseOrderChange
    {
        public string OriginalOrderItemID { get; set; }
        public string OriginalModel { get; set; }
        public decimal OriginalQty { get; set; }
        public decimal NowQty { get; set; }
        public string Origin { get; set; }
        public Enums.OrderChangeCasuedReason ReasonType { get; set; }
    }


}
