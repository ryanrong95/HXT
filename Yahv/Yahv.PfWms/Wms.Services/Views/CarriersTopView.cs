using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    /// <summary>
    /// 承运商视图
    /// </summary>
    public class CarriersTopView : UniqueView<Carrier, PvWmsRepository>
    {
        public CarriersTopView()
        {

        }

        public CarriersTopView(PvWmsRepository repository) : base(repository)
        {

        }

        protected override IQueryable<Carrier> GetIQueryable()
        {
            return new Yahv.Services.Views.CarriersTopView<PvWmsRepository>();
        }
    }
}
