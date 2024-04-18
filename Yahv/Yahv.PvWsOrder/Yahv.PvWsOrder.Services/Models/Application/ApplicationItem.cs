using Layers.Data.Sqls;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 收付款申请项明细
    /// </summary>
    public class ApplicationItem : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ApplicationID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        public GeneralStatus Status { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                using (OrderItemAlls items = new OrderItemAlls())
                {
                    return items.SearchByOrderID(this.OrderID).ToArray().Sum(item => item.TotalPrice);
                }
            }
        }
        /// <summary>
        /// 已申请总金额
        /// </summary>
        public decimal AppliedPrice { get; set; }
        /// <summary>
        /// 可申请金额
        /// </summary>
        public decimal AppLeftPrice => TotalPrice - AppliedPrice;

        public Currency? SettlementCurrency { get; set; }
        public string Currency => SettlementCurrency == null ? "" : SettlementCurrency.GetDescription();
        public string SupplierID { get; set; }
        #endregion

        public ApplicationItem()
        {
            this.Status = GeneralStatus.Normal;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>().Any(item => item.ID == this.ID))
                {
                    this.ID = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ApplicationItems
                    {
                        ID = this.ID,
                        ApplicationID = this.ApplicationID,
                        OrderID = this.OrderID,
                        Amount = this.Amount,
                        Status = (int)this.Status,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ApplicationItems>(new
                    {
                        Amount = this.Amount,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }
            };
        }
    }
}
