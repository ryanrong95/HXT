using System;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 应收视图
    /// </summary>
    public class ReceivablesView : Yahv.Linq.QueryView<Receivable, PvbCrmReponsitory>
    {
        public ReceivablesView()
        {

        }

        internal ReceivablesView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Receivable> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receivables>()
                   select new Receivable
                   {
                       ID = entity.ID,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       PayerID = entity.PayerID,
                       PayeeID = entity.PayeeID,
                       Business = entity.Business,
                       Catalog = entity.Catalog,
                       Subject = entity.Subject,
                       Currency = (Underly.Currency)entity.Currency,
                       Price = entity.Price,
                       Currency1 = (Underly.Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       SettlementCurrency = (Underly.Currency)entity.SettlementCurrency,
                       SettlementPrice = entity.SettlementPrice,
                       SettlementRate = entity.SettlementRate,
                       OrderID = entity.OrderID,
                       WaybillID = entity.WaybillID,
                       CreateDate = entity.CreateDate,
                       OriginalDate = entity.OriginalDate,
                       ChangeDate = entity.ChangeDate,
                       OriginalIndex = entity.OriginalIndex,
                       ChangeIndex = entity.ChangeIndex,
                       Summay = entity.Summay,
                       AdminID = entity.AdminID,
                       ItemID = entity.ItemID,
                       ApplicationID = entity.ApplicationID,
                       TinyID = entity.TinyID,
                       Rate11 = entity.Rate11 ?? 0,
                       Price11 = entity.Price11 ?? 0,
                       Currency11 = (Currency)entity.Currency11,
                   };
        }

        public Receivable this[string receivableId]
        {
            get { return this.SingleOrDefault(item => item.ID == receivableId); }
        }
    }
}
