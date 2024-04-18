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
    /// 税则的视图
    /// </summary>
    public class CustomsTariffsView : UniqueView<Models.CustomsTariff, ScCustomsReponsitory>,Needs.Underly.IFkoView<Models.CustomsTariff>
    {
        public CustomsTariffsView()
        {
        }

        internal CustomsTariffsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsTariff> GetIQueryable()
        {
            return from tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>()               
                   where tariff.Status == (int)Enums.Status.Normal
                   select new Models.CustomsTariff
                   {
                       ID = tariff.ID,
                       HSCode = tariff.HSCode,
                       Name = tariff.Name,
                       MFN = tariff.MFN,
                       General = tariff.General,
                       AddedValue = tariff.AddedValue,
                       Consume = tariff.Consume,
                       Elements = tariff.Elements,
                       RegulatoryCode = tariff.RegulatoryCode,
                       Unit1 = tariff.Unit1,
                       Unit2 = tariff.Unit2,
                       CIQCode = tariff.CIQCode,
                       Status = (Enums.Status)tariff.Status,
                       CreateDate = tariff.CreateDate,
                       UpdateDate = tariff.UpdateDate,
                       Summary = tariff.Summary,
                       InspectionCode = tariff.InspectionCode,
                   };
        }
    }
}
