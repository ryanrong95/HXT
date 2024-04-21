using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    public class CashStatisticsView : QueryView<Cash, PvbCrmReponsitory>
    {
        internal CashStatisticsView() { }

        internal CashStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Cash> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CashStatisticsView>()
                   select new Cash()
                   {
                       Currency = (Currency)entity.Currency,
                       Business = entity.Business,
                       Payee = entity.Payee,
                       Payer = entity.Payer,
                       Available = entity.Available,
                   };
        }
    }
}
