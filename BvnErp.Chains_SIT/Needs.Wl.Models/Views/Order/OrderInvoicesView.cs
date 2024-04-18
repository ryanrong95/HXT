using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单的发票信息
    /// </summary>
    public class OrderInvoicesView : View<Models.OrderInvoice, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderInvoicesView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.OrderInvoice> GetIQueryable()
        {
            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                         join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on noticeItem.InvoiceNoticeID equals notice.ID
                         join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on noticeItem.OrderItemID equals orderItem.ID
                         join unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>() on orderItem.Unit equals unit.Code
                         where noticeItem.Status == (int)Needs.Wl.Models.Enums.Status.Normal && noticeItem.OrderID == this.OrderID
                         select new Models.OrderInvoice
                         {
                             ID = noticeItem.ID,
                             OrderID = noticeItem.OrderID,
                             OrderItemID = noticeItem.OrderItemID,
                             ProductUnionID = orderItem.ProductUniqueCode,
                             UnitPrice = noticeItem.UnitPrice,
                             Amount = noticeItem.Amount,
                             Difference = noticeItem.Difference,
                             InvoiceNo = noticeItem.InvoiceNo,
                             InvoiceTaxRate = notice.InvoiceTaxRate,
                             UnitName = unit.Name,
                         };
            return result;
        }

        /// <summary>
        /// 获取订单的发票号码
        /// </summary>
        /// <returns></returns>
        public string GetOrderInvoicesNo()
        {
            var query = this.GetIQueryable().Select(item => new
            {
                item.InvoiceNo
            }).Distinct().ToList();

            return string.Join(";", query);
        }
    }
}