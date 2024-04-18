using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseTrafModesView : UniqueView<Models.BaseTrafMode, ScCustomsReponsitory>
    {
        public BaseTrafModesView()
        {
        }

        internal BaseTrafModesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseTrafMode> GetIQueryable()
        {
            return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTrafMode>()
                   select new Models.BaseTrafMode
                   {
                       ID = monitorWay.ID,
                       Code = monitorWay.Code,
                       Name = monitorWay.Name
                   };
        }
    }
}