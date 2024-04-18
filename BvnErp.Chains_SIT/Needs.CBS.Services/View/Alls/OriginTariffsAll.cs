using Layer.Data.Sqls;
using Needs.Ccs.Services;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Alls
{
    /// <summary>
    /// 原产地税率的视图
    /// </summary>
    public class CustomsOriginTariffsView : UniqueView<Model.Origins.CustomsOriginTariff, ScCustomsReponsitory>
    {
        public CustomsOriginTariffsView()
        {
        }
        protected override IQueryable<Model.Origins.CustomsOriginTariff> GetIQueryable()
        {
            return new Origins.CustomsOriginTariffOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }

    public class OriginTariffsAll : CustomsOriginTariffsView
    {
        public OriginTariffsAll()
        {

        }

        protected override IQueryable<Model.Origins.CustomsOriginTariff> GetIQueryable()
        {
            var customsTariffsView = new Origins.CustomsTariffOrigin(this.Reponsitory);
            var countriesView = new Origins.CustomsSettingOrigin(this.Reponsitory);

            return from originTariff in base.GetIQueryable()
                   join customsTariff in customsTariffsView on originTariff.CustomsTariffID equals customsTariff.ID
                   join country in countriesView on originTariff.Origin equals country.Code
                   where originTariff.Status == Enums.Status.Normal && customsTariff.Status == Enums.Status.Normal
                   where country.Type==Enums.BaseType.Country
                   select new Model.Origins.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       CustomsTariffID = originTariff.CustomsTariffID,
                       HSCode = customsTariff.HSCode,
                       Name = customsTariff.Name,
                       Origin = originTariff.Origin,
                       CountryName = country.Name,
                       Type = (Enums.CustomsRateType)originTariff.Type,
                       Rate = originTariff.Rate,
                       Status = (Enums.Status)originTariff.Status,
                       CreateDate = originTariff.CreateDate,
                       UpdateDate = originTariff.UpdateDate,
                       StartDate = originTariff.StartDate,
                       EndDate = originTariff.EndDate,
                       Summary = originTariff.Summary,
                   }; ;
        }
    }
}
