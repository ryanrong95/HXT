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
    public class GoodsBillMasterDataView : QueryView<Models.GoodsBillDetail, ScCustomsReponsitory>
    {
        public GoodsBillMasterDataView()
        {
        }

        protected GoodsBillMasterDataView(ScCustomsReponsitory reponsitory, IQueryable<Models.GoodsBillDetail> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.GoodsBillDetail> GetIQueryable()
        {
            var iQuery = from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on decHead.ID equals decList.DeclarationID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decHead.OrderID equals order.ID
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                         where decHead.IsSuccess == true
                         select new Models.GoodsBillDetail
                         {
                             DeclarationID = decHead.ID,
                             //ID = decList.DecListID,
                             GName = decList.GName,
                             //GoodsBrand = decList.GoodsBrand,
                             GoodsModel = decList.GoodsModel,
                             //GQty = decList.GQty,
                             //Gunit = decList.GUnit,
                             //DeclPrice = decList.DeclPrice,
                             //DeclTotal = decList.DeclTotal,
                             //TradeCurr = decList.TradeCurr,
                             //TaxedPrice = decList.TaxedPrice,
                             OrderItemID = decList.OrderItemID,
                             DDate = decHead.DDate,
                             //OrderID = decHead.OrderID,
                             ContrNo = decHead.ContrNo,
                             OwnerName = decHead.OwnerName,
                             //VoyNo = decHead.VoyNo,
                             //OperatorID = inView.AdminID,
                             //InStoreDate = inView.CreateDate,
                             //LotNumber = inView.LotNumber,
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IQueryable<Models.GoodsBillDetail> iquery = this.IQueryable.Cast<Models.GoodsBillDetail>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }


            //获取数据
            var ienum_myDeclares = iquery.ToArray();
            stopWatch.Stop();
            string s1 = stopWatch.Elapsed.ToString();

            //获取申报的ID
            var declaresID = ienum_myDeclares.Select(item => item.OrderItemID);
            //获取申报的declarationID
            var declarationIDs = ienum_myDeclares.Select(item => item.DeclarationID);


            //var InputInfoQuery = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>()
            //                     where declaresID.Contains(c.OrderItemID)
            //                     select c;
            //var linq_inputInfo = InputInfoQuery.ToArray();


            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();

            var linq_decHead = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                where declarationIDs.Contains(c.ID)
                                select new
                                {
                                    ID = c.ID,
                                    DDate = c.DDate,
                                    OrderID = c.OrderID,
                                    ContrNo = c.ContrNo,
                                    OwnerName = c.OwnerName,
                                    VoyNo = c.VoyNo,
                                    EntryId = c.EntryId,
                                }).ToArray();

            var linq_decList = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                                where declarationIDs.Contains(c.DeclarationID)
                                select new
                                {
                                    DeclarationID = c.DeclarationID,
                                    GNo = c.GNo,
                                    CodeTS = c.CodeTS,
                                    ID = c.DecListID,
                                    GName = c.GName,
                                    GoodsBrand = c.GoodsBrand,
                                    GoodsModel = c.GoodsModel,
                                    GModel = c.GModel,
                                    GQty = c.GQty,
                                    Gunit = c.GUnit,
                                    DeclPrice = c.DeclPrice,
                                    DeclTotal = c.DeclTotal,
                                    TradeCurr = c.TradeCurr,
                                    TaxedPrice = c.TaxedPrice,
                                    OrderItemID = c.OrderItemID,
                                    CaseNo = c.CaseNo,
                                    NetWt = c.NetWt,
                                    GrossWt = c.GrossWt,
                                    OriginCountry = c.OriginCountry,
                                    CiqCode = c.CiqCode,
                                }).ToArray();

            var linq_decTaxFlows = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                                    where declarationIDs.Contains(c.DecTaxID)
                                    select c).ToArray();

            var linq_OrderItemTaxes = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>()
                                       where declaresID.Contains(c.OrderItemID) && c.Type == (int)Enums.CustomsRateType.ImportTax
                                       select c).ToArray();

            var linq_OrderItemCategories = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                                            where declaresID.Contains(c.OrderItemID)
                                            select c).ToArray();


            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => declarationIDs.Contains(t.ID)).ToList();
            //var InvoiceInfo = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => declaresID.Contains(t.OrderItemID));
            var InvoiceInfo = (from invoiceItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                               join invoiceNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on invoiceItem.InvoiceNoticeID equals invoiceNotice.ID
                               where declaresID.Contains(invoiceItem.OrderItemID)
                               select new
                               {
                                   InvoiceDate = invoiceItem.InvoiceDate,
                                   Amount = invoiceItem.Amount,
                                   InvoiceNo = invoiceItem.InvoiceNo,
                                   InvoiceNoticeID = invoiceItem.InvoiceNoticeID,
                                   OrderItemID = invoiceItem.OrderItemID,
                                   InvoiceTaxRate = invoiceNotice.InvoiceTaxRate,
                               }).ToArray();
            stopWatch2.Stop();
            string s2 = stopWatch2.Elapsed.ToString();

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            var baseUnit = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var ienums_linq = from decHead in linq_decHead
                              join decList in linq_decList on decHead.ID equals decList.DeclarationID
                              //join inputInfo in linq_inputInfo on new { OrderItemID = decList.OrderItemID, Qty = decList.GQty }
                              //equals new { OrderItemID = inputInfo.OrderItemID, Qty = inputInfo.Quantity }
                              join _tariff in linq_decTaxFlows
                                  on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.Tariff }
                                  equals new { declarationID = _tariff.DecTaxID, taxType = _tariff.TaxType } into tariffs
                              from tariff in tariffs.DefaultIfEmpty()
                              join _ValueAddedTax in linq_decTaxFlows
                                   on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.AddedValueTax }
                                   equals new { declarationID = _ValueAddedTax.DecTaxID, taxType = _ValueAddedTax.TaxType } into ValueAddedTaxs
                              from ValueAddedTax in ValueAddedTaxs.DefaultIfEmpty()
                              join _ConsumptionTax in linq_decTaxFlows
                                  on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.ExciseTax }
                                  equals new { declarationID = _ConsumptionTax.DecTaxID, taxType = _ConsumptionTax.TaxType } into ConsumptionTaxs
                              from ConsumptionTax in ConsumptionTaxs.DefaultIfEmpty()
                              join tariffRate in linq_OrderItemTaxes on decList.OrderItemID equals tariffRate.OrderItemID
                              join OrderItemCategories in linq_OrderItemCategories on decList.OrderItemID equals OrderItemCategories.OrderItemID
                              join decTax in decTaxs on decHead.ID equals decTax.ID
                              join invoice in InvoiceInfo on decList.OrderItemID equals invoice.OrderItemID into invoices
                              from invoice in invoices.DefaultIfEmpty()
                              join unit in baseUnit on decList.Gunit equals unit.Code
                              //join admin in adminTopView on inputInfo.AdminID equals admin.ID into admins
                              //from admin in admins.DefaultIfEmpty()
                              select new Models.GoodsBillDetail
                              {
                                  ID = decList.ID,
                                  GNo = decList.GNo,
                                  CodeTS = decList.CodeTS,
                                  GName = decList.GName,
                                  GoodsBrand = decList.GoodsBrand,

                                  GoodsModel = decList.GoodsModel,
                                  GModel = decList.GModel,
                                  GQty = decList.GQty,
                                  GUnit = decList.Gunit,
                                  GunitName = unit.Name,
                                  DeclPrice = decList.DeclPrice,

                                  DeclTotal = decList.DeclTotal,
                                  TaxedPrice = decList.TaxedPrice,
                                  TradeCurr = decList.TradeCurr,
                                  CaseNo = decList.CaseNo,
                                  NetWt = decList.NetWt,

                                  GrossWt = decList.GrossWt,
                                  OriginCountry = decList.OriginCountry,
                                  ContrNo = decHead.ContrNo,
                                  EntryId = decHead.EntryId,
                                  DDate = decHead.DDate,

                                  tariffRate = tariffRate.ReceiptRate,
                                  OwnerName = decHead.OwnerName,
                                  CiqCode = decList.CiqCode,
                                  TaxName = OrderItemCategories.TaxName,
                                  TaxCode = OrderItemCategories.TaxCode,

                                  OrderID = decHead.OrderID,
                                  OperatorID = "",
                                  OperatorName = "系统机器人",
                                  InStoreDate = decHead.DDate,

                                  tariffTN = tariff == null ? "" : tariff.TaxNumber,
                                  tariffAmount = tariff == null ? 0 : tariff.Amount,
                                  DeductionMonth = tariff == null ? null : tariff.DeductionMonth,
                                  valueAddedTN = ValueAddedTax == null ? "" : ValueAddedTax.TaxNumber,
                                  valueAddedAmount = ValueAddedTax == null ? 0 : ValueAddedTax.Amount,
                                  ConsumptionTN = ConsumptionTax == null ? "" : ConsumptionTax.TaxNumber,
                                  ConsumptionAmount = ConsumptionTax == null ? 0 : ConsumptionTax.Amount,

                                  InvoiceDate = invoice==null?null:invoice.InvoiceDate,
                                  InvoiceAmount = invoice == null ? 0 : invoice.Amount,                                
                                  InvoiceNo = invoice == null ? "" : invoice.InvoiceNo,
                                  InvoiceType = decTax.InvoiceType,
                                  InvoiceNoticeID = invoice == null ? null : invoice.InvoiceNoticeID,
                                  WaybillCode = "",
                                  InvoiceTaxRate = invoice == null ? 0 : invoice.InvoiceTaxRate,

                                  VoyNo = decHead.VoyNo,
                                  OrderItemID = decList.OrderItemID,
                                  DeclarationID = decList.DeclarationID,                                  
                                  //LotNumber = inputInfo.LotNumber,
                                  LotNumber = "",

                              };

            stopWatch3.Stop();
            string s3 = stopWatch3.Elapsed.ToString();

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
            Func<Needs.Ccs.Services.Models.GoodsBillDetail, object> convert = decList => new
            {
                ID = decList.ID,
                GNo = decList.GNo,
                CodeTS = decList.CodeTS,
                GName = decList.GName,
                GoodsBrand = decList.GoodsBrand,

                GoodsModel = decList.GoodsModel,
                GModel = decList.GModel,
                GQty = decList.GQty,
                GUnit = decList.GUnit,
                GunitName = decList.GunitName,
                DeclPrice = decList.DeclPrice,

                DeclTotal = decList.DeclTotal,
                TaxedPrice = decList.TaxedPrice,
                TradeCurr = decList.TradeCurr,
                CaseNo = decList.CaseNo,
                NetWt = decList.NetWt,

                GrossWt = decList.GrossWt,
                OriginCountry = decList.OriginCountry,
                ContrNo = decList.ContrNo,
                EntryId = decList.EntryId,
                DDate = decList.DDate.Value.ToString("yyyy-MM-dd"),

                tariffRate = decList.tariffRate,
                OwnerName = decList.OwnerName,
                CiqCode = decList.CiqCode,
                TaxName = decList.TaxName,
                TaxCode = decList.TaxCode,

                OrderID = decList.OrderID,
                OperatorID = decList.AdminID,
                OperatorName = decList.OperatorName,
                InStoreDate = decList.DDate.Value.ToString("yyyy-MM-dd"),

                tariffTN = decList.tariffTN,
                tariffAmount = decList.tariffAmount,
                DeductionMonth = decList.DeductionMonth,
                valueAddedTN = decList.valueAddedTN,
                valueAddedAmount = decList.valueAddedAmount,
                ConsumptionTN = decList.ConsumptionTN,
                ConsumptionAmount = decList.ConsumptionAmount,

                InvoiceDate = decList.InvoiceDate==null?"":decList.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                InvoiceAmount = decList.InvoiceAmount,
                InvoiceTaxAmount = decList.InvoiceTaxAmount,
                InvoiceNo = decList.InvoiceNo,
                InvoiceType = decList.InvoiceType,
                WaybillCode = "",
                InvoiceNoticeID = decList.InvoiceNoticeID,

                VoyNo = decList.VoyNo,
                OrderItemID = decList.OrderItemID,
                DeclarationID = decList.DeclarationID,
                LotNumber = decList.LotNumber,
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public GoodsBillMasterDataView SearchByEntryID(string entryID)
        {
            var linq = from query in this.IQueryable
                       where query.EntryId == entryID
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByContrNo(string contrNo)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo == contrNo
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(clientName)
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByClientNo(string clientNo)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode == clientNo
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByModel(string model)
        {
            var linq = from query in this.IQueryable
                       where query.GoodsModel == model
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByGName(string gname)
        {
            var linq = from query in this.IQueryable
                       where query.GName == gname
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= fromtime
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillMasterDataView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate <= totime
                       select query;

            var view = new GoodsBillMasterDataView(this.Reponsitory, linq);
            return view;
        }
    }
}
