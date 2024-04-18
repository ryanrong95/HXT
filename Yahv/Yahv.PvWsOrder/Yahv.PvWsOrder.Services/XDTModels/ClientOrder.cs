using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// XDT订单
    /// </summary>
   public class ClientOrder:IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 订单类型
        /// 内单:100、外单:200、Icgoo:300
        /// </summary>
        public OrderType Type { get; set; }

        public string ClientID { get; set; }



        /// <summary>
        /// 下单时的会员补充协议
        /// </summary>
        public string AgreementID { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报价时的海关税率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 报价时的实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 是否包车：Yes/No
        /// </summary>
        public bool IsFullVehicle { get; set; }

        /// <summary>
        /// 是否垫款：Yes/No
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 包装种类
        /// </summary>
        public string WarpType { get; set; }

        /// <summary>
        /// 报关总金额
        /// 统计
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 订单的开票状态
        /// </summary>
        public InvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 已申请付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 是否挂起：Yes/No
        /// </summary>
        public bool IsHangUp { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 订单产品明细
        /// </summary>
        public OrderItems Items { get; set; }
        /// <summary>
        /// 主订单ID
        /// </summary>
        public string MainOrderID { get; set; }

        public System.DateTime MainOrderCreateDate { get; set; }
        #endregion
    }

    /// <summary>
    /// 订单本次申请金额
    /// </summary>
    public class OrderCurrentPayAmount : ClientOrder
    {
        /// <summary>
        /// 该订单中是否有供应商匹配型号
        /// </summary>
        public bool IsMatchSupplier { get; set; }

        /// <summary>
        /// 本次申请金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }
    }
}
