using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单管控审批记录
    /// </summary>
    public class OrderControlRecord : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 管控ID
        /// </summary>
        public string OrderControlID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem OrderItem { get; set; }

        /// <summary>
        /// 管控类型
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 审核步骤/审核层级
        /// </summary>
        public Enums.OrderControlStep Step { get; set; }

        /// <summary>
        /// 管控状态
        /// </summary>
        public Enums.OrderControlStatus ControlStatus { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        public string ApproveSummary { get; set; }
        
    }
}
