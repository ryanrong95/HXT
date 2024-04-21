using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 信用批复统计
    /// </summary>
    public class CreditsStatisticsView : QueryView<CreditStatistic, PvbCrmReponsitory>
    {
        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.CreditsStatisticsView<PvbCrmReponsitory>();
        }

        public CreditStatistic this[string payer, string payee, string catalog, Currency currency]
        {
            get
            {
                return
                    this.SingleOrDefault(
                        item =>
                            item.Payer == payer && item.Payee == payee && item.Catalog == catalog &&
                            item.Currency == currency);
            }
        }
    }
}
