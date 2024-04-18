using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BasePortsView : UniqueView<Models.BasePort, ScCustomsReponsitory>
    {
        public BasePortsView()
        {
        }

        internal BasePortsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BasePort> GetIQueryable()
        {
            return from port in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePort>()
                   select new Models.BasePort
                   {
                       ID = port.ID,
                       Code = port.Code,
                       Name = port.Name,
                       EnglishName = port.EnglishName
                   };
        }
    }
}
