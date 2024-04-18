using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Alls
{
    /// <summary>
    /// 原产地税率的视图
    /// </summary>
    public class CustomsOriginTariffsAll : UniqueView<Models.Origins.CustomsOriginTariff, ScCustomsReponsitory>
    {
        public CustomsOriginTariffsAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsOriginTariff> GetIQueryable()
        {
            var originTariffsView = new Origins.CustomsOriginTariffsOrigin(this.Reponsitory).Where(t => t.Status == Enums.Status.Normal);
            var customsTariffsView = new Origins.CustomsTariffsOrigin(this.Reponsitory).Where(t => t.Status == Enums.Status.Normal);
            var countriesView = new Origins.CustomsSettingsOrigin(this.Reponsitory)[Enums.BaseType.Country];

            return from originTariff in originTariffsView
                   join customsTariff in customsTariffsView on originTariff.CustomsTariffID equals customsTariff.ID
                   join country in countriesView on originTariff.Origin equals country.Code
                   select new Models.Origins.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       CustomsTariffID = originTariff.CustomsTariffID,
                       HSCode = customsTariff.HSCode,
                       Name = customsTariff.Name,
                       Origin = originTariff.Origin,
                       CountryName = country.Name,
                       Type = originTariff.Type,
                       Rate = originTariff.Rate,
                       Status = originTariff.Status,
                       CreateDate = originTariff.CreateDate,
                       UpdateDate = originTariff.UpdateDate,
                       StartDate = originTariff.StartDate,
                       EndDate = originTariff.EndDate,
                       Summary = originTariff.Summary,
                   };
        }
    }
}
