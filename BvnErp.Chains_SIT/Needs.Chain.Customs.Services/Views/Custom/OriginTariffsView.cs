using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 原产地税率的视图
    /// </summary>
    public class CustomsOriginTariffsView : UniqueView<Models.CustomsOriginTariff, ScCustomsReponsitory>
    {
        public CustomsOriginTariffsView()
        {
        }

        internal CustomsOriginTariffsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsOriginTariff> GetIQueryable()
        {
            return from originTariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsOriginTariffs>()
                   where originTariff.Status == (int)Enums.Status.Normal
                   select new Models.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       CustomsTariffID = originTariff.CustomsTariffID,
                       Origin = originTariff.Origin,
                       Type = (Enums.CustomsRateType)originTariff.Type,
                       Rate = originTariff.Rate,
                       Status = (Enums.Status)originTariff.Status,
                       CreateDate = originTariff.CreateDate,
                       UpdateDate = originTariff.UpdateDate,
                       StartDate=originTariff.StartDate,
                       EndDate = originTariff.EndDate,
                       Summary = originTariff.Summary,
                   };
        }
    }

    public class OriginTariffsView : CustomsOriginTariffsView
    {
        public OriginTariffsView()
        {

        }

        protected override IQueryable<CustomsOriginTariff> GetIQueryable()
        {
            var customsTariffsView = new CustomsTariffsView(this.Reponsitory);
            var countriesView = new BaseCountriesView(this.Reponsitory);

            return from originTariff in base.GetIQueryable()
                   join customsTariff in customsTariffsView on originTariff.CustomsTariffID equals customsTariff.ID
                   join country in countriesView on originTariff.Origin equals country.Code
                   where originTariff.Status == Enums.Status.Normal && customsTariff.Status == Enums.Status.Normal
                   select new Models.CustomsOriginTariff
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

    /// <summary>
    /// 原产地税率的视图--中心
    /// </summary>
    public class PvDataOriginTariffsView : UniqueView<Models.CustomsOriginTariff, ScCustomsReponsitory>
    {
        public PvDataOriginTariffsView()
        {
        }

        internal PvDataOriginTariffsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsOriginTariff> GetIQueryable()
        {
            return from originTariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OriginsATRateTopView>()
                   where originTariff.Status == (int)Enums.Status.Normal
                   select new Models.CustomsOriginTariff
                   {
                       ID = originTariff.ID,
                       HSCode = originTariff.TariffID,
                       Origin = originTariff.Origin,
                       Rate = originTariff.Rate,
                       //Status = (Enums.Status)originTariff.Status,
                       StartDate = originTariff.StartDate,
                       EndDate = originTariff.EndDate
                   };
        }
    }
}