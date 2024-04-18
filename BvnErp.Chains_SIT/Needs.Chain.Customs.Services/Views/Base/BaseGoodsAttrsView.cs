using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseGoodsAttrsView : UniqueView<Models.BaseGoodsAttr, ScCustomsReponsitory>
    {
        public BaseGoodsAttrsView()
        {
        }

        internal BaseGoodsAttrsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseGoodsAttr> GetIQueryable()
        {
            return from monitorWay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseGoodsAttr>()
                   select new Models.BaseGoodsAttr
                   {
                       ID = monitorWay.ID,
                       Code = monitorWay.Code,
                       Name = monitorWay.Name
                   };
        }
    }
}