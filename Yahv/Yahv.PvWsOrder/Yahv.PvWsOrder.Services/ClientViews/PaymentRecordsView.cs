using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class PaymentRecordsView : UniqueView<PaymentRecord, ScCustomReponsitory>
    {
        private IUser User;

        public PaymentRecordsView(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<PaymentRecord> GetIQueryable()
        {
            var financeReceipts = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.FinanceReceipts>();
            var receiptNotices = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ReceiptNotices>();

            var linq = from financeReceipt in financeReceipts
                       join receiptNotice in receiptNotices on financeReceipt.ID equals receiptNotice.ID
                       where receiptNotice.Status == (int)GeneralStatus.Normal
                       && receiptNotice.ClientID == this.User.XDTClientID
                       select new PaymentRecord
                       {
                           ID = financeReceipt.ID,
                           CreateDate = financeReceipt.CreateDate,
                           Amount = financeReceipt.Amount,
                       };

            return linq;
        }

        public Tuple<PaymentRecord[], int> ToPage(int page, int rows)
        {
            IQueryable<PaymentRecord> iquery = this.IQueryable.Cast<PaymentRecord>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();
            iquery = iquery.Skip(rows * (page - 1)).Take(rows);

            //获取数据
            var ienum_financeReceipt = iquery.ToArray();

            var financeReceiptIds = ienum_financeReceipt.Select(t => t.ID).ToArray();

            var orderReceipts = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderReceipts>();
            var item_orderReceipts = orderReceipts.Where(t => financeReceiptIds.Contains(t.FinanceReceiptID)
                                                            && t.Type == (int)OrderReceiptType.Received
                                                            && t.Amount < 0
                                                            && t.Status == (int)GeneralStatus.Normal).ToArray();

            var taxAgencyFeeTypes = new[] { (int)OrderFeeType.Tariff, (int)OrderFeeType.AddedValueTax, (int)OrderFeeType.AgencyFee, 
                                            (int)OrderFeeType.Incidental, (int)OrderFeeType.ExciseTax, };

            var result = (from financeReceipt in ienum_financeReceipt
                          orderby financeReceipt.CreateDate descending
                          select new PaymentRecord
                          {
                              ID = financeReceipt.ID,
                              CreateDate = financeReceipt.CreateDate,
                              Amount = financeReceipt.Amount,
                              PaymentDetails = (from item_orderReceipt in item_orderReceipts
                                                where item_orderReceipt.FinanceReceiptID == financeReceipt.ID
                                                group item_orderReceipt by new { item_orderReceipt.OrderID } into g
                                                orderby g.Key.OrderID descending
                                                select new PaymentRecord.PaymentDetail
                                                {
                                                    OrderID = g.Key.OrderID,
                                                    ProductFee = 0 - g.Where(t => t.FeeType == (int)OrderFeeType.Product).Sum(t => t.Amount),
                                                    TaxAgencyFee = 0 - g.Where(t => taxAgencyFeeTypes.Contains(t.FeeType)).Sum(t => t.Amount),
                                                }).ToList(),
                          }).ToArray();

            return new Tuple<PaymentRecord[], int>(result, total);
        }

    }
}
