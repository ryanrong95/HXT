using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Erm.Services.Views
{
    public class WarehousePlatesTopView : UniqueView<WarehousePlate, PvbCrmReponsitory>
    {
        protected override IQueryable<WarehousePlate> GetIQueryable()
        {
            return new Yahv.Services.Views.WarehousePlatesTopView<PvbCrmReponsitory>();
        }
    }
}
