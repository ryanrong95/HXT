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
    public class TransportTopView : QueryView<Transport, PvWmsRepository>
    {
        public TransportTopView()
        {

        }
        public TransportTopView(PvWmsRepository repository) : base(repository)
        {
                
        }
        protected override IQueryable<Transport> GetIQueryable()
        {
            return new Yahv.Services.Views.TransportTopView<PvWmsRepository>();
        }
    }
}
