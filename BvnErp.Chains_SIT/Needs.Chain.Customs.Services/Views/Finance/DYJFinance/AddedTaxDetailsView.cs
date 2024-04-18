using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AddedTaxDetailsView : QueryView<object, ScCustomsReponsitory>
    {
        protected override IQueryable<object> GetIQueryable()
        {

           

            throw new NotImplementedException();
        }

        public IQueryable<Models.SZInput> GetAddedTaxListIDs(string InvoiceNo)
        {
            var linq = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                       join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on list.DeclarationID equals dechead.ID
                       //join order in this.Reponsitory.ReadTable<Orders>() on dechead.OrderID equals order.ID
                       //join clientagreement in this.Reponsitory.ReadTable<ClientAgreements>() on order.ClientAgreementID equals clientagreement.ID
                       where dechead.IsSuccess == true && dechead.ContrNo == InvoiceNo
                       orderby dechead.DDate descending
                       orderby dechead.ContrNo descending
                       select new Models.SZInput
                       {
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           //DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                           OrderID = list.OrderID,
                           OrderItemID = list.OrderItemID,
                           //DeclarationNoticeItem = list.DeclarationNoticeItem,
                           //GNo = list.GNo,
                           CodeTS = list.CodeTS,
                           //CiqCode = list.CiqCode,
                           GName = list.GName,
                           //GModel = list.GModel,
                           GQty = list.GQty,
                           GUnit = list.GUnit,
                           //FirstUnit = list.FirstUnit,
                           //FirstQty = list.FirstQty,
                           //SecondUnit = list.SecondUnit,
                           //SecondQty = list.SecondQty,
                           DeclPrice = list.DeclPrice,
                           DeclTotal = list.DeclTotal,
                           //TradeCurr = list.TradeCurr,
                           //OriginCountry = list.OriginCountry,
                           //GoodsSpec = list.GoodsSpec,
                           GoodsModel = list.GoodsModel,
                           GoodsBrand = list.GoodsBrand,
                           //CaseNo = list.CaseNo,
                           //NetWt = list.NetWt,
                           //GrossWt = list.GrossWt,
                           //Purpose = list.Purpose,
                           //GoodsAttr = list.GoodsAttr,

                           //海关汇率
                           CustomsRate = dechead.CustomsExchangeRate == null ? 0M : dechead.CustomsExchangeRate.Value,
                           //开票公司
                           //InvoiceType = (Enums.InvoiceType)clientagreement.InvoiceType,
                           //InvoiceCompany = dechead.OwnerName,
                           //ConsignorCode = dechead.ConsignorCode,
                           ContrNo = dechead.ContrNo,
                           //EntryId = dechead.EntryId,
                           //CusDecStatus = dechead.CusDecStatus,
                           //CreateDate = dechead.DDate,
                           //ClientID = order.ClientID,
                           //UserID = order.UserID,
                           //DecHeadType = dechead.Type,
                           DecHeadDDate = dechead.DDate,
                           //OrderCreateDate = order.CreateDate,

                       };

            return linq;
        }

        public List<Models.SZInput> GetDetailAddedTaxInfo(Models.SZInput[] results)
        {

            var OrderItemIds = results.Select(o => o.OrderItemID).ToArray();
            var OrderItemCategoryView = new OrderItemCategoriesView(this.Reponsitory).Where(t => OrderItemIds.Contains(t.OrderItemID));

            var orderId = results.Select(o => o.OrderID).FirstOrDefault();
            //var IcgooOrderMapView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => orderIds.Contains(t.OrderID)).ToArray();

            //var codes = results.Select(o => o.OriginCountry).ToArray();
            //var BaseCountriesView = new BaseCountriesView(this.Reponsitory).Where(t => codes.Contains(t.Code)).ToArray();

            //================================计算报关单的应交关税总价 Begin===============================================================================

            var decHeads = results.Select(o => new { ID = o.DeclarationID, o.CustomsRate }).Distinct().ToArray();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>().Where(t => decHeads.Select(dh => dh.ID).Contains(t.DeclarationID)).ToArray();
            var orderItemTaxes = new OrderItemTaxesView(this.Reponsitory).Where(t => decLists.Select(dl => dl.OrderItemID).Contains(t.OrderItemID)).ToArray();
            var orderitems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == orderId).ToArray();

            var results1 = (from entity in decLists
                            join decHead in decHeads on entity.DeclarationID equals decHead.ID
                            join importTax in orderItemTaxes on new { entity.OrderItemID, Type = Enums.CustomsRateType.ImportTax } equals new { importTax.OrderItemID, importTax.Type }
                            join addedValueTax in orderItemTaxes on new { entity.OrderItemID, Type = Enums.CustomsRateType.AddedValueTax } equals new { addedValueTax.OrderItemID, addedValueTax.Type }
                            join exciseTax in orderItemTaxes on new { entity.OrderItemID, Type = Enums.CustomsRateType.ConsumeTax } equals new { exciseTax.OrderItemID, exciseTax.Type } into exciseTaxes
                            from exciseTax in exciseTaxes.DefaultIfEmpty()
                            select new
                            {
                                DeclTotal = entity.DeclTotal,
                                DeclarationID = entity.DeclarationID,
                                CustomsRate = decHead.CustomsRate,
                                TariffRate = importTax.Rate,
                                ExciseTaxRate = exciseTax == null ? 0M : exciseTax.Rate,
                                Vat = addedValueTax.Rate,
                            }).ToArray();

            var results2 = from entity in results1
                           group entity by entity.DeclarationID into entities
                           select new
                           {
                               DeclarationID = entities.Key,
                               //TODO：此处算法存在误差
                               TotalTariff = entities.Sum(t => t.DeclTotal * t.CustomsRate * t.TariffRate),
                               TotalExciseTax = entities.Sum(t => t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) / (1 - t.ExciseTaxRate) * t.ExciseTaxRate),
                               TotalValueVat = entities.Sum(t => (t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) + t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) / (1 - t.ExciseTaxRate) * t.ExciseTaxRate) * t.Vat),
                           };

            //================================计算报关单的应交关税总价 End=================================================================================

            var linq = from list in results
                       join entity in results2 on list.DeclarationID equals entity.DeclarationID
                       //join BaseCountry in BaseCountriesView on list.OriginCountry equals BaseCountry.Code
                       join importTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.ImportTax } equals new { importTax.OrderItemID, importTax.Type }
                       join addedValueTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.AddedValueTax } equals new { addedValueTax.OrderItemID, addedValueTax.Type }
                       join exciseTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.ConsumeTax } equals new { exciseTax.OrderItemID, exciseTax.Type } into exciseTaxes
                       from exciseTax in exciseTaxes.DefaultIfEmpty()
                       join OrderItemCategory in OrderItemCategoryView on list.OrderItemID equals OrderItemCategory.OrderItemID
                       join orderitem in orderitems on list.OrderItemID equals orderitem.ID
                       //join icgooOrderMap in IcgooOrderMapView on list.OrderID equals icgooOrderMap.OrderID into icgooOrders
                       //from icgooOrder in icgooOrders.DefaultIfEmpty()
                       select new Models.SZInput
                       {
                           OrderID = list.OrderID,
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           //DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                           //DeclarationNoticeItem = list.DeclarationNoticeItem,
                           //GNo = list.GNo,
                           //CodeTS = list.CodeTS,
                           //CiqCode = list.CiqCode,
                           GName = list.GName,
                           //GModel = list.GModel,
                           GQty = list.GQty,
                           GUnit = list.GUnit,
                           //FirstUnit = list.FirstUnit,
                           //FirstQty = list.FirstQty,
                           //SecondUnit = list.SecondUnit,
                           //SecondQty = list.SecondQty,
                           DeclPrice = list.DeclPrice,
                           DeclTotal = list.DeclTotal,
                           //TradeCurr = list.TradeCurr,
                           //OriginCountry = list.OriginCountry,
                           //OriginCountryName = BaseCountry.Name,
                           //GoodsSpec = list.GoodsSpec,
                           GoodsModel = list.GoodsModel,
                           GoodsBrand = list.GoodsBrand,
                           //CaseNo = list.CaseNo,
                           //NetWt = list.NetWt,
                           //GrossWt = list.GrossWt,
                           //Purpose = list.Purpose,
                           //GoodsAttr = list.GoodsAttr,
                           //海关汇率
                           CustomsRate = list.CustomsRate,
                           //开票公司
                           //InvoiceType = list.InvoiceType,
                           //InvoiceCompany = list.InvoiceCompany,
                           //ConsignorCode = list.ConsignorCode,
                           ContrNo = list.ContrNo,
                           //EntryId = list.EntryId,
                           //CusDecStatus = list.CusDecStatus,
                           //CreateDate = list.CreateDate,
                           //关税率
                           TariffRate = importTax.Rate,
                           TariffRatePaid = importTax.ReceiptRate,
                           //消费税率
                           ExciseTaxRate = exciseTax == null ? 0M : exciseTax.Rate,
                           //增值税率
                           Vat = addedValueTax.Rate,
                           TaxName = OrderItemCategory.TaxName,
                           TaxCode = OrderItemCategory.TaxCode,
                           //报关单，关税总价
                           TotalTariff = entity.TotalTariff,
                           TotalExciseTax = entity.TotalExciseTax,
                           TotalValueVat = entity.TotalValueVat,
                           //是否外单
                           //IsExternalOrder = true,
                           //大赢家订单编号
                           DYJOrderID = orderitem.ProductUniqueCode,
                           //DecHeadType = list.DecHeadType,
                           DecHeadDDate = list.DecHeadDDate,
                           //OrderCreateDate = list.OrderCreateDate,
                       };

            return linq.ToList();

        }


        public bool CheckInvoiceNo(string InvoiceNo)
        {
            var linq = from dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on dechead.OrderID equals order.ID
                       where order.Type == (int)Enums.OrderType.Inside && dechead.IsSuccess
                       select dechead;

            return linq.Any(t => t.ContrNo == InvoiceNo);
        }

        /// <summary>
        /// 增值税缴纳流水
        /// </summary>
        /// <returns></returns>
        public IQueryable<Models.DecTaxFlow> GetTaxFolw()
        {

            var linq = from flow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                       where flow.TaxType == (int)Enums.DecTaxType.AddedValueTax
                       select new Models.DecTaxFlow
                       {
                           DecheadID = flow.DecTaxID,
                           TaxNumber = flow.TaxNumber,
                           PayDate = flow.PayDate,
                           Amount = flow.Amount
                       };

            return linq;
        }

        /// <summary>
        /// 增值税缴款书
        /// </summary>
        /// <returns></returns>
        public IQueryable<Models.DecHeadFile> GetAddedTaxFile()
        {

            var linq = from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                       where file.FileType == (int)Enums.FileType.DecHeadVatFile
                       select new Models.DecHeadFile
                       {
                           DecHeadID = file.DecHeadID,
                           Url = file.Url
                       };

            return linq;
        }
    }


}
