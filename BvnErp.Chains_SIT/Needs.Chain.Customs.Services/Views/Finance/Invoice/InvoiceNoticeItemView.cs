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
    /// 开票申请项的视图
    /// </summary>
    public class InvoiceNoticeItemView : UniqueView<Models.InvoiceNoticeItem, ScCustomsReponsitory>
    {
        public InvoiceNoticeItemView()
        {

        }

        internal InvoiceNoticeItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {
            var orderItemView = new OrderItemsView(this.Reponsitory);
            var noticeView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var unitview = new BaseUnitsView(this.Reponsitory);

            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                         join orderItem in orderItemView on noticeItem.OrderItemID equals orderItem.ID into orderItems
                         from orderItem in orderItems.DefaultIfEmpty()
                         join unit in unitview on orderItem.Unit equals unit.Code into units
                         from unit in units.DefaultIfEmpty()
                         join notice in noticeView on noticeItem.InvoiceNoticeID equals notice.ID
                         select new Models.InvoiceNoticeItem
                         {
                             ID = noticeItem.ID,
                             InvoiceNoticeID = noticeItem.InvoiceNoticeID,
                             OrderID = noticeItem.OrderID,
                             OrderItem = orderItem,
                             UnitPrice = noticeItem.UnitPrice,
                             Amount = noticeItem.Amount,
                             Difference = noticeItem.Difference,
                             InvoiceCode = noticeItem.InvoiceCode,
                             InvoiceNo = noticeItem.InvoiceNo,
                             InvoiceDate = noticeItem.InvoiceDate,
                             Status = (Enums.Status)noticeItem.Status,
                             CreateDate = noticeItem.CreateDate,
                             UpdateDate = noticeItem.UpdateDate,
                             Summary = noticeItem.Summary,
                             InvoiceTaxRate = notice.InvoiceTaxRate,

                             UnitName = unit.Name,
                             ClientID = notice.ClientID,
                         };
            return result;
        }
    }


    public class InvoiceDetaiView : UniqueView<Models.InvoiceNoticeItem, ScCustomsReponsitory>
    {
        public InvoiceDetaiView()
        {

        }

        internal InvoiceDetaiView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {
            var orderItemView = new OrderItemsView(this.Reponsitory);
            var noticeView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>();
            var clientView = new ClientsView(this.Reponsitory);

            var unitview = new BaseUnitsView(this.Reponsitory);

            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                         join orderItem in orderItemView on noticeItem.OrderItemID equals orderItem.ID into orderItems
                         from orderItem in orderItems.DefaultIfEmpty()
                         join unit in unitview on orderItem.Unit equals unit.Code into units
                         from unit in units.DefaultIfEmpty()
                         join notice in noticeView on noticeItem.InvoiceNoticeID equals notice.ID
                         join client in clientView on notice.ClientID equals client.ID
                         where notice.Status == (int)Enums.InvoiceNoticeStatus.Confirmed
                         orderby noticeItem.CreateDate descending
                         select new Models.InvoiceNoticeItem
                         {
                             ID = noticeItem.ID,
                             InvoiceNoticeID = noticeItem.InvoiceNoticeID,
                             OrderID = noticeItem.OrderID,
                             OrderItem = orderItem,
                             //OrderItem = new OrderItem
                             //{
                             //    Category = orderItem.Category,
                             //    Product = orderItem.Product,
                             //    Unit = unit.Name,
                             //    Quantity = orderItem.Quantity
                             //},
                             UnitName = unit.Name,
                             UnitPrice = noticeItem.UnitPrice,
                             Amount = noticeItem.Amount,
                             Difference = noticeItem.Difference,
                             InvoiceNo = noticeItem.InvoiceNo,
                             Status = (Enums.Status)noticeItem.Status,
                             CreateDate = noticeItem.CreateDate,
                             UpdateDate = noticeItem.UpdateDate,
                             Summary = noticeItem.Summary,

                             Client = client,
                             InvoiceTaxRate = notice.InvoiceTaxRate,
                             InvoiceTime = notice.UpdateDate,
                         };
            return result;
        }
    }

    public class InvoiceXmlDetaiView : UniqueView<Models.InvoiceNoticeItem, ScCustomsReponsitory>
    {
        public InvoiceXmlDetaiView()
        {

        }

        internal InvoiceXmlDetaiView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {
            var orderItemView = new OrderItemsView(this.Reponsitory);
            var unitview = new BaseUnitsView(this.Reponsitory);

            var result = from noticeXmlItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>()
                         join xmlItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>() on noticeXmlItem.InvoiceNoticeXmlID equals xmlItem.ID
                         join noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>() on noticeXmlItem.InvoiceNoticeItemID equals noticeItem.ID
                         join orderItem in orderItemView on noticeItem.OrderItemID equals orderItem.ID into orderItems
                         from orderItem in orderItems.DefaultIfEmpty()
                         join unit in unitview on orderItem.Unit equals unit.Code into units
                         from unit in units.DefaultIfEmpty()                                    
                         where xmlItem.InvoiceNo != null
                         orderby noticeItem.CreateDate descending
                         select new Models.InvoiceNoticeItem
                         {
                             ID = noticeItem.ID,
                             InvoiceNoticeID = noticeItem.InvoiceNoticeID,
                             OrderID = noticeItem.OrderID,
                             OrderItem = orderItem,
                           
                             UnitName = unit.Name,
                             UnitPrice = noticeXmlItem.Dj,
                             Amount = noticeXmlItem.Sl,
                             Difference = noticeItem.Difference,
                             InvoiceNo = xmlItem.InvoiceNo,                           
                             CreateDate = noticeXmlItem.CreateDate,
                             UpdateDate = noticeXmlItem.UpdateDate,
                             Summary = noticeItem.Summary,

                             Client = new Client
                             {
                                 Company = new Company
                                 {
                                     Name = xmlItem.Gfmc
                                 }
                             },
                             InvoiceTaxRate = noticeXmlItem.Slv,
                             InvoiceTime = xmlItem.InvoiceDate,
                             Tax = noticeXmlItem.Se,
                             TaxFreeAmout = noticeXmlItem.Je,
                             InvoiceQty = noticeXmlItem.Sl,

                         };
            return result;
        }
    }

    public class InvoiceNoticeItemOriginView : UniqueView<Models.InvoiceNoticeItem, ScCustomsReponsitory>
    {
        public InvoiceNoticeItemOriginView()
        {

        }

        internal InvoiceNoticeItemOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {

            var result = from noticeItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                         select new Models.InvoiceNoticeItem
                         {
                             ID = noticeItem.ID,
                             InvoiceNoticeID = noticeItem.InvoiceNoticeID,
                             OrderID = noticeItem.OrderID,
                             OrderItemID = noticeItem.OrderItemID,
                             UnitPrice = noticeItem.UnitPrice,
                             Amount = noticeItem.Amount,
                             Difference = noticeItem.Difference,
                             InvoiceCode = noticeItem.InvoiceCode,
                             InvoiceNo = noticeItem.InvoiceNo,
                             InvoiceDate = noticeItem.InvoiceDate,
                             Status = (Enums.Status)noticeItem.Status,
                             CreateDate = noticeItem.CreateDate,
                             UpdateDate = noticeItem.UpdateDate,
                             Summary = noticeItem.Summary
                         };

            return result;
        }
    }
}
