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
    /// 海关检疫视图
    /// </summary>
    internal class CustomsQuarantinesOrigin : UniqueView<Models.Origins.CustomsQuarantine, ScCustomsReponsitory>
    {
        internal CustomsQuarantinesOrigin()
        {
        }

        internal CustomsQuarantinesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsQuarantine> GetIQueryable()
        {
            return from Quarantine in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsQuarantines>()
                   select new Models.Origins.CustomsQuarantine
                   {
                       ID = Quarantine.ID,
                       Origin = Quarantine.Origin,
                       StartDate = Quarantine.StartDate,
                       EndDate = Quarantine.EndDate,
                       Status = (Enums.Status)Quarantine.Status,
                       CreateDate = Quarantine.CreateDate,
                       UpdateDate = Quarantine.UpdateDate,
                       Summary = Quarantine.Summary
                   };
        }
    }
}
