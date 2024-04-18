using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    public class TStoragesOrigin<TReponsitory> : UniqueView<Models.TStorages, PvWmsRepository>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TStoragesOrigin()
        {

        }

        public TStoragesOrigin(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.TStorages> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TStorages>()
                   select new Models.TStorages()
                   {
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       ProductID = entity.ProductID,
                       Quantity = entity.Quantity,
                       Origin = entity.Origin,
                       DateCode = entity.DateCode,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
