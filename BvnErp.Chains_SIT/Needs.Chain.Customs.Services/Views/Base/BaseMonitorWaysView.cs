using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseMonitorWaysView : UniqueView<Models.BaseMonitorWay, ScCustomsReponsitory>
    {
        public BaseMonitorWaysView()
        {
        }

        internal BaseMonitorWaysView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseMonitorWay> GetIQueryable()
        {
            //return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseMonitorWays>()
            //       select new Models.BaseMonitorWay
            //       {
            //           ID = monitorWay.ID,
            //           Code = monitorWay.Code,
            //           Name = monitorWay.Name
            //       };

            return null;
        }
    }
}
