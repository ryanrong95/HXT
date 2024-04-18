using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseTradeModesView : UniqueView<Models.BaseTradeMode, ScCustomsReponsitory>
    {
        public BaseTradeModesView()
        {
        }

        internal BaseTradeModesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseTradeMode> GetIQueryable()
        {
            return from baseTradeMode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTradeMode>()
                   select new Models.BaseTradeMode
                   {
                       ID = baseTradeMode.ID,
                       Code = baseTradeMode.Code,
                       Name = baseTradeMode.Name
                   };
        }
    }
}