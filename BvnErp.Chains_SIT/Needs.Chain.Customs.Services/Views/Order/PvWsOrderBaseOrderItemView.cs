using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    class PvWsOrderBaseOrderItemView : UniqueView<Models.PvWsOrderItemViewModel, ScCustomsReponsitory>
    {
        public PvWsOrderBaseOrderItemView()
        {

        }

        public PvWsOrderBaseOrderItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PvWsOrderItemViewModel> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvWsOrderBaseOrderItemsView>()
                         select new Models.PvWsOrderItemViewModel
                         {
                             ID = c.ID,
                             OrderID = c.OrderID,
                             TinyOrderID = c.TinyOrderID,
                             StorageID = c.StorageID
                         };

            return result;
        }
    }
}
