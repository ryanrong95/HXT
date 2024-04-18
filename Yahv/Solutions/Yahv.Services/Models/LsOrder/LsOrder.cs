using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.LsOrder
{
    /// <summary>
    /// 租赁订单
    /// </summary>
    public class LsOrder : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// fatherID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public LsOrderType Type { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public LsOrderSource Source { get; set; }
        /// <summary>
        /// 订单是否被续租
        /// </summary>
        public bool InheritStatus { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 平台ID
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 受益人ID
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 发票ID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 是否开具发票
        /// </summary>
        public bool IsInvoiced { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime ? StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime ? EndDate { get; set; }

        /// <summary>
        /// 通用状态
        /// </summary>
        public LsOrderStatus Status { get; set; }

        /// <summary>
        /// 通用状态描述
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 开票状态
        /// </summary>
        public OrderInvoiceStatus InvoiceStatus { get; set; }
        #endregion
    }
}
