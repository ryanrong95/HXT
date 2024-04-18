using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PvWmsTWaybillsTopView : UniqueView<Models.PvWmsTWaybillsViewModel, ScCustomsReponsitory>
    {
        public PvWmsTWaybillsTopView()
        {

        }

        public PvWmsTWaybillsTopView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PvWmsTWaybillsViewModel> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvWmsTWaybillsTopView>()
                         select new Models.PvWmsTWaybillsViewModel
                         {
                             ID = c.ID,
                             OrderID = c.ForOrderID,
                             CreateDate = c.CreateDate
                         };

            return result;
        }
    }
}
