using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;

namespace Wms.Services.Views
{
    public class LsOrderTopView : QueryView<LsOrder, PvWmsRepository>
    {
        public LsOrderTopView()
        {

        }

        public LsOrderTopView(PvWmsRepository repository):base(repository)
        {
                
        }

        protected override IQueryable<LsOrder> GetIQueryable()
        {
            return new Yahv.Services.Views.LsOrderTopView<PvWmsRepository>();
        }
    }
}
