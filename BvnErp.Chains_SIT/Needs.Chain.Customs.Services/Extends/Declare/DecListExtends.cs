using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class DecListExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DecLists ToLinq(this Models.DecList entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DecLists
            {
                ID = entity.ID,
                DeclarationID = entity.DeclarationID,
                DeclarationNoticeItemID = entity.DeclarationNoticeItemID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                CusDecStatus = (int)entity.CusDecStatus,
                GNo = entity.GNo,
                CodeTS = entity.CodeTS,
                CiqCode = entity.CiqCode,
                GName = entity.GName,
                GModel = entity.GModel,
                GQty = entity.GQty,
                GUnit = entity.GUnit,
                FirstUnit = entity.FirstUnit,
                FirstQty = entity.FirstQty,
                SecondUnit = entity.SecondUnit,
                SecondQty = entity.SecondQty,
                DeclPrice = entity.DeclPrice,
                DeclTotal = entity.DeclTotal,
                TradeCurr = entity.TradeCurr,
                OriginCountry = entity.OriginCountry,
                DestinationCountry = entity.DestinationCountry,
                DestCode = entity.DestCode,
                DistrictCode = entity.DistrictCode,
                DutyMode = entity.DutyMode,
                GoodsSpec = entity.GoodsSpec,
                GoodsModel = entity.GoodsModel,
                GoodsBrand = entity.GoodsBrand,
                CaseNo = entity.CaseNo,
                NetWt = entity.NetWt,
                GrossWt = entity.GrossWt,
                Purpose = entity.Purpose,
                GoodsAttr = entity.GoodsAttr,
                ContrItem = entity.ContrItem,
                OrigPlaceCode = entity.OrigPlaceCode,
                CiqName = entity.CiqName,
                GoodsBatch = entity.GoodsBatch,
                DecListID = entity.DecListID,
                InputID = entity.InputID,
            };
        }
    }
}
