using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsOriginTariffOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsOriginTariff, ScCustomsReponsitory>
    {
        internal CustomsOriginTariffOrigin()
        {
        }

        internal CustomsOriginTariffOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Cbs.Services.Model.Origins.CustomsOriginTariff> GetIQueryable()
        {
            return from originTariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsOriginTariffs>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       CustomsTariffID = originTariff.CustomsTariffID,
                       Origin = originTariff.Origin,
                       Type = (Cbs.Services.Enums.CustomsRateType)originTariff.Type,
                       Rate = originTariff.Rate,
                       Status = (Cbs.Services.Enums.Status)originTariff.Status,
                       CreateDate = originTariff.CreateDate,
                       UpdateDate = originTariff.UpdateDate,
                       StartDate = originTariff.StartDate,
                       EndDate = originTariff.EndDate,
                       Summary = originTariff.Summary,
                   };
        }
    }
}
