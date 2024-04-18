using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Wl.Models.Hanlders;

namespace Needs.Wl.Models
{
    public class Order : ModelBase<Layer.Data.Sqls.ScCustoms.Orders, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        /// <summary>
        /// 订单类型
        /// 内单:100、外单:200、Icgoo:300
        /// </summary>
        public Enums.OrderType Type { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 下单时的跟单员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public User User { get; set; }

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
        public Enums.InvoiceStatus InvoiceStatus { get; set; }

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
        public Enums.OrderStatus OrderStatus { get; set; }

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

        public Order()
        {
          
        }

        /// <summary>
        /// 当订单删除后发生
        /// </summary>
        public event OrderDeletedHanlder Deleted;

        void OnDeleted()
        {
            if (this != null && this.Deleted != null)
            {
                this.Deleted(this, new OrderDeletedEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            this.Status = (int)Enums.Status.Delete;
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnDeleted();
        }
    }
}