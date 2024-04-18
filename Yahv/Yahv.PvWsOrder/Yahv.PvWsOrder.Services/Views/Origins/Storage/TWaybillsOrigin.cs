using Layers.Data.Sqls;
using Layers.Data.Sqls.PvWms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    public class TWaybillsOrigin<TReponsitory> : UniqueView<Models.TWaybills, PvWmsRepository>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TWaybillsOrigin()
        {

        }

        public TWaybillsOrigin(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.TWaybills> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.TWaybills>()
                   select new Models.TWaybills()
                   {
                       ID = entity.ID,
                       WaybillCode = entity.WaybillCode,
                       WareHouseID = entity.WareHouseID,
                       CarrierID = entity.CarrierID,
                       EnterCode = entity.EnterCode,
                       ShelveID = entity.ShelveID,
                       Supplier = entity.Supplier,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       AdminID = entity.AdminID,
                       Status = (Enums.TempStorageStatus)entity.Status,
                       ForOrderID = entity.ForOrderID,
                       CompleteDate = entity.CompleteDate,
                   };
        }
    }
}
