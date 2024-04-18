using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单管控项
    /// </summary>
    public class OrderControlItem : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem OrderItem { get; set; }

        /// <summary>
        /// 管控类型
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion
    }
}
