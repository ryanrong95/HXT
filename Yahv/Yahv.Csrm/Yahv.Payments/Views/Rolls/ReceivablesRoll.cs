using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Payments.Views.Rolls
{
    /// <summary>
    /// 应收视图
    /// </summary>
    public class ReceivablesRoll : QueryView<Receivable, PvbCrmReponsitory>
    {
        public ReceivablesRoll()
        {

        }

        public ReceivablesRoll(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }



        protected override IQueryable<Receivable> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receivables>()
                   select new Receivable()
                   {
                       ID = entity.ID,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       PayerID = entity.PayerID,
                       PayeeID = entity.PayeeID,
                       Business = entity.Business,
                       Catalog = entity.Catalog,
                       Subject = entity.Subject,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       SettlementCurrency = (Currency)entity.SettlementCurrency,
                       SettlementPrice = entity.SettlementPrice,
                       SettlementRate = entity.SettlementRate,
                       OrderID = entity.OrderID,
                       WaybillID = entity.WaybillID,
                       CreateDate = entity.CreateDate,
                       OriginalDate = entity.OriginalDate,
                       ChangeDate = entity.ChangeDate,
                       OriginalIndex = entity.OriginalIndex,
                       ChangeIndex = entity.ChangeIndex,
                       AdminID = entity.AdminID,
                       Summay = entity.Summay,
                       //CouponCode = entity.CouponCode,
                   };
        }
    }
}