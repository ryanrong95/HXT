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
    public class InputViewSpeed : QueryView<Models.InStoreViewModel, ScCustomsReponsitory>
    {
        public InputViewSpeed()
        {
        }

        protected InputViewSpeed(ScCustomsReponsitory reponsitory, IQueryable<Models.InStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.InStoreViewModel> GetIQueryable()
        {                     
            var iQuery = from inView in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>()                        
                         select new Models.InStoreViewModel
                         {                                                                             
                             OperatorID = inView.AdminID,
                             InStoreDate = inView.CreateDate,
                             LotNumber = inView.LotNumber,
                             InputID = inView.InputID,
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IQueryable<Models.InStoreViewModel> iquery = this.IQueryable.Cast<Models.InStoreViewModel>().OrderByDescending(item => item.InStoreDate);
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
            var InputIDs = ienum_myDeclares.Select(item => item.InputID);
        
            var linq_decList = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                                where InputIDs.Contains(c.InputID)
                                select new {
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
                                    InputID = c.InputID
                                }).ToArray();

            var declarationIDs = linq_decList.Select(item => item.DeclarationID);
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
                                }).ToArray();

            var linq_decTaxFlows = (from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                                   where  declarationIDs.Contains(c.DecTaxID)
                                   select c).ToArray();
            //stopWatch2.Stop();
            //string s2 = stopWatch2.Elapsed.ToString();

            //Stopwatch stopWatch3 = new Stopwatch();
            //stopWatch3.Start();
            var baseUnit = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>();
            var adminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var ienums_linq = from decHead in linq_decHead 
                              join decList in linq_decList on decHead.ID equals decList.DeclarationID
                              join inputInfo in ienum_myDeclares on decList.InputID equals inputInfo.InputID
                              join _tariff in linq_decTaxFlows
                                  on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.Tariff }
                                  equals new { declarationID = _tariff.DecTaxID, taxType = _tariff.TaxType } into tariffs
                              from tariff in tariffs.DefaultIfEmpty()
                              join _ValueAddedTax in linq_decTaxFlows
                                   on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.AddedValueTax }
                                   equals new { declarationID = _ValueAddedTax.DecTaxID, taxType = _ValueAddedTax.TaxType } into ValueAddedTaxs
                              from ValueAddedTax in ValueAddedTaxs.DefaultIfEmpty()
                              join unit in baseUnit on decList.Gunit equals unit.Code
                              join admin in adminTopView on inputInfo.OperatorID equals admin.ID into admins
                              from admin in admins.DefaultIfEmpty()
                              select new Models.InStoreViewModel
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
                                  ContrNo = decHead.ContrNo,
                                  DDate = decHead.DDate,
                                  OwnerName = decHead.OwnerName,
                                  VoyNo = decHead.VoyNo,
                                  OrderItemID = decList.OrderItemID,
                                  DeclarationID = decList.DeclarationID,
                                  GunitName = unit.Name,                                 
                                  TariffTaxNumber = tariff == null ? "" : tariff.TaxNumber,
                                  ValueAddedTaxNumber = ValueAddedTax == null ? "" : ValueAddedTax.TaxNumber,
                                  OperatorID = inputInfo.OperatorID,
                                  InStoreDate = inputInfo.InStoreDate,
                                  LotNumber = inputInfo.LotNumber,
                                  OperatorName = admin == null ? "" : admin.RealName,
                              };

            //stopWatch3.Stop();
            //string s3 = stopWatch3.Elapsed.ToString();

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
            Func<Needs.Ccs.Services.Models.InStoreViewModel, object> convert = decList => new
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
                ContrNo = decList.ContrNo,
                DDate = decList.DDate == null ? "" : decList.DDate.Value.ToString("yyyy-MM-dd"),
                OwnerName = decList.OwnerName,
                VoyNo = decList.VoyNo,
                OrderItemID = decList.OrderItemID,
                DeclarationID = decList.DeclarationID,
                GunitName = decList.GunitName,
                TariffTaxNumber = decList.TariffTaxNumber,
                ValueAddedTaxNumber = decList.ValueAddedTaxNumber,
                PurchasingPrice = decList.PurchasingPrice,
                OperatorID = decList.OperatorID,
                InStoreDate = decList.InStoreDate,
                LotNumber = decList.LotNumber,
                OperatorName = decList.OperatorName,
                GoodsStatus = decList.GoodsStatus,
                InStoreDateShow = decList.InStoreDate == dtOld ? "" : decList.InStoreDate.Value.ToString("yyyy-MM-dd")
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public InputViewSpeed SearchByFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.InStoreDate >= fromtime
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.InStoreDate <= totime
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.OwnerName.Contains(clientName)
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByModel(string model)
        {
            var linq = from query in this.IQueryable
                       where query.GoodsModel == model
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByContrNo(string ContrNo)
        {
            var linq = from query in this.IQueryable
                       where query.ContrNo == ContrNo
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByVoyNo(string ContrNo)
        {
            var linq = from query in this.IQueryable
                       where query.VoyNo == ContrNo
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByDFrom(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate >= fromtime
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }

        public InputViewSpeed SearchByDTo(DateTime totime)
        {
            var linq = from query in this.IQueryable
                       where query.DDate <= totime
                       select query;

            var view = new InputViewSpeed(this.Reponsitory, linq);
            return view;
        }
    }
}
