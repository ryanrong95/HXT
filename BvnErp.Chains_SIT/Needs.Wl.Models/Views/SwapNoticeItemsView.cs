using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class SwapNoticeItemsView : UniqueView<Models.SwapNoticeItem, ScCustomsReponsitory>
    {
        public SwapNoticeItemsView()
        {

        }

        public SwapNoticeItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.SwapNoticeItem> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                   select new Models.SwapNoticeItem
                   {
                       ID = entity.ID,
                       SwapNoticeID = entity.SwapNoticeID,
                       DecHeadID = entity.DecHeadID,
                       CreateDate = entity.CreateDate,
                       Amount = entity.Amount ?? 0,
                       Status = (Enums.Status)entity.Status,
                   };
        }
    }
}
