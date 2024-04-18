using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Wl.CustomsTool.WinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Views
{
    public class DecOriginListsView : UniqueView<DecList, ScCustomsReponsitory>
    {
        public DecOriginListsView()
        { }

        internal DecOriginListsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecList> GetIQueryable()
        {
            var result = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         join BaseCountry in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>() on list.OriginCountry equals BaseCountry.Code into g
                         from b in g.DefaultIfEmpty()
                         select new Models.DecList
                         {
                             ID = list.ID,
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
                             OriginCountryName = b.Name,
                             GoodsSpec = list.GoodsSpec,
                             GoodsModel = list.GoodsModel,
                             GoodsBrand = list.GoodsBrand,
                             CaseNo = list.CaseNo,
                             NetWt = list.NetWt,
                             GrossWt = list.GrossWt,
                             Purpose = list.Purpose,
                             GoodsAttr = list.GoodsAttr,
                             OrigPlaceCode = list.OrigPlaceCode,
                             CiqName = list.CiqName,
                             GoodsBatch = list.GoodsBatch
                         };
            return result;
        }
    }
}
