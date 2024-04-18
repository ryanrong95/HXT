using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    public class CouponStatisticsTopView<TReponsitory> : QueryView<CouponStatistic, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CouponStatisticsTopView()
        {

        }
        public CouponStatisticsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CouponStatistic> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CouponStatisticsTopView>()
                   select new CouponStatistic
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       Type = (Underly.CouponType)entity.Type,
                       Conduct = entity.Conduct,
                       Catalog = entity.Catalog,
                       Subject = entity.Subject,
                       Currency = (Underly.Currency)entity.Currency,
                       Price = entity.Price,
                       InOrderCount = entity.InOrderCount,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Status = (Underly.GeneralStatus)entity.Status,

                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       Input = entity.Input.GetValueOrDefault(),
                       Output = entity.Output.GetValueOrDefault(),
                       Balance = entity.Balance.GetValueOrDefault()
                   };
        }
    }
}
