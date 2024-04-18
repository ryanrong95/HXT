using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 进项
    /// 采购单
    /// 
    /// </summary>
    public abstract class InOrder
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public object Supplier { get; set; }

        /// <summary>
        /// 采购时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 运单
        /// </summary>
        public object Waybill { get; set; }
    }

    /// <summary>
    /// 采购单
    /// 来源于代理报关订单
    /// 
    /// </summary>
    public class HTInOrder: InOrder
    {
        /// <summary>
        /// PI
        /// 发票
        /// </summary>
        public OrderFiles OrderFiles { get; set; }

        public HTInOrder()
        {

        }

        public HTInOrder(Order order)
        {
            this.CreateDate = order.CreateDate;
            //完成采购的进项
        }
    }

    /// <summary>
    /// 恒远的进项
    /// 恒远的采购单
    /// 来源于订单及报关单
    /// </summary>
    public class HYInOrder : InOrder
    {
        public HYInOrder()
        {

        }

        public HYInOrder(Order order)
        {
            this.CreateDate = order.CreateDate;
            //完成采购的进项
        }
    }
}
