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
    public class PickingNoticesView : QueryView<Yahv.Services.Models.PickingNotice, PvWmsRepository>
    {
        public PickingNoticesView()
        {

        }
        public PickingNoticesView(PvWmsRepository repository) : base(repository)
        {
                
        }
        protected override IQueryable<Yahv.Services.Models.PickingNotice> GetIQueryable()
        {
            return new Yahv.Services.Views.PickingNoticesView<PvWmsRepository>();
        }
    }
}
