using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 产地消毒/检疫
    /// </summary>
    internal class OriginsDisinfectionOrigin : UniqueView<Models.OriginDisinfection, PvDataReponsitory>
    {
        internal OriginsDisinfectionOrigin()
        {
        }

        internal OriginsDisinfectionOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OriginDisinfection> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.OriginsDisinfection>()
                   select new Models.OriginDisinfection
                   {
                       ID = entity.ID,
                       Origin = entity.Origin,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
