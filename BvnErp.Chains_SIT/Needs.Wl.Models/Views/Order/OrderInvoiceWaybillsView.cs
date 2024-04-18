using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单的发票运单
    /// </summary>
    public class OrderInvoiceWaybillsView : View<Models.OrderInvoiceWaybill, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderInvoiceWaybillsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.OrderInvoiceWaybill> GetIQueryable()
        {
            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                         join notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on noticeItem.InvoiceNoticeID equals notice.ID
                         join waybill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>() on notice.ID equals waybill.InvoiceNoticeID
                         join carrier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>() on waybill.CompanyName equals carrier.ID
                         where noticeItem.Status == (int)Needs.Wl.Models.Enums.Status.Normal && noticeItem.OrderID == this.OrderID
                         select new Models.OrderInvoiceWaybill
                         {
                             ID = noticeItem.ID,
                             WaybillCode = waybill.WaybillCode,
                             CreateDate = waybill.CreateDate,
                             Carrier = new Carrier()
                             {
                                 ID = carrier.ID,
                                 Name = carrier.Name
                             }
                         };
            return result;
        }
    }
}