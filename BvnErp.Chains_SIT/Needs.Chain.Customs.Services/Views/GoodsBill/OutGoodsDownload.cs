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
    public class OutGoodsDownload : QueryView<Models.OutStoreViewModel, ScCustomsReponsitory>
    {
        public OutGoodsDownload()
        {
        }

        protected OutGoodsDownload(ScCustomsReponsitory reponsitory, IQueryable<Models.OutStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.OutStoreViewModel> GetIQueryable()
        {
            DateTime dtOld = Convert.ToDateTime("1900-01-01");
            var iQuery = from outGoods in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OutGoods>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on outGoods.OrderItemID equals decList.OrderItemID
                         join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decList.DeclarationID equals decHead.ID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decList.OrderID equals order.ID
                         join orderitemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on outGoods.OrderItemID equals orderitemCategory.OrderItemID
                         join decTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on decList.DeclarationID equals decTax.ID
                         //join outView in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>() on decList.OrderItemID equals outView.OrderItemID into outStores
                         //from outView in outStores.DefaultIfEmpty()
                         join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>() on decList.OrderItemID equals invoice.OrderItemID into invoices
                         from invoice in invoices.DefaultIfEmpty()
                         select new Models.OutStoreViewModel
                         {
                             DeclarationID = decList.DeclarationID,
                             ID = decList.DecListID,
                             GName = decList.GName,
                             GoodsBrand = decList.GoodsBrand,
                             GoodsModel = decList.GoodsModel,
                             GQty = decList.GQty,
                             Gunit = decList.GUnit,
                             DeclPrice = decList.DeclPrice,
                             DeclTotal = decList.DeclTotal,
                             TradeCurr = decList.TradeCurr,
                             TaxedPrice = decList.TaxedPrice,
                             OrderItemID = decList.OrderItemID,
                             DDate = decHead.DDate,
                             OrderID = decHead.OrderID,
                             ContrNo = decHead.ContrNo,
                             OwnerName = decHead.OwnerName,
                             //OperatorID = outView == null ? "" : outView.AdminID,
                             //OutStoreDate = outView == null ? dtOld : outView.CreateDate,
                             //WaybillID = outView == null ? "" : outView.WaybillID,
                             OperatorID =  "",
                             OutStoreDate =  dtOld ,                          
                             ClientID = order.ClientID,
                             StorageDate = outGoods.StorageDate,
                             TaxName = orderitemCategory.TaxName,
                             TaxCode = orderitemCategory.TaxCode,
                             InvoiceNo = invoice == null ? "" : invoice.InvoiceNo,
                             InvoiceDate = invoice == null ? null : invoice.InvoiceDate,
                             InvoiceNoticeID = invoice == null ? "" : invoice.InvoiceNoticeID,
                             RealExchangeRate = order.RealExchangeRate,
                             InvoiceType = decTax.InvoiceType,  
                         };
            return iQuery;
        }
    }
}
