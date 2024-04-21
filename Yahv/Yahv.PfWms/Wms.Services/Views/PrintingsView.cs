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
    public class PrintingsView : UniqueView<Printings, PvWmsRepository>
    {
        protected override IQueryable<Printings> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Printings>()
                   select new Printings
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (PrintingType)entity.Type,
                       Width = entity.Width ?? 0,
                       Height = entity.Height ?? 0,
                       Url = entity.Url,
                       Summary = entity.Summary,
                       Status = (PrintingStatus)entity.Status

                   };
        }
    }
}
