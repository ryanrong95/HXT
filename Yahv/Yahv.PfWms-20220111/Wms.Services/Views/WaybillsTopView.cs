using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    public class ServicesWaybillsTopView : QueryView<Waybill, PvWmsRepository>
    {
        public ServicesWaybillsTopView()
        {

        }
        public ServicesWaybillsTopView(PvWmsRepository repository) :base(repository)
        {

        }
        protected override IQueryable<Waybill> GetIQueryable()
        {
            return new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>();
        }
    }
}
