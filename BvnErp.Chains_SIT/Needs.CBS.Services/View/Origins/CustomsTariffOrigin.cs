using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsTariffOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsTariff, ScCustomsReponsitory>
    {
        internal CustomsTariffOrigin()
        {
        }

        internal CustomsTariffOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Model.Origins.CustomsTariff> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsTariffs>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsTariff
                   {
                       ID = cert.ID,
                       HSCode = cert.HSCode,
                       Name = cert.Name,
                       MFN = cert.MFN,
                       General = cert.General,
                       AddedValue = cert.AddedValue,
                       Consume = cert.Consume,
                       Elements = cert.Elements,
                       RegulatoryCode = cert.RegulatoryCode,
                       Unit1 = cert.Unit1,
                       Unit2 = cert.Unit2,
                       CIQCode = cert.CIQCode,
                       Status = (Cbs.Services.Enums.Status)cert.Status,
                       CreateDate = cert.CreateDate,
                       UpdateDate = cert.UpdateDate,
                       Summary = cert.Summary,
                       InspectionCode = cert.InspectionCode,
                   };
        }
    }
}
