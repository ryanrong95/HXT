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
    public class PickingsView : UniqueView<Models.Pickings, PvWmsRepository>
    {
        public PickingsView()
        {

        }
        protected override IQueryable<Pickings> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Pickings>()
                   select new Models.Pickings
                   {
                       ID = entity.ID,
                       AdminID = entity.AdminID,
                       BoxingCode = entity.BoxCode,
                       CreateDate = entity.CreateDate,
                       NoticeID = entity.NoticeID,
                       Quantity = entity.Quantity,
                       Volume = entity.Volume,
                       Weight = entity.Weight,
                       NetWeight = entity.NetWeight,
                       StorageID = entity.StorageID
                   };
        }
    }
}
