using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单付款
    /// </summary>
    public class OrderPayment
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 费用类型：货款、关税、增值税、代理费、杂费
        /// </summary>
        public object Type { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count;

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public OrderPayment()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }
    }
}
