using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class SwapNoticeReceiptUseLogsOrigin : UniqueView<Models.SwapNoticeReceiptUseLog, ScCustomsReponsitory>
    {
        public SwapNoticeReceiptUseLogsOrigin()
        {
        }

        public SwapNoticeReceiptUseLogsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapNoticeReceiptUseLog> GetIQueryable()
        {
            return from swapNoticeReceiptUseLog in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeReceiptUseLogs>()
                   select new Models.SwapNoticeReceiptUseLog
                   {
                       ID = swapNoticeReceiptUseLog.ID,
                       SwapNoticeReceiptUseID = swapNoticeReceiptUseLog.SwapNoticeReceiptUseID,
                       OrderReceiptID = swapNoticeReceiptUseLog.OrderReceiptID,
                       OrderReceiptAmount = swapNoticeReceiptUseLog.OrderReceiptAmount,
                       SwapNoticeID = swapNoticeReceiptUseLog.SwapNoticeID,
                       SwapNoticeAmountCNY = swapNoticeReceiptUseLog.SwapNoticeAmountCNY,
                       Status = (Enums.Status)swapNoticeReceiptUseLog.Status,
                       CreateDate = swapNoticeReceiptUseLog.CreateDate,
                       UpdateDate = swapNoticeReceiptUseLog.UpdateDate,
                       Summary = swapNoticeReceiptUseLog.Summary,
                   };
        }
    }
}
