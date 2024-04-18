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
    /// 报关通知项的视图
    /// </summary>
    public class DeclarationNoticeItemsView : UniqueView<Models.DeclarationNoticeItem, ScCustomsReponsitory>
    {
        public DeclarationNoticeItemsView()
        {
        }

        internal DeclarationNoticeItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DeclarationNoticeItem> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);
            //var sortingView = new CenterSortingsView(this.Reponsitory);            

            return from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>()
                   join sorting in sortingView on noticeItem.SortingID equals sorting.ID
                   select new Models.DeclarationNoticeItem
                   {
                       ID = noticeItem.ID,
                       DeclarationNoticeID = noticeItem.DeclarationNoticeID,
                       Sorting = sorting,
                       Status = (Enums.DeclareNoticeItemStatus)noticeItem.Status,
                       CreateDate = noticeItem.CreateDate,
                       UpdateDate = noticeItem.UpdateDate,
                       Summary = noticeItem.Summary
                   };
        }
    }
}