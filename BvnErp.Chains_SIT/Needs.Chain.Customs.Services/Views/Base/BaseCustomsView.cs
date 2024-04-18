using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseCustomsView : UniqueView<Models.BaseCustom, ScCustomsReponsitory>
    {
        public BaseCustomsView()
        {
        }

        internal BaseCustomsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseCustom> GetIQueryable()
        {
            //return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustoms>()
            //       select new Models.BaseCustom
            //       {
            //           ID = monitorWay.ID,
            //           Code = monitorWay.Code,
            //           Name = monitorWay.Name
            //       };
            return null;
        }
    }
}
