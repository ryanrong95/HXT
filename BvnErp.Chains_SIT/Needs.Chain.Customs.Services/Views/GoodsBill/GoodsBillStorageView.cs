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
    public class GoodsBillStorageView : QueryView<Models.StoreViewModel, ScCustomsReponsitory>
    {
        public GoodsBillStorageView()
        {
        }

        protected GoodsBillStorageView(ScCustomsReponsitory reponsitory, IQueryable<Models.StoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.StoreViewModel> GetIQueryable()
        {          
            var iQuery = from decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on decHead.ID equals decList.DeclarationID                                                 
                         where decHead.IsSuccess == true 
                         select new Models.StoreViewModel
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
                             InStoreQty = 0,
                             OutStoreQty = 0, 
                             InputID = decList.InputID,
                         };
            return iQuery;
        }

        public object ToMyPage(string deadLineFrom,int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.StoreViewModel> iquery = this.IQueryable.Cast<Models.StoreViewModel>().OrderByDescending(item => item.DDate);
            int total = iquery.Count();

           
            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            var ienum_myDeclares = iquery.ToArray();
            //stopWatch.Stop();
            //string s1 = stopWatch.Elapsed.ToString();

            
            //获取申报的ID
            var declaresID = ienum_myDeclares.Select(item => item.OrderItemID);
            //获取申报的declarationID
            var declarationIDs = ienum_myDeclares.Select(item => item.DeclarationID);


            var linq_decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => declarationIDs.Contains(t.DecTaxID)).ToList();
            var baseUnit = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();
            
            //Stopwatch stopWatch2 = new Stopwatch();
            //stopWatch2.Start();
            //入库数据
            var inView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>().Where(t => declaresID.Contains(t.OrderItemID)).ToList();
            //stopWatch2.Stop();
            //string s2 = stopWatch2.Elapsed.ToString();

            //Stopwatch stopWatch3 = new Stopwatch();
            //stopWatch3.Start();
            //出库数据
            var outView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_OutView>().Where(t => declaresID.Contains(t.OrderItemID)).ToList();
            //截止时间
            if (!string.IsNullOrEmpty(deadLineFrom))
            {
                DateTime dtDead = Convert.ToDateTime(deadLineFrom);
                outView = outView.Where(t => t.CreateDate <= dtDead).ToList();
            }
            var outViewGroup = from a in outView
                               group a by a.OrderItemID into b
                               select new
                               {
                                   orderItemID = b.Key,
                                   qty = b.Sum(c => c.Quantity)
                               };
            //stopWatch3.Stop();
            //string s3 = stopWatch3.Elapsed.ToString();

            var ienums_linq = from decList in ienum_myDeclares
                              join ingoods in inView on decList.InputID equals ingoods.InputID                                   
                              join outgoods in outViewGroup on decList.OrderItemID equals outgoods.orderItemID into goods
                              from outgoods in goods.DefaultIfEmpty()
                              join _tariff in linq_decTaxFlows
                                  on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.Tariff }
                                  equals new { declarationID = _tariff.DecTaxID, taxType = _tariff.TaxType } into tariffs
                              from tariff in tariffs.DefaultIfEmpty()
                              join _ValueAddedTax in linq_decTaxFlows
                                   on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.AddedValueTax }
                                   equals new { declarationID = _ValueAddedTax.DecTaxID, taxType = _ValueAddedTax.TaxType } into ValueAddedTaxs
                              from ValueAddedTax in ValueAddedTaxs.DefaultIfEmpty()
                              join unit in baseUnit on decList.Gunit equals unit.Code
                              join admin in adminTopView on ingoods.AdminID equals admin.ID into admins
                              from admin in admins.DefaultIfEmpty()
                              select new Models.StoreViewModel
                              {
                                  ID = decList.ID,
                                  GName = decList.GName,
                                  GoodsBrand = decList.GoodsBrand,
                                  GoodsModel = decList.GoodsModel,
                                  InStoreQty = ingoods.Quantity,
                                  Gunit = decList.Gunit,
                                  DeclPrice = decList.DeclPrice,
                                  TaxedPrice = decList.TaxedPrice,
                                  TradeCurr = decList.TradeCurr,
                                  ContrNo = decList.ContrNo,
                                  DDate = decList.DDate,   
                                  OwnerName = decList.OwnerName,
                                  OrderItemID = decList.OrderItemID,
                                  DeclarationID = decList.DeclarationID,
                                  GunitName = unit.Name,
                                  TariffTaxNumber = tariff == null ? "" : tariff.TaxNumber,
                                  ValueAddedTaxNumber = ValueAddedTax == null ? "" : ValueAddedTax.TaxNumber,
                                  OperatorID = decList.OperatorID,
                                  InStoreDate = decList.DDate.Value,                               
                                  OperatorName = admin == null ? "" : admin.RealName,
                                  OutStoreQty = outgoods==null?0:outgoods.qty,
                                  GQty = decList.GQty
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
           
            Func<Needs.Ccs.Services.Models.StoreViewModel, object> convert = decList => new
            {
                ID = decList.ID,
                GName = decList.GName,
                GoodsBrand = decList.GoodsBrand,
                GoodsModel = decList.GoodsModel,
                InStoreQty = decList.InStoreQty,
                Gunit = decList.Gunit,
                DeclPrice = decList.DeclPrice,
                TaxedPrice = decList.TaxedPrice,
                TradeCurr = decList.TradeCurr,
                ContrNo = decList.ContrNo,                         
                OrderItemID = decList.OrderItemID,
                DeclarationID = decList.DeclarationID,
                GunitName = decList.GunitName,
                TariffTaxNumber = decList.TariffTaxNumber,
                ValueAddedTaxNumber = decList.ValueAddedTaxNumber,
                PurchasingPrice = decList.PurchasingPrice,
                OperatorID = decList.OperatorID,
                InStoreDate = decList.InStoreDate,               
                OperatorName = decList.OperatorName,               
                InStoreDateShow = decList.InStoreDate.ToString("yyyy-MM-dd"),
                OutStoreQty = decList.OutStoreQty,
                StockQty = decList.StockQty,
                OwnerName = decList.OwnerName,
            };

           

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };

           
        }

        public GoodsBillStorageView SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= fromtime
                       select query;

            var view = new GoodsBillStorageView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillStorageView SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate <= totime
                       select query;

            var view = new GoodsBillStorageView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillStorageView SearchByModel(string model)
        {
            var linq = from query in this.IQueryable
                       where query.GoodsModel == model
                       select query;

            var view = new GoodsBillStorageView(this.Reponsitory, linq);
            return view;
        }

        public GoodsBillStorageView SearchByClientName(string model)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName == model
                       select query;

            var view = new GoodsBillStorageView(this.Reponsitory, linq);
            return view;
        }
    }
}
