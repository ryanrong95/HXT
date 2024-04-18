using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayExchangeApplieItemsView : UniqueView<Models.PayExchangeApplyItem, ScCustomsReponsitory>
    {
        public PayExchangeApplieItemsView()
        {

        }

        internal PayExchangeApplieItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeApplyItem> GetIQueryable()
        {
            var OrderReceivablesView = new OrderReceivablesView(this.Reponsitory).Where(item => item.FeeType == Enums.OrderFeeType.Product);
            var OrderReceivedsView = new OrderReceivedsView(this.Reponsitory).Where(item => item.FeeType == Enums.OrderFeeType.Product);
            var Receivables = from entity in OrderReceivablesView
                              group entity by entity.OrderID into g
                              select new
                              {
                                  OrderID = g.Key,
                                  TotalAmount = g.Sum(c => c.Amount),
                              };
            var Receiveds = from entity in OrderReceivedsView
                            group entity by new { entity.OrderID,entity.FeeSourceID } into g
                            let dyjIDs = g.Select(t => "p" + t.DyjID).ToArray()
                            select new
                            {
                                OrderID = g.Key.OrderID,
                                FeeSourceID = g.Key.FeeSourceID,
                                TotalAmount = g.Sum(c => c.Amount),
                                DyjIDs = string.Join(",", dyjIDs),
                            };


            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                   join order in ordersView on entity.OrderID equals order.ID
                   join receivable in Receivables on entity.OrderID equals receivable.OrderID into receivables
                   from receivable in receivables.DefaultIfEmpty()
                   join received in Receiveds on  new { entity.OrderID , FeeSourceID  = entity.PayExchangeApplyID } equals new { received.OrderID ,received.FeeSourceID } into receiveds
                   from received in receiveds.DefaultIfEmpty()
                   select new Models.PayExchangeApplyItem
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       Amount = entity.Amount,
                       PayExchangeApplyID = entity.PayExchangeApplyID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,

                       DeclarePrice = order.DeclarePrice,
                       PaidExchangeAmount = order.PaidExchangeAmount,

                       ReceivableAmount = receivable == null ? 0M : receivable.TotalAmount,
                       ReceivedAmount = received == null ? 0M : received.TotalAmount,
                       DyjIDs = received == null ? "" : received.DyjIDs,
                   };
        }
    }



    public class PayExchangeApplieItemsOriginView : UniqueView<PayExchangeApplyForSplit, ScCustomsReponsitory>
    {
        public PayExchangeApplieItemsOriginView()
        {

        }

        internal PayExchangeApplieItemsOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PayExchangeApplyForSplit> GetIQueryable()
        {
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                   join apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on entity.PayExchangeApplyID equals apply.ID
                   join order in ordersView on entity.OrderID equals order.ID
                   where apply.Status == (int)Enums.Status.Normal
                   && apply.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled
                   select new PayExchangeApplyForSplit
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       Amount = entity.Amount,
                       PayExchangeApplyID = entity.PayExchangeApplyID,
                       CreateDate = entity.CreateDate,

                       Status = (Enums.Status)apply.Status,
                       SupplierEnglishName = apply.SupplierEnglishName,
                       BankName = apply.BankName,
                       BankAccount = apply.BankAccount,
                       PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)apply.PayExchangeApplyStatus,
                       IsAdvanceMoney = apply.IsAdvanceMoney,
                       FatherID = apply.FatherID
                   };
        }
    }


    public class PayExchangeApplyForSplit : IUnique
    {
        public string ID { get; set; }

        public string PayExchangeApplyID { get; set; }

        public string OrderID { get; set; }

        public decimal Amount { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 是否垫款
        /// </summary>
        public int? IsAdvanceMoney { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string FatherID { get; set; }
    }
}