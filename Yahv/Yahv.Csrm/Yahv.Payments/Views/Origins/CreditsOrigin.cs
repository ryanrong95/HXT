using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;

namespace Yahv.Payments.Views.Origins
{
    /// <summary>
    /// 信用统计视图
    /// </summary>
    public class CreditsOrigin : QueryView<Credit, PvbCrmReponsitory>
    {
        internal CreditsOrigin()
        {

        }

        internal CreditsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Credit> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CreditsStatisticsView>()
                   select new Credit()
                   {
                       Currency = (Currency)entity.Currency,
                       Business = entity.Business,
                       Catalog = entity.Catalog,
                       Payee = entity.Payee,
                       Payer = entity.Payer,
                       Total = entity.Total,
                       Cost = entity.Cost,
                   };
        }
    }
}
