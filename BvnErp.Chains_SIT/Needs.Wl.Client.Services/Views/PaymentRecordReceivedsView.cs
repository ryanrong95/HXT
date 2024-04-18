using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的付款实收明细
    /// </summary>
    public class PaymentRecordReceivedsView : View<Needs.Wl.Models.OrderReceivables, ScCustomsReponsitory>
    {
        private string PaymentRecordID;

        public PaymentRecordReceivedsView(string paymentRecordID)
        {
            this.PaymentRecordID = paymentRecordID;
        }

        protected override IQueryable<Needs.Wl.Models.OrderReceivables> GetIQueryable()
        {
            return from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                   where receipt.Status == (int)Needs.Wl.Models.Enums.Status.Normal && receipt.Type == (int)Needs.Wl.Models.Enums.OrderReceiptType.Received
                   && receipt.FinanceReceiptID == this.PaymentRecordID
                   orderby receipt.CreateDate descending
                   select new Needs.Wl.Models.OrderReceivables
                   {
                       ID = receipt.ID,
                       ClientID = receipt.ClientID,
                       OrderID = receipt.OrderID,
                       FeeType = (Needs.Wl.Models.Enums.OrderFeeType)receipt.FeeType,
                       Currency = receipt.Currency,
                       Rate = receipt.Rate,
                       Amount = receipt.Amount,
                       Status = receipt.Status,
                       CreateDate = receipt.CreateDate,
                       UpdateDate = receipt.UpdateDate,
                       Summary = receipt.Summary
                   };
        }
    }
}