using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsCiqCodeOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsCiqCode, ScCustomsReponsitory>
    {
        internal CustomsCiqCodeOrigin()
        {
        }

        internal CustomsCiqCodeOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Cbs.Services.Model.Origins.CustomsCiqCode> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsCiqCodes>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsCiqCode
                   {
                       ID = cert.ID,
                       Category = cert.Category,
                       CreateDate = cert.CreateDate,
                       InspectionCode = cert.InspectionCode,
                       Name = cert.Name,
                       Status = (Needs.Cbs.Services.Enums.Status)cert.Status,
                       Summary = cert.Summary,
                       UpdateDate = cert.UpdateDate,
                   };
        }
    }
}
