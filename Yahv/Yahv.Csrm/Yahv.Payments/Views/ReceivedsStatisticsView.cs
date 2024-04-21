using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 实收通用视图
    /// </summary>
    public class ReceivedsStatisticsView : QueryView<ReceivedStatistic, PvbCrmReponsitory>
    {
        public ReceivedsStatisticsView()
        {

        }

        public ReceivedsStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ReceivedStatistic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ReceivedsStatisticsView>()
                   select new ReceivedStatistic()
                   {
                       Payer = entity.Payer,
                       PayerName = entity.PayerName,
                       Payee = entity.Payee,
                       OrderID = entity.OrderID,
                       Catalog = entity.Catalog,
                       Subject = entity.Subject,
                       CreateDate = entity.CreateDate,
                       Price = entity.Price,
                       PayeeName = entity.PayeeName,
                       AccountType = (AccountType)entity.AccountType,
                       ReceivableID = entity.ReceivableID,
                       Business = entity.Business,
                       TinyID = entity.TinyID,
                       AdminID = entity.AdminID,
                       ReceivedID = entity.ReceivedID,
                       SettlementCurrency = (Currency)entity.SettlementCurrency,
                       FlowID = entity.FlowID,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                   };
        }
    }
}