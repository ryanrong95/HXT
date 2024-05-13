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
    public class SummariesView : UniqueView<Summaries, PvWmsRepository>
    {
        protected override IQueryable<Summaries> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Summaries>()
                   select new Summaries {ID=entity.ID,Otype=entity.OType,Title=entity.Title,Summary=entity.Summary };
        }
    }
}
