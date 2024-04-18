using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Order : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID{ get;set;}

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 订单服务类型
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// 发票ID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 平台公司ID
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 受益人ID
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }


        /// <summary>
        /// 订单主状态
        /// </summary>
        public CgOrderStatus MainStatus { get; set; }

        /// <summary>
        /// 订单支付状态
        /// </summary>
        public OrderPaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 订单开票状态
        /// </summary>
        public OrderInvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 付汇状态
        /// </summary>
        public OrderRemittanceStatus RemittanceStatus { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 管理员（会员）ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public Currency? SettlementCurrency { get; set; }
        
        /// <summary>
        /// 收货订单拓展
        /// </summary>
        public OrderInput Input { get; set; }

        /// <summary>
        /// 发货订单拓展
        /// </summary>
        public OrderOutput Output { get; set; }
        #endregion
    }
}
