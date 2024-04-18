using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OutputViewDownload : QueryView<Models.OutStoreViewModel, ScCustomsReponsitory>
    {

        public OutputViewDownload()
        {
        }

        protected OutputViewDownload(ScCustomsReponsitory reponsitory, IQueryable<Models.OutStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.OutStoreViewModel> GetIQueryable()
        {
            var iQuery = from outPut in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on outPut.InputID equals decList.InputID
                         join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decList.DeclarationID equals decHead.ID
                         join orderItemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on decList.OrderItemID equals orderItemCategory.OrderItemID
                         join decTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on decList.DeclarationID equals decTax.ID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decList.OrderID equals order.ID
                         join invoiceItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>() on decList.OrderItemID equals invoiceItem.OrderItemID into invoiceItems
                         from invoice in invoiceItems.DefaultIfEmpty()
                         select new Models.OutStoreViewModel
                         {
                             ID = decList.DecListID,  
                             OrderItemID = outPut.OrderItemID,
                             OutStoreDate = outPut.CreateDate,
                             OutQty = outPut.Quantity,
                             InvoiceDate = invoice == null?null:invoice.InvoiceDate,
                             GName = decList.GName,
                             GoodsBrand = decList.GoodsBrand,
                             GoodsModel = decList.GoodsModel,
                             GQty = decList.GQty,
                             Gunit = decList.GUnit,
                             TaxedPrice = decList.TaxedPrice,
                             TradeCurr = decList.TradeCurr,
                             OwnerName = decHead.OwnerName,
                             ContrNo = decHead.ContrNo,
                             TaxCode = orderItemCategory.TaxCode,
                             TaxName = orderItemCategory.TaxName,
                             OperatorID = outPut.AdminID,
                             DeclarationID = decHead.ID,
                             OrderID = decHead.OrderID,
                             InvoiceNo = invoice == null ? null : invoice.InvoiceNo,                             
                             InvoiceType = decTax.InvoiceType,
                             RealExchangeRate = order.RealExchangeRate,
                             DeclPrice = decList.DeclPrice,
                             DeclTotal = decList.DeclTotal,
                         };

            return iQuery;

        }

    }
}
