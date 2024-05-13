using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{
    public class WayLoadingsView : UniqueView<WayLoadings, PvWmsRepository>
    {
        protected override IQueryable<WayLoadings> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WayLoadings>()
                   select new WayLoadings
                   {
                       ID = entity.ID,
                       TakingDate = entity.TakingDate,
                       TakingAddress = entity.TakingAddress,
                       TakingContact = entity.TakingContact,
                       TakingPhone = entity.TakingPhone,
                       CarNumber1 = entity.CarNumber1,
                       Driver = entity.Driver,
                       Carload = entity.Carload,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID
                   };
        }
    }
}
