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
    public class DriversTopView : QueryView<Driver, PvWmsRepository>
    {
        public DriversTopView()
        {

        }
        public DriversTopView(PvWmsRepository repository) : base(repository)
        {
                
        }
        protected override IQueryable<Driver> GetIQueryable()
        {
            return new Yahv.Services.Views.DriversTopView<PvWmsRepository>();
        }
    }
}
