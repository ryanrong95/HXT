using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseEdocCodesView : UniqueView<Models.BaseEdocCode, ScCustomsReponsitory>
    {
        public BaseEdocCodesView()
        {
        }

        internal BaseEdocCodesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseEdocCode> GetIQueryable()
        {
            return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseEdocCode>()
                   select new Models.BaseEdocCode
                   {
                       ID = monitorWay.ID,
                       Code = monitorWay.Code,
                       Name = monitorWay.Name
                   };
        }
    }
}