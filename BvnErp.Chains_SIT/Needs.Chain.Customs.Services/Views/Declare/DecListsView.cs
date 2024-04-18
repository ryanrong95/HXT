using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecListsView : UniqueView<Models.DecList, ScCustomsReponsitory>
    {
        public DecListsView()
        { }

        internal DecListsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecList> GetIQueryable()
        {
            var DeclarationNoticeItemView = new DeclarationNoticeItemsView(this.Reponsitory);

            var result = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         join item in DeclarationNoticeItemView on list.DeclarationNoticeItemID equals item.ID
                         select new Models.DecList
                         {
                             ID = list.ID,
                             DeclarationID = list.DeclarationID,
                             DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                             OrderID = list.OrderID,
                             OrderItemID = list.OrderItemID,
                             DeclarationNoticeItem = item,
                             GNo = list.GNo,
                             CodeTS = list.CodeTS,
                             CiqCode = list.CiqCode,
                             GName = list.GName,
                             GModel = list.GModel,
                             GQty = list.GQty,
                             GUnit = list.GUnit,
                             FirstUnit = list.FirstUnit,
                             FirstQty = list.FirstQty,
                             SecondUnit = list.SecondUnit,
                             SecondQty = list.SecondQty,
                             DeclPrice = list.DeclPrice,
                             DeclTotal = list.DeclTotal,
                             TradeCurr = list.TradeCurr,
                             OriginCountry = list.OriginCountry,
                             //DestinationCountry = list.DestinationCountry,
                             //DestCode = list.DestCode,
                             //DistrictCode = list.DistrictCode,
                             //DutyMode = list.DutyMode,
                             GoodsSpec = list.GoodsSpec,
                             GoodsModel = list.GoodsModel,
                             GoodsBrand = list.GoodsBrand,
                             CaseNo = list.CaseNo,
                             NetWt = list.NetWt,
                             GrossWt = list.GrossWt,
                             Purpose = list.Purpose,
                             GoodsAttr = list.GoodsAttr,
                             CiqName = list.CiqName,
                             GoodsBatch = list.GoodsBatch
                         };
            return result;
        }
    }


    public class DecOriginListsView : UniqueView<Models.DecList, ScCustomsReponsitory>
    {
        public DecOriginListsView()
        { }

        internal DecOriginListsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecList> GetIQueryable()
        {
            var BaseCountriesView = new BaseCountriesView(this.Reponsitory);

            var result = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         join OrderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on list.OrderItemID equals OrderItem.ID
                         join BaseCountry in BaseCountriesView on list.OriginCountry equals BaseCountry.Code into g
                         from b in g.DefaultIfEmpty()
                         select new Models.DecList
                         {
                             ID = list.ID,
                             DecListID = list.DecListID,
                             DeclarationID = list.DeclarationID,
                             DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                             OrderID = list.OrderID,
                             OrderItemID = list.OrderItemID,
                             ContrItem = list.ContrItem,
                             CusDecStatus = (CusItemDecStatus)list.CusDecStatus,
                             GNo = list.GNo,
                             CodeTS = list.CodeTS,
                             CiqCode = list.CiqCode,
                             GName = list.GName,
                             GModel = list.GModel.Replace("\"", "&#34"),
                             GQty = list.GQty,
                             GUnit = list.GUnit,
                             FirstUnit = list.FirstUnit,
                             FirstQty = list.FirstQty,
                             SecondUnit = list.SecondUnit,
                             SecondQty = list.SecondQty,
                             DeclPrice = list.DeclPrice,
                             DeclTotal = list.DeclTotal,
                             TradeCurr = list.TradeCurr,
                             OriginCountry = list.OriginCountry,
                             OriginCountryName = b.Name,
                             //DestinationCountry = list.DestinationCountry,
                             //DestCode = list.DestCode,
                             //DistrictCode = list.DistrictCode,
                             //DutyMode = list.DutyMode,
                             GoodsSpec = list.GoodsSpec,
                             GoodsModel = list.GoodsModel.Replace("\"", "&#34"),
                             GoodsBrand = list.GoodsBrand,
                             CaseNo = list.CaseNo,
                             NetWt = list.NetWt,
                             GrossWt = list.GrossWt,
                             Purpose = list.Purpose,
                             GoodsAttr = list.GoodsAttr,
                             OrigPlaceCode = list.OrigPlaceCode,
                             CiqName = list.CiqName,
                             GoodsBatch = list.GoodsBatch,

                             //  2020-09-03 by yeshuangshuang 
                             OrderPrice = OrderItem.UnitPrice,
                             OrderTotal = (OrderItem.UnitPrice * list.GQty).ToRound(2),
                             TaxedPrice = list.TaxedPrice
                         };
            return result;
        }
    }

    public class DecListsClassifyView : UniqueView<DecListsClassify, ScCustomsReponsitory>
    {
        public DecListsClassifyView()
        { }

        internal DecListsClassifyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecListsClassify> GetIQueryable()
        {
            var adminview = new AdminsTopView2(this.Reponsitory);
            var result = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on list.OrderItemID equals item.OrderItemID
                         join Tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TariffsTopView>() on list.CodeTS equals Tariff.HSCode
                         join user1 in adminview on item.ClassifyFirstOperator equals user1.OriginID into user1Admins
                         from user1 in user1Admins.DefaultIfEmpty()
                         join user2 in adminview on item.ClassifySecondOperator equals user2.OriginID into user2Admins
                         from user2 in user2Admins.DefaultIfEmpty()
                         select new DecListsClassify
                         {
                             GNo = list.GNo,
                             OrderID = list.OrderID,
                             HsCode = item.HSCode,
                             Name = item.Name,
                             Elements = item.Elements,
                             StandardElements = Tariff.DeclareElements,
                             Operate1 = user1.ByName,
                             Operate2 = user2.ByName
                         };
            return result;
        }
    }

    public class DecListsClassify:IUnique
    { 
        public string ID { get; set; }

        public string OrderID { get; set; }

        public int GNo { get; set; }

        public string Operate1 { get; set; }

        public string Operate1Time { get; set; }

        public string Operate2 { get; set; }

        public string Operate2Time { get; set; }

        public string HsCode { get; set; }

        public string Name { get; set; }

        public string Elements { get; set; }

        public string StandardElements { get; set; }

    }
}
