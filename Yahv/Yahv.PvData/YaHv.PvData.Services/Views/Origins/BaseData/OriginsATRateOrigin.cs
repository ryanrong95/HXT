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
    /// 产地加征税率
    /// </summary>
    internal class OriginsATRateOrigin : UniqueView<Models.OriginATRate, PvDataReponsitory>
    {
        internal OriginsATRateOrigin()
        {
        }

        internal OriginsATRateOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OriginATRate> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.OriginsATRate>()
                   select new Models.OriginATRate
                   {
                       ID = entity.ID,
                       TariffID = entity.TariffID,
                       Origin = entity.Origin,
                       Rate = entity.Rate,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
