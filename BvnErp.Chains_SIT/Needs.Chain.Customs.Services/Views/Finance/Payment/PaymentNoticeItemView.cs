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
    /// 付款通知明细的视图，也可作为订单的应收实收视图
    /// </summary>
    public class PaymentNoticeItemView : UniqueView<Models.PaymentNoticeItem, ScCustomsReponsitory>
    {
        public PaymentNoticeItemView()
        {
        }

        internal PaymentNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaymentNoticeItem> GetIQueryable()
        {
            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                         select new Models.PaymentNoticeItem
                         {
                             ID = noticeItem.ID,
                             PaymentNoticeID = noticeItem.PaymentNoticeID,
                             OrderID = noticeItem.OrderID,
                             PayFeeType = (Enums.FinanceFeeType)noticeItem.FeeType,
                             Amount = noticeItem.Amount,
                             Currency = noticeItem.Currency,
                             Status = (Enums.PaymentNoticeStatus)noticeItem.Status,
                             CreateDate = noticeItem.CreateDate,
                             UpdateDate = noticeItem.UpdateDate,
                             Summary = noticeItem.Summary,
                         };
            return result;
        }
    }
}
