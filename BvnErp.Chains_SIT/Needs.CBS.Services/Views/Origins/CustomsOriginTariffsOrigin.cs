using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 原产地税则视图
    /// </summary>
    internal class CustomsOriginTariffsOrigin : UniqueView<Models.Origins.CustomsOriginTariff, ScCustomsReponsitory>
    {
        internal CustomsOriginTariffsOrigin()
        {
        }

        internal CustomsOriginTariffsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsOriginTariff> GetIQueryable()
        {
            return from originTariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsOriginTariffs>()
                   select new Models.Origins.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       CustomsTariffID = originTariff.CustomsTariffID,
                       Origin = originTariff.Origin,
                       Type = (Enums.CustomsRateType)originTariff.Type,
                       Rate = originTariff.Rate,
                       Status = (Enums.Status)originTariff.Status,
                       CreateDate = originTariff.CreateDate,
                       UpdateDate = originTariff.UpdateDate,
                       StartDate = originTariff.StartDate,
                       EndDate = originTariff.EndDate,
                       Summary = originTariff.Summary,
                   };
        }
    }
}
