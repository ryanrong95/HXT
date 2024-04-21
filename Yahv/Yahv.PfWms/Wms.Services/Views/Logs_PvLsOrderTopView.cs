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
    public class Logs_PvLsOrderTopView : QueryView<Logs_PvLsOrder, PvWmsRepository>
    {

        public Logs_PvLsOrderTopView()
        {

        }

        public Logs_PvLsOrderTopView(PvWmsRepository repository) : base(repository)
        {

        }

        protected override IQueryable<Logs_PvLsOrder> GetIQueryable()
        {
            return new Yahv.Services.Views.Logs_PvLsOrderTopView<PvWmsRepository>();
        }
    }
}
