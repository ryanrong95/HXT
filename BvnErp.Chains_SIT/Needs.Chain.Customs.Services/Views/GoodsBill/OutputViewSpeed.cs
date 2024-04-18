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
    public class OutputViewSpeed : QueryView<Models.OutStoreViewModel, ScCustomsReponsitory>
    {
        public OutputViewSpeed()
        {
        }

        protected OutputViewSpeed(ScCustomsReponsitory reponsitory, IQueryable<Models.OutStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.OutStoreViewModel> GetIQueryable()
        {
           
            var iQuery = from outPut in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>()                        
                         select new Models.OutStoreViewModel
                         {
                             OperatorID = outPut.AdminID,
                             OrderItemID = outPut.OrderItemID,
                             OutStoreDate = outPut.CreateDate,
                             WaybillID = outPut.WaybillID,
                             InputID = outPut.InputID,                           
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IQueryable<Models.OutStoreViewModel> iquery = this.IQueryable.Cast<Models.OutStoreViewModel>().OrderByDescending(item => item.OutStoreDate);
            int total = iquery.Count();

            
            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }
           
           
            //获取数据
            var ienum_myDeclares = iquery.ToArray();
            stopWatch.Stop();
            string s1 = stopWatch.Elapsed.ToString();

            var InputIDs = ienum_myDeclares.Select(item => item.InputID);

            var linq_decList = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                                where InputIDs.Contains(c.InputID)
                                select new
                                {
                                    DeclarationID = c.DeclarationID,
                                    ID = c.DecListID,
                                    GName = c.GName,
                                    GoodsBrand = c.GoodsBrand,
                                    GoodsModel = c.GoodsModel,
                                    GQty = c.GQty,
                                    Gunit = c.GUnit,
                                    DeclPrice = c.DeclPrice,
                                    DeclTotal = c.DeclTotal,
                                    TradeCurr = c.TradeCurr,
                                    TaxedPrice = c.TaxedPrice,
                                    OrderItemID = c.OrderItemID,
                                    OrderID = c.OrderID,
                                    InputID = c.InputID
                                }).ToArray();

            //获取申报的ID
            var declaresID = linq_decList.Select(item => item.OrderItemID);
            //获取申报的declarationID
            var declarationIDs = linq_decList.Select(item => item.DeclarationID);
            //获取OrderID
            var orderIDs = linq_decList.Select(item => item.OrderID);

            var linq_InvoiceNoticeItems = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                                           where declaresID.Contains(c.OrderItemID)
                                           select new
                                           {
                                               OrderItemID = c.OrderItemID,
                                               InvoiceNo = c.InvoiceNo,
                                               InvoiceDate = c.InvoiceDate,
                                               InvoiceNoticeID = c.InvoiceNoticeID,
                                           }).ToArray();

            var baseUnit = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();
            var orderItemCategoryView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => declaresID.Contains(t.OrderItemID)).ToList();
           

            var orderInfos = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                             join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>()
                             on  c.ClientID  equals  clientAgreement.ClientID
                             where orderIDs.Contains(c.ID)&& clientAgreement.Status==(int)Enums.Status.Normal
                             select new
                             {
                                 OrderID = c.ID,
                                 RealExchangeRate = c.RealExchangeRate,
                                 InvoiceType = clientAgreement.InvoiceType
                             };


            var ienums_linq = from outStore in ienum_myDeclares
                              join decList in linq_decList on outStore.InputID equals decList.InputID
                              join category in orderItemCategoryView on decList.OrderItemID equals category.OrderItemID
                              join order in orderInfos on decList.OrderID equals order.OrderID
                              join unit in baseUnit on decList.Gunit equals unit.Code                                                 
                              join admin in adminTopView on outStore.OperatorID equals admin.ID into admins
                              from admin in admins.DefaultIfEmpty()
                              join invoiceInfo in linq_InvoiceNoticeItems on outStore.OrderItemID equals invoiceInfo.OrderItemID into invoices
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
                                  DeclTotal = decList.DeclTotal,
                                  TaxedPrice = decList.TaxedPrice,
                                  TradeCurr = decList.TradeCurr,                                                                 
                                  OwnerName = outStore.OwnerName,                                 
                                  OrderItemID = decList.OrderItemID,
                                  DeclarationID = decList.DeclarationID,
                                  GunitName = unit.Name,                                
                                  OperatorID = outStore.OperatorID,
                                  OutStoreDate = outStore.OutStoreDate,                                
                                  OperatorName = admin == null ? "" : admin.RealName,
                                  TaxName = category.TaxName,
                                  TaxCode = category.TaxCode,
                                  InvoiceNo = invoice == null ? "" : invoice.InvoiceNo,
                                  InvoiceNoticeID = invoice == null ? "" : invoice.InvoiceNoticeID,
                                  InvoiceDate = invoice == null ? null : invoice.InvoiceDate,
                                  RealExchangeRate = order.RealExchangeRate,
                                  InvoiceType = order.InvoiceType,
                                  WaybillID = outStore.WaybillID,
                                  
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

            DateTime dtOld = Convert.ToDateTime("1900-01-01");
            Func<Needs.Ccs.Services.Models.OutStoreViewModel, object> convert = decList => new
            {
                ID = decList.ID,
                OutStoreDate = decList.OutStoreDate.ToString("yyyy-MM-dd"),
                InvoiceDateShow = decList.InvoiceDate==null?"": decList.InvoiceDate.Value.ToString("yyyy-MM-dd"),
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
                OperatorName = decList.OperatorName,
                SalesPrice = decList.SalesPrice,
                InvoicePrice = decList.InvoicePrice,
                InvoiceNo = decList.InvoiceNo,
                InvoiceType = decList.InvoiceType,
                InvoiceDate = decList.InvoiceDate,
                WaybillID = decList.WaybillID,
                InvoiceNoticeID = decList.InvoiceNoticeID               
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public OutputViewSpeed SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.OutStoreDate >= fromtime
                       select query;

            var view = new OutputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public OutputViewSpeed SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.OutStoreDate <= totime
                       select query;

            var view = new OutputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public OutputViewSpeed SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(clientName)
                       select query;

            var view = new OutputViewSpeed(this.Reponsitory, linq);
            return view;
        }

  

        public OutputViewSpeed SearchByModel(string model)
        {
            var linq = from query in this.IQueryable
                       where query.GoodsModel == model
                       select query;

            var view = new OutputViewSpeed(this.Reponsitory, linq);
            return view;
        }
    }
}
