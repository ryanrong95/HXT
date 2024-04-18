using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class SwapNoticeReceiptUsesOrigin : UniqueView<Models.SwapNoticeReceiptUse, ScCustomsReponsitory>
    {
        public SwapNoticeReceiptUsesOrigin()
        {
        }

        public SwapNoticeReceiptUsesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SwapNoticeReceiptUse> GetIQueryable()
        {
            return from swapNoticeReceiptUse in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUses>()
                   select new Models.SwapNoticeReceiptUse
                   {
                       ID = swapNoticeReceiptUse.ID,
                       SourceID = swapNoticeReceiptUse.SourceID,
                       Type = (Enums.ReceiptUseType)swapNoticeReceiptUse.Type,
                       OrderReceiptID = swapNoticeReceiptUse.OrderReceiptID,
                       OrderReceiptAmount = swapNoticeReceiptUse.OrderReceiptAmount,
                       SwapNoticeID = swapNoticeReceiptUse.SwapNoticeID,
                       SwapNoticeAmountCNY = swapNoticeReceiptUse.SwapNoticeAmountCNY,
                       Status = (Enums.Status)swapNoticeReceiptUse.Status,
                       CreateDate = swapNoticeReceiptUse.CreateDate,
                       UpdateDate = swapNoticeReceiptUse.UpdateDate,
                       Summary = swapNoticeReceiptUse.Summary,
                   };
        }
    }
}
