using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;

namespace Wms.Services.Views
{
    public class WaybillBoxesView : Yahv.Linq.QueryView<Models.WaybillBox, PvWmsRepository>
    {
        protected override IQueryable<WaybillBox> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillBoxes>()
                   select new Models.WaybillBox
                   {
                       BoxID = entity.BoxID,        //
                       Specs = entity.Specs,        //
                       Weight = entity.Weight,        //
                       ShelveID = entity.ShelveID,        //
                   };
        }
    }
}
