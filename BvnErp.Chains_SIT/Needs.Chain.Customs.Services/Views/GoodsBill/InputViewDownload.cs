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
    public class InputViewDownload : QueryView<Models.InStoreViewModel, ScCustomsReponsitory>
    {

        public InputViewDownload()
        {
        }

        protected InputViewDownload(ScCustomsReponsitory reponsitory, IQueryable<Models.InStoreViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.InStoreViewModel> GetIQueryable()
        {
            var iQuery = //from inView in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sz_Cfb_InView>()
                         from decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         //on inView.InputID equals decList.InputID                      
                         join decHead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                         on decList.DeclarationID equals decHead.ID
                         join _tariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                          on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.Tariff }
                                  equals new { declarationID = _tariff.DecTaxID, taxType = _tariff.TaxType } into tariffs
                         from tariff in tariffs.DefaultIfEmpty()
                         join _ValueAddedTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                                  on new { declarationID = decList.DeclarationID, taxType = (int)Enums.DecTaxType.AddedValueTax }
                                  equals new { declarationID = _ValueAddedTax.DecTaxID, taxType = _ValueAddedTax.TaxType } into ValueAddedTaxs
                         from ValueAddedTax in ValueAddedTaxs.DefaultIfEmpty()
                         select new Models.InStoreViewModel
                         {
                             DeclarationID = decHead.ID,
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
                             VoyNo = decHead.VoyNo,
                             //OperatorID = inView.AdminID,
                             //InStoreDate = inView.CreateDate,
                             //LotNumber = inView.LotNumber,
                             OperatorID = "",
                             InStoreDate = null,
                             LotNumber = "",
                             TariffTaxNumber = tariff == null ? "" : tariff.TaxNumber,
                             ValueAddedTaxNumber = ValueAddedTax == null ? "" : ValueAddedTax.TaxNumber,
                             InputID = decList.InputID,
                         };

            return iQuery;

        }

    }
}
