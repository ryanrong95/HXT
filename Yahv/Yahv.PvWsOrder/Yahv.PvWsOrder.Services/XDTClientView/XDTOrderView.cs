using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class XDTOrderView : UniqueView<XDTOrder, ScCustomReponsitory>
    {
        IUser user;

        private XDTOrderView()
        {

        }

        public XDTOrderView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<XDTOrder> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>()
                .Where(item => item.Status == (int)GeneralStatus.Normal);
            if (this.user.IsMain)
            {
                orders = orders.Where(item => item.ClientID == this.user.XDTClientID);
            }
            else
            {
                orders = orders.Where(item => item.UserID == this.user.ID);
            }

            return from order in orders
                   select new XDTOrder
                   {
                       ID = order.ID,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (OrderInvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       MainOrderID = order.MainOrderId,
                       Type=order.Type,
                       OrderStatus = (OrderStatus)order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                   };
        }
    }

    public class XDTOrder : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 订单类型
        /// 内单:100、外单:200、Icgoo:300
        /// </summary>
        public int Type { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public string UserID { get; set; }

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
        public OrderInvoiceStatus InvoiceStatus { get; set; }

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
        /// 是否可以预换汇，否则不可以在报关前换汇
        /// </summary>
        public bool IsPrePayExchange { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 报关状态
        /// </summary>
        public int DeclareStatus { get; set; }


        public string MainOrderID { get; set; }

        public DateTime MainOrderCreateDate { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string[] SupplierName { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public OrderPayExchangeSupplier[] PayExchangeSuppliers { get; set; }
        #endregion

    }

}
