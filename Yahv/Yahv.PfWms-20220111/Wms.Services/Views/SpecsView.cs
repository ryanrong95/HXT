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
    public class SpecsView : UniqueView<Models.Specs, PvWmsRepository>
    {
        protected override IQueryable<Specs> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Specs>()
                   select new Models.Specs
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       //Type = (Enums.SpecsType)entity.Type,
                       Width = entity.Width,
                       Length = entity.Length,
                       Height = entity.Height,
                       //RowTotal = entity.RowTotal,
                       //Volume = entity.Volume,
                       Load = entity.Load,
                   };
        }
    }
}
