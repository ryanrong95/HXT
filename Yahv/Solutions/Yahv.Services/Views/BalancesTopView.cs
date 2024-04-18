using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 余额 通用视图（现金）
    /// </summary>
    public class BalancesTopView<TReponsitory> : QueryView<Balance, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public BalancesTopView()
        {

        }
        public BalancesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Balance> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowAccountsTopView>()
                   where entity.Type == 30
                   group entity by new { entity.Currency, entity.Payer, entity.Payee, entity.Business }
                into g
                   select new Balance()
                   {
                       Currency = (Currency)g.Key.Currency,
                       Price = g.Sum(item => item.Price),
                       Payer = g.Key.Payer,
                       Payee = g.Key.Payee,
                       Business = g.Key.Business,
                   };
        }
    }
}
