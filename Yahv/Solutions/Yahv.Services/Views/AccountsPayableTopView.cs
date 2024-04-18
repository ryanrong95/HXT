using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 应付款 通用视图（待支付款）
    /// </summary>
    public class AccountsPayableTopView<TReponsitory> : QueryView<Balance, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public AccountsPayableTopView()
        {

        }

        public AccountsPayableTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Balance> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.VouchersStatisticsView>().Where(item => item.Status == (int)GeneralStatus.Normal)
                   group entity by new { entity.Payer, entity.Payee, entity.Business, entity.Catalog, entity.Currency }
                into g
                   select new Balance()
                   {
                       Currency = (Currency)g.Key.Currency,
                       Payer = g.Key.Payer,
                       Payee = g.Key.Payee,
                       Business = g.Key.Business,
                       Catalog = g.Key.Catalog,
                       //待支付款=应收金额-实收金额-减免金额
                       Price = g.Sum(item => item.LeftPrice) - (g.Sum(item => item.RightPrice) ?? 0) - (g.Sum(item => item.ReducePrice) ?? 0)
                   };
        }
    }
}
