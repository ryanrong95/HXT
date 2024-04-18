using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 订单收款的视图（用于统计客户订单的未付款/欠款）
    /// </summary>
    public class OrderReceiptsAllsView : UniqueView<Models.OrderReceipt, ScCustomsReponsitory>
    {
        public OrderReceiptsAllsView()
        {
        }

        internal OrderReceiptsAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderReceipt> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();

            var result = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                         join admin in adminsView on receipt.AdminID equals admin.ID

                         join financeReceipt in financeReceipts on receipt.FinanceReceiptID equals financeReceipt.ID into financeReceipts2
                         from financeReceipt in financeReceipts2.DefaultIfEmpty()

                         where receipt.Status == (int)Enums.Status.Normal
                         select new Models.OrderReceipt
                         {
                             ID = receipt.ID,
                             FinanceReceiptID = receipt.FinanceReceiptID,
                             ClientID = receipt.ClientID,
                             OrderID = receipt.OrderID,
                             FeeSourceID = receipt.FeeSourceID,
                             FeeType = (Enums.OrderFeeType)receipt.FeeType,
                             Type = (Enums.OrderReceiptType)receipt.Type,
                             Currency = receipt.Currency,
                             Rate = receipt.Rate,
                             Amount = receipt.Amount,
                             Admin = admin,
                             Status = (Enums.Status)receipt.Status,
                             CreateDate = receipt.CreateDate,
                             UpdateDate = receipt.UpdateDate,
                             Summary = receipt.Summary,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             IsLoan = order.IsLoan,

                             SeqNo = financeReceipt.SeqNo,
                             ReceiptDate = financeReceipt != null ? (DateTime?)financeReceipt.ReceiptDate : null,
                             ReImport = receipt.ReImport==null?false:receipt.ReImport.Value
                         };
            return result;
        }
    }

    /// <summary>
    /// 所有状态的（不排除非200状态的）订单收款的视图
    /// </summary>
    public class OrderReceiptsAllStatusView : UniqueView<Models.OrderReceipt, ScCustomsReponsitory>
    {
        public OrderReceiptsAllStatusView()
        {
        }

        internal OrderReceiptsAllStatusView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderReceipt> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();

            var result = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                         join admin in adminsView on receipt.AdminID equals admin.ID
                         join financeReceipt in financeReceipts on receipt.FinanceReceiptID equals financeReceipt.ID into FinanceReceipts2
                         from financeReceipt in FinanceReceipts2.DefaultIfEmpty()
                         select new Models.OrderReceipt
                         {
                             ID = receipt.ID,
                             FinanceReceiptID = receipt.FinanceReceiptID,
                             ClientID = receipt.ClientID,
                             OrderID = receipt.OrderID,
                             FeeSourceID = receipt.FeeSourceID,
                             FeeType = (Enums.OrderFeeType)receipt.FeeType,
                             Type = (Enums.OrderReceiptType)receipt.Type,
                             Currency = receipt.Currency,
                             Rate = receipt.Rate,
                             Amount = receipt.Amount,
                             Admin = admin,
                             Status = (Enums.Status)receipt.Status,
                             CreateDate = receipt.CreateDate,
                             UpdateDate = receipt.UpdateDate,
                             Summary = receipt.Summary,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             IsLoan = order.IsLoan,

                             SeqNo = financeReceipt != null ? financeReceipt.SeqNo : "",
                             ReceiptDate = financeReceipt != null ? (DateTime?)financeReceipt.ReceiptDate : null,
                             ReImport = receipt.ReImport == null ? false : receipt.ReImport.Value
                         };
            return result;
        }
    }

    /// <summary>
    /// 订单应收的视图
    /// </summary>
    public class OrderReceivablesView : UniqueView<Models.OrderReceivable, ScCustomsReponsitory>
    {
        public OrderReceivablesView()
        {
        }

        internal OrderReceivablesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<OrderReceivable> GetIQueryable()
        {
            var orderReceiptsView = new OrderReceiptsAllsView(this.Reponsitory);
            var result = from receipt in orderReceiptsView
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                         where receipt.Type == Enums.OrderReceiptType.Receivable && receipt.Status == Enums.Status.Normal
                         select new Models.OrderReceivable
                         {
                             ID = receipt.ID,
                             ClientID = receipt.ClientID,
                             OrderID = receipt.OrderID,
                             FeeSourceID = receipt.FeeSourceID,
                             FeeType = receipt.FeeType,
                             Type = receipt.Type,
                             Currency = receipt.Currency,
                             Rate = receipt.Rate,
                             Amount = receipt.Amount,
                             Admin = receipt.Admin,
                             Status = receipt.Status,
                             CreateDate = receipt.CreateDate,
                             UpdateDate = receipt.UpdateDate,
                             Summary = receipt.Summary,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             IsLoan = order.IsLoan,
                             ReImport = receipt.ReImport 
                         };
            return result;
        }
    }

    /// <summary>
    /// 订单实收的视图
    /// </summary>
    public class OrderReceivedsView : UniqueView<Models.OrderReceived, ScCustomsReponsitory>
    {
        public OrderReceivedsView()
        {
        }

        internal OrderReceivedsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderReceived> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var receiptNoticesView=new ReceiptNoticesView(this.Reponsitory);
            var result = from receipt in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on receipt.OrderID equals order.ID
                         join receiptNotice in receiptNoticesView on receipt.FinanceReceiptID equals receiptNotice.ID
                         join admin in adminsView on receipt.AdminID equals admin.ID
                         where receipt.Type == (int)Enums.OrderReceiptType.Received && receipt.Status == (int)Enums.Status.Normal
                         orderby receipt.CreateDate descending
                         select new Models.OrderReceived
                         {
                             ID = receipt.ID,
                             ReceiptNoticeID = receipt.FinanceReceiptID,
                             SeqNo = receiptNotice.SeqNo,
                             ClientID = receipt.ClientID,
                             OrderID = receipt.OrderID,
                             FeeSourceID = receipt.FeeSourceID,
                             FeeType = (Enums.OrderFeeType)receipt.FeeType,
                             Type = (Enums.OrderReceiptType)receipt.Type,
                             Currency = receipt.Currency,
                             Rate = receipt.Rate,
                             //实收金额在订单收款表中存的负值
                             Amount = -receipt.Amount,
                             Admin = admin,
                             Status = (Enums.Status)receipt.Status,
                             CreateDate = receipt.CreateDate,
                             UpdateDate = receipt.UpdateDate,
                             Summary = receipt.Summary,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             IsLoan = order.IsLoan,
                             DyjID = receiptNotice.DyjID,
                             ReImport = receipt.ReImport == null ? false : receipt.ReImport.Value
                         };
            return result;
        }
    }
}
