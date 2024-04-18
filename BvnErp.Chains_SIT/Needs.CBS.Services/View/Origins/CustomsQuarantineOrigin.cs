using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsQuarantineOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsQuarantine, ScCustomsReponsitory>
    {
        internal CustomsQuarantineOrigin()
        {
        }

        internal CustomsQuarantineOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Cbs.Services.Model.Origins.CustomsQuarantine> GetIQueryable()
        {
            return from Quarantine in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsQuarantines>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsQuarantine
                   {
                       ID = Quarantine.ID,
                       Origin = Quarantine.Origin,
                       StartDate = Quarantine.StartDate,
                       EndDate = Quarantine.EndDate,
                       Status = (Cbs.Services.Enums.Status)Quarantine.Status,
                       CreateDate = Quarantine.CreateDate,
                       UpdateDate = Quarantine.UpdateDate,
                       Summary = Quarantine.Summary
                   };
        }
    }
}
