using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.LsOrder
{
    public class LsOrderItem : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单主键ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 供应商（冗余字段）
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        /// <summary>
        /// 租赁业务拓展
        /// </summary>
        public OrderItemsLease Lease { get; set; }

        public LsProducts Product { get; set; }

    }

    /// <summary>
    /// 订单项库位租赁拓展
    /// </summary>
    public class OrderItemsLease : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 租赁开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 租赁结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 租赁状态（到期，存续）
        /// </summary>
        public LsStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
        #endregion

        #region 扩展属性
        public int Month
        {
            get
            {
                return (this.EndDate.Year - this.StartDate.Year) * 12 + this.EndDate.Month - this.StartDate.Month;
            }
        }
        #endregion
    }
}
