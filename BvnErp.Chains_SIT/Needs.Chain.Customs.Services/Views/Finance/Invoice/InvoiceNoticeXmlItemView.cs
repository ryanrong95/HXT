using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceNoticeXmlItemView : UniqueView<Models.InvoiceNoticeXmlItem, ScCustomsReponsitory>
    {
        public InvoiceNoticeXmlItemView()
        {

        }

        internal InvoiceNoticeXmlItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNoticeXmlItem> GetIQueryable()
        {
         

            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>()
                         where noticeItem.Status == (int)Enums.Status.Normal
                         select new Models.InvoiceNoticeXmlItem
                         {
                             ID = noticeItem.ID,
                             InvoiceNoticeXmlID = noticeItem.InvoiceNoticeXmlID,
                             InvoiceNoticeItemID = noticeItem.InvoiceNoticeItemID,
                             Xh = noticeItem.Xh,
                             Spmc = noticeItem.Spmc,
                             Ggxh = noticeItem.Ggxh,
                             Jldw = noticeItem.Jldw,
                             Spbm = noticeItem.Spbm,
                             Qyspbm = noticeItem.Qyspbm,
                             Syyhzcbz = noticeItem.Syyhzcbz,
                             Lslbz = noticeItem.Lslbz,
                             Yhzcsm = noticeItem.Yhzcsm,
                             Dj = noticeItem.Dj,
                             Sl = noticeItem.Sl,
                             Je = noticeItem.Je,
                             Slv = noticeItem.Slv,
                             Se = noticeItem.Se,
                             Kce = noticeItem.Kce,
                             Status = (Enums.Status)noticeItem.Status,
                             CreateDate = noticeItem.CreateDate,
                             UpdateDate = noticeItem.UpdateDate
                         };
            return result;
        }
    }
}
