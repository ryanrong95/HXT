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
    public class EnterprisesTopView : QueryView<Enterprises, PvWmsRepository>
    {
        protected override IQueryable<Enterprises> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.EnterprisesTopView>()
                   select new Enterprises
                   {
                       ID = entity.ID,
                       Name = entity.Name
                   };
        }
    }
}