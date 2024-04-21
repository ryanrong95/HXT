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
    public class SortingsView : UniqueView<Models.Sorting, PvWmsRepository>
    {
        public SortingsView()
        {

        }
        protected override IQueryable<Sorting> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Sortings>()
                   select new Models.Sorting
                   {
                       ID = entity.ID,
                       AdminID = entity.AdminID,
                       BoxCode = entity.BoxCode,
                       CreateDate = entity.CreateDate,
                       NoticeID = entity.NoticeID,
                       Quantity = entity.Quantity,                       
                       Weight = entity.Weight,
                       WaybillID=entity.WaybillID
                   };
        }
    }
}
