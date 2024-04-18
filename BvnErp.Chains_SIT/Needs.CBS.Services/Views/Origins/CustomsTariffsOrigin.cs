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
    /// 海关税则视图
    /// </summary>
    internal class CustomsTariffsOrigin : UniqueView<Models.Origins.CustomsTariff, ScCustomsReponsitory>
    {
        internal CustomsTariffsOrigin()
        {
        }

        internal CustomsTariffsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsTariff> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsTariffs>()
                   select new Models.Origins.CustomsTariff
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
                       Status = (Enums.Status)cert.Status,
                       CreateDate = cert.CreateDate,
                       UpdateDate = cert.UpdateDate,
                       Summary = cert.Summary,
                       InspectionCode = cert.InspectionCode,
                   };
        }
    }
}
