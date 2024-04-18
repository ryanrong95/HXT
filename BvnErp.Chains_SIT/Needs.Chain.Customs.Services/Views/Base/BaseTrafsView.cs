using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseTrafsView : UniqueView<Models.BaseTraf, ScCustomsReponsitory>
    {
        public BaseTrafsView()
        {
        }

        internal BaseTrafsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseTraf> GetIQueryable()
        {
            //return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTrafs>()
            //       select new Models.BaseTraf
            //       {
            //           ID = monitorWay.ID,
            //           Code = monitorWay.Code,
            //           Name = monitorWay.Name
            //       };
            return null;
        }
    }
}
