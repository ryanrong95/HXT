using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class WsOrder : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

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

        public Currency? SettlementCurrency { get; set; }

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
        /// 总价
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 发货订单拓展
        /// </summary>
        public WsOrderInput Input { get; set; }

        /// <summary>
        /// 收货订单拓展
        /// </summary>
        public WsOrderOutput Output { get; set; }

        public decimal PayAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        #endregion
    }

    /// <summary>
    /// 华芯通回传到货信息时使用
    /// </summary>
    public class OrderChanges
    {
        public string OrderID { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// 订单是否重新确认
        /// </summary>
        public bool Confirmed { get; set; }
        public List<OrderItemChanges> items { get; set; }

        public Currency CurrencyEx
        {
            get
            {
                return (Currency)Enum.Parse(typeof(Currency), this.Currency);
            }
        }
        /// <summary>
        /// 被拆的OrderItemID 需要生成新的InputID
        /// </summary>
        public List<string> OriginOrderItemIDs { get; set; }
    }
    public class OrderItemChanges
    {
        public string OrderItemID { get; set; }
        public string InputID { get; set; }
        public string CustomName { get; set; }
        public CenterProduct Product { get; set; }
        public string Origin { get; set; }
        public string DateCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal GrossWeight { get; set; }
        public string Unit { get; set; }
        public string TinyOrderID { get; set; }

        public string ProductID
        {
            get
            {
                Yahv.Services.Views.ProductsTopView<Layers.Data.Sqls.PvDataReponsitory>.Enter(this.Product);
                return this.Product.ID;
            }
        }

        public Origin OriginEx
        {
            get
            {
                return (Origin)Enum.Parse(typeof(Origin), this.Origin);
            }
        }
    }
}
