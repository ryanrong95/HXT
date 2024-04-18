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
    /// 账期条款视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class DebtTermsTopView<TReponsitory> : QueryView<DebtTerm, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public DebtTermsTopView()
        {

        }

        public DebtTermsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<DebtTerm> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.DebtTermsTopView>()
                   select new DebtTerm()
                   {
                       Payer = entity.Payer,
                       Business = entity.Business,
                       Catalog = entity.Catalog,
                       Payee = entity.Payee,
                       Days = entity.Days,
                       ExchangeType = (ExchangeType)entity.ERateType,
                       Months = entity.Months,
                       SettlementType = (SettlementType)entity.SettlementType,
                   };
        }
    }
}
