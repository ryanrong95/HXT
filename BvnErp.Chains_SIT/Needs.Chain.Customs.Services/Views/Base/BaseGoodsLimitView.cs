using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseGoodsLimitView : UniqueView<Models.BaseGoodsLimit, ScCustomsReponsitory>
    {
        public BaseGoodsLimitView()
        {
        }

        internal BaseGoodsLimitView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseGoodsLimit> GetIQueryable()
        {
            return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseGoodsLimit>()
                   select new Models.BaseGoodsLimit
                   {
                       ID = monitorWay.ID,
                       Code = monitorWay.Code,
                       Name = monitorWay.Name,
                       Type = monitorWay.Type
                   };
        }
    }
}