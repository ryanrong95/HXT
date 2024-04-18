using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 换汇明细的视图
    /// </summary>
    public class SwapNoticeItemView : UniqueView<Models.SwapNoticeItem, ScCustomsReponsitory>
    {
        public SwapNoticeItemView()
        {
        }

        internal SwapNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapNoticeItem> GetIQueryable()
        {
            var decHeadView = new SwapDecHeadView(this.Reponsitory);

            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                         join dechead in decHeadView on noticeItem.DecHeadID equals dechead.ID
                         where noticeItem.Status == (int)Enums.Status.Normal
                         select new Models.SwapNoticeItem
                         {
                             ID = noticeItem.ID,
                             SwapNoticeID = noticeItem.SwapNoticeID,
                             SwapDecHead = dechead,
                             CreateDate= noticeItem.CreateDate,
                             Amount = noticeItem.Amount ?? 0,
                             Status = (Enums.Status)noticeItem.Status,
                         };
            return result;
        }
    }
}
