using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PvWsOrderBaseOrderView : UniqueView<Models.PvWsOrderViewModel, ScCustomsReponsitory>
    {
        public PvWsOrderBaseOrderView()
        {

        }

        public PvWsOrderBaseOrderView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
       
        protected override IQueryable<Models.PvWsOrderViewModel> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvWsOrderBaseOrderView>()
                         select new Models.PvWsOrderViewModel
                         {
                             ID = c.ID,
                             OrderType = (Enums.ClientOrderType)c.Type
                         };

            return result;
        }
    }

   
}
