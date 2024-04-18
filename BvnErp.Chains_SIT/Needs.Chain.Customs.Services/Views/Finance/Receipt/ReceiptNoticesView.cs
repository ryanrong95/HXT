using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 收款通知的视图
    /// </summary>
    public class ReceiptNoticesView : UniqueView<Models.ReceiptNotice, ScCustomsReponsitory>
    {
        public ReceiptNoticesView()
        {

        }

        public ReceiptNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ReceiptNotice> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var financeReceiptsView = new FinanceReceiptsView(this.Reponsitory);

            var result = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                         join financeReceipt in financeReceiptsView on receiptNotice.ID equals financeReceipt.ID
                         join client in clientsView on receiptNotice.ClientID equals client.ID into clients
                         from client in clients.DefaultIfEmpty()
                         where receiptNotice.Status == (int)Enums.Status.Normal
                         select new Models.ReceiptNotice()
                         {
                             ID = receiptNotice.ID,
                             Client = client,
                             ClearAmount = receiptNotice.ClearAmount,
                             Status = (Status)receiptNotice.Status,
                             CreateDate = receiptNotice.CreateDate,
                             UpdateDate = receiptNotice.UpdateDate,
                             Summary = receiptNotice.Summary,
                             SeqNo = financeReceipt.SeqNo,
                             Account = financeReceipt.Account,
                             FeeType = financeReceipt.FeeType,
                             Amount = financeReceipt.Amount,
                             AvailableAmount = receiptNotice.AvailableAmount,
                             ReceiptDate = financeReceipt.ReceiptDate,
                             Vault = financeReceipt.Vault,
                             ReceiptType = financeReceipt.ReceiptType,
                             Admin = financeReceipt.Admin,
                             Currency = financeReceipt.Currency,
                             Rate = financeReceipt.Rate,
                             DyjID = financeReceipt.DyjID
                         };

            return result;
        }
    }

    public class ReceiptNoticesViewForReceivingList : UniqueView<Models.ReceiptNotice, ScCustomsReponsitory>
    {
        public ReceiptNoticesViewForReceivingList()
        {

        }

        public ReceiptNoticesViewForReceivingList(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ReceiptNotice> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);
            var financeReceiptsView = new FinanceReceiptsView(this.Reponsitory);
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var lastReceipts = from orderReceipt in orderReceipts
                               where orderReceipt.FinanceReceiptID != null
                                  && orderReceipt.Status == (int)Enums.Status.Normal
                                  && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                               orderby orderReceipt.CreateDate descending
                               group orderReceipt by new { orderReceipt.FinanceReceiptID } into g
                               select new
                               {
                                   FinanceReceiptID = g.Key.FinanceReceiptID,
                                   CreateDate = g.FirstOrDefault().CreateDate,
                               };

            var result = from receiptNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>()
                         join financeReceipt in financeReceiptsView on receiptNotice.ID equals financeReceipt.ID
                         join client in clientsView on receiptNotice.ClientID equals client.ID into clients
                         from client in clients.DefaultIfEmpty()

                         join lastReceipt in lastReceipts on receiptNotice.ID equals lastReceipt.FinanceReceiptID into lastReceipts2
                         from lastReceipt in lastReceipts2.DefaultIfEmpty()

                         where receiptNotice.Status == (int)Enums.Status.Normal
                         select new Models.ReceiptNotice()
                         {
                             ID = receiptNotice.ID,
                             Client = client,
                             ClearAmount = receiptNotice.ClearAmount,
                             Status = (Status)receiptNotice.Status,
                             CreateDate = receiptNotice.CreateDate,
                             UpdateDate = receiptNotice.UpdateDate,
                             Summary = receiptNotice.Summary,
                             SeqNo = financeReceipt.SeqNo,
                             Account = financeReceipt.Account,
                             FeeType = financeReceipt.FeeType,
                             Amount = financeReceipt.Amount,
                             ReceiptDate = financeReceipt.ReceiptDate,
                             Vault = financeReceipt.Vault,

                             LastReceiptDate = lastReceipt != null ? (DateTime?)lastReceipt.CreateDate : null,
                         };

            return result;
        }
    }

}
