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
    public class OutGoodsDetailView : QueryView<Models.OutStoreViewModel, ScCustomsReponsitory>
    {
        public OutGoodsDetailView()
        {
        }

        protected OutGoodsDetailView(ScCustomsReponsitory reponsitory, IQueryable<Models.OutStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.OutStoreViewModel> GetIQueryable()
        {
           
            var iQuery = from outGoods in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OutGoods>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on outGoods.OrderItemID equals decList.OrderItemID
                         join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on decList.DeclarationID equals decHead.ID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decList.OrderID equals order.ID
                        
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
                             //OperatorID = outView.AdminID,
                             //OutStoreDate = outView.CreateDate,
                             //InvoiceDate = invoice == null ? dtOld : invoice.InvoiceDate,
                             //InvoiceNo = invoice == null ? "" : invoice.InvoiceNo,
                             ClientID = order.ClientID,
                             StorageDate = outGoods.StorageDate,
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.OutStoreViewModel> iquery = this.IQueryable.Cast<Models.OutStoreViewModel>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();


            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

           
            //获取数据
            var ienum_myDeclares = iquery.ToArray();
          
            //获取申报的ID
            var declaresID = ienum_myDeclares.Select(item => item.OrderItemID);
            //获取申报的declarationID
            var declarationIDs = ienum_myDeclares.Select(item => item.DeclarationID);
            //获取OrderID
            var orderIDs = ienum_myDeclares.Select(item => item.OrderID);

            var baseUnit = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();
            var orderItemCategoryView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => declaresID.Contains(t.OrderItemID)).ToList();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => orderIDs.Contains(t.ID)).ToList();
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => declarationIDs.Contains(t.ID)).ToList();
            var InvoiceInfo = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => declaresID.Contains(t.OrderItemID));
            var outStore = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>().Where(t => declaresID.Contains(t.OrderItemID)).ToList();


            DateTime dtOld = Convert.ToDateTime("1900-01-01");
            var ienums_linq = from decList in ienum_myDeclares
                              join category in orderItemCategoryView on decList.OrderItemID equals category.OrderItemID
                              join order in orders on decList.OrderID equals order.ID
                              join unit in baseUnit on decList.Gunit equals unit.Code
                              join decTax in decTaxs on decList.DeclarationID equals decTax.ID
                              join outView in outStore on decList.OrderItemID equals outView.OrderItemID into outStores
                              from outView in outStores.DefaultIfEmpty()                              
                              join invoice in InvoiceInfo on decList.OrderItemID equals invoice.OrderItemID into invoices
                              from invoice in invoices.DefaultIfEmpty()
                              select new Models.OutStoreViewModel
                              {
                                  ID = decList.ID,
                                  GName = decList.GName,
                                  GoodsBrand = decList.GoodsBrand,
                                  GoodsModel = decList.GoodsModel,
                                  GQty = decList.GQty,
                                  Gunit = decList.Gunit,
                                  DeclPrice = decList.DeclPrice,
                                  TaxedPrice = decList.TaxedPrice,
                                  TradeCurr = decList.TradeCurr,
                                  DDate = decList.DDate,
                                  OwnerName = decList.OwnerName,
                                  OrderItemID = decList.OrderItemID,
                                  DeclarationID = decList.DeclarationID,
                                  GunitName = unit.Name,
                                  OperatorID = outView == null?"": outView.AdminID,
                                  OutStoreDate = outView == null ? dtOld : outView.CreateDate,
                                  //OperatorName = admin == null ? "" : admin.RealName,
                                  TaxName = category.TaxName,
                                  TaxCode = category.TaxCode,
                                  InvoiceNo = invoice ==null?"": invoice.InvoiceNo,
                                  InvoiceDate = invoice == null ? dtOld : invoice.InvoiceDate,
                                  InvoiceNoticeID = invoice == null ? "" : invoice.InvoiceNoticeID,
                                  RealExchangeRate = order.RealExchangeRate,
                                  InvoiceType = decTax.InvoiceType,
                                  WaybillID = outView == null ? "" : outView.WaybillID,
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            
            Func<Needs.Ccs.Services.Models.OutStoreViewModel, object> convert = decList => new
            {
                ID = decList.ID,
                OutStoreDate = decList.OutStoreDate.ToString("yyyy-MM-dd"),
                InvoiceDateShow = decList.InvoiceDate == null ? "" : decList.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                GName = decList.GName,
                GoodsBrand = decList.GoodsBrand,
                GoodsModel = decList.GoodsModel,
                InvoiceQty = decList.GQty,
                GQty = decList.GQty,
                Gunit = decList.Gunit,
                GunitName = decList.GunitName,
                TradeCurr = decList.TradeCurr,
                TaxedPrice = decList.TaxedPrice,
                PurchasingPrice = decList.PurchasingPrice,
                OwnerName = decList.OwnerName,
                TaxName = decList.TaxName,
                TaxCode = decList.TaxCode,
                OperatorID = decList.OperatorID,
                OperatorName = "",
                SalesPrice = decList.SalesPrice,
                InvoicePrice = decList.InvoicePrice,
                InvoiceNo = decList.InvoiceNo,
                InvoiceType = decList.InvoiceType,
                WaybillID = decList.WaybillID,
                InvoiceNoticeID = decList.InvoiceNoticeID,
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public OutGoodsDetailView SearchByClientID(string clientID)
        {
            var linq = from query in this.IQueryable
                       where query.ClientID == clientID
                       select query;

            var view = new OutGoodsDetailView(this.Reponsitory, linq);
            return view;
        }

        public OutGoodsDetailView SearchByCurrent(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.StorageDate >= fromtime
                       select query;

            var view = new OutGoodsDetailView(this.Reponsitory, linq);
            return view;
        }

        public OutGoodsDetailView SearchByPrevious(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.StorageDate < fromtime
                       select query;

            var view = new OutGoodsDetailView(this.Reponsitory, linq);
            return view;
        }

        public OutGoodsDetailView SearchByOwnerName(String ownerName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(ownerName)
                       select query;

            var view = new OutGoodsDetailView(this.Reponsitory, linq);
            return view;
        }
    }
}
