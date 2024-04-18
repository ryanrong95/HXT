using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls.ScCustoms;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    public class SZInputView : UniqueView<Models.SZInput, ScCustomsReponsitory>
    {
        public SZInputView()
        { }

        internal SZInputView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZInput> GetIQueryable()
        {
            //var DecListView = new DecListsView(this.Reponsitory);
            var DecListView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            //报关单
            var DecHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            //订单的进口关税
            var ImportTaxView = new OrderItemTaxesView(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.ImportTax);
            //订单的增值税
            var AddedValueTaxView = new OrderItemTaxesView(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.AddedValueTax);
            //订单的消费税
            var ExciseTaxView = new OrderItemTaxesView(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.ConsumeTax);
            //产品项分类
            var OrderItemCategoryView = new OrderItemCategoriesView(this.Reponsitory);
            var BaseCountriesView = new BaseCountriesView(this.Reponsitory);

            var DecTaxFlowView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == (int)Enums.DecTaxType.AddedValueTax);

            var IcgooOrderMapView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();
            //报关单

            var result = from list in DecListView
                         join BaseCountry in BaseCountriesView on list.OriginCountry equals BaseCountry.Code
                         join DecHead in DecHeadsView on list.DeclarationID equals DecHead.ID
                         join ImportTax in ImportTaxView on list.OrderItemID equals ImportTax.OrderItemID
                         join AddedValueTax in AddedValueTaxView on list.OrderItemID equals AddedValueTax.OrderItemID
                         join ExciseTax in ExciseTaxView on list.OrderItemID equals ExciseTax.OrderItemID into exciseTaxes
                         from ExciseTax in exciseTaxes.DefaultIfEmpty()
                         join OrderItemCategory in OrderItemCategoryView on list.OrderItemID equals OrderItemCategory.OrderItemID
                         join icgooOrderMap in IcgooOrderMapView on list.OrderID equals icgooOrderMap.OrderID into icgooOrders
                         from icgooOrder in icgooOrders.DefaultIfEmpty()
                         join order in this.Reponsitory.ReadTable<Orders>() on list.OrderID equals order.ID
                         join clientagreement in this.Reponsitory.ReadTable<ClientAgreements>() on order.ClientAgreementID equals clientagreement.ID
                         // where (DecHead.CusDecStatus == "7" || DecHead.CusDecStatus == "B" || DecHead.CusDecStatus == "K" || DecHead.CusDecStatus == "G" || DecHead.CusDecStatus == "P" || DecHead.CusDecStatus == "R" || DecHead.CusDecStatus == "E1")
                         join client in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                         join company in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                         join dectaxflow in DecTaxFlowView on DecHead.ID equals dectaxflow.DecTaxID into tempDecTaxFlow
                         from dectaxflow in tempDecTaxFlow.DefaultIfEmpty()
                         where DecHead.IsSuccess == true
                         select new Models.SZInput
                         {
                             ID = list.ID,
                             DeclarationID = list.DeclarationID,
                             DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                             //DeclarationNoticeItem = list.DeclarationNoticeItem,
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
                             OriginCountryName = BaseCountry.Name,
                             GoodsSpec = list.GoodsSpec,
                             GoodsModel = list.GoodsModel,
                             GoodsBrand = list.GoodsBrand,
                             CaseNo = list.CaseNo,
                             NetWt = list.NetWt,
                             GrossWt = list.GrossWt,
                             Purpose = list.Purpose,
                             GoodsAttr = list.GoodsAttr,
                             //海关汇率
                             CustomsRate = DecHead.CustomsExchangeRate == null ? 0M : DecHead.CustomsExchangeRate.Value,
                             //开票公司
                             InvoiceType = (Enums.InvoiceType)clientagreement.InvoiceType,
                             //InvoiceCompany = DecHead.OwnerName,
                             InvoiceCompany = company.Name,
                             ConsignorCode = DecHead.ConsignorCode,
                             ContrNo = DecHead.ContrNo,
                             EntryId = DecHead.EntryId,
                             CusDecStatus = DecHead.CusDecStatus,
                             CreateDate = DecHead.DDate,
                             //关税率
                             TariffRate = ImportTax.Rate,
                             TariffRatePaid = ImportTax.ReceiptRate,
                             //消费税率
                             ExciseTaxRate = ExciseTax == null ? 0M : ExciseTax.Rate,
                             //增值税率
                             Vat = AddedValueTax.Rate,
                             TaxName = OrderItemCategory.TaxName,
                             TaxCode = OrderItemCategory.TaxCode,
                             //是否外单
                             IsExternalOrder = true,
                             //大赢家订单编号
                             DYJOrderID = icgooOrder.IcgooOrder,
                             ClientID = order.ClientID,
                             UserID = order.UserID,
                             OrderID=order.ID,
                             DecHeadType = DecHead.Type,
                             DecHeadDDate = DecHead.DDate,
                             FillinDate = dectaxflow == null ? null : dectaxflow.FillinDate
                         };
            //计算报关单的应交关税总价
            var result2 = from entity in result
                          group entity by entity.DeclarationID into entities
                          select new
                          {
                              DeclarationID = entities.Key,
                              //TODO：此处算法存在误差
                              TotalTariff = entities.Sum(t => t.DeclTotal * t.CustomsRate * t.TariffRate),
                              TotalExciseTax = entities.Sum(t => t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) / (1 - t.ExciseTaxRate) * t.ExciseTaxRate),
                              TotalValueVat = entities.Sum(t => (t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) + t.DeclTotal * t.CustomsRate * (1 + t.TariffRate) / (1 - t.ExciseTaxRate) * t.ExciseTaxRate) * t.Vat),
                          };

            var result3 = from list in result
                          join entity in result2 on list.DeclarationID equals entity.DeclarationID
                        
                          orderby list.CreateDate descending
                          orderby list.ContrNo descending
                          select new Models.SZInput
                          {
                              ID = list.ID,
                              DeclarationID = list.DeclarationID,
                              DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                              //DeclarationNoticeItem = list.DeclarationNoticeItem,
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
                              OriginCountryName = list.OriginCountryName,
                              GoodsSpec = list.GoodsSpec,
                              GoodsModel = list.GoodsModel,
                              GoodsBrand = list.GoodsBrand,
                              CaseNo = list.CaseNo,
                              NetWt = list.NetWt,
                              GrossWt = list.GrossWt,
                              Purpose = list.Purpose,
                              GoodsAttr = list.GoodsAttr,

                              //海关汇率
                              CustomsRate = list.CustomsRate,
                              //开票公司
                              InvoiceType = (Enums.InvoiceType)list.InvoiceType,
                              InvoiceCompany = list.InvoiceCompany,
                              ConsignorCode = list.ConsignorCode,
                              ContrNo = list.ContrNo,
                              EntryId = list.EntryId,
                              CusDecStatus = list.CusDecStatus,
                              IsExternalOrder = true,
                              CreateDate = list.CreateDate,
                              //关税率
                              TariffRate = list.TariffRate,
                              TariffRatePaid = list.TariffRatePaid,
                              //消费税率
                              ExciseTaxRate = list.ExciseTaxRate,
                              //增值税率
                              Vat = list.Vat,
                              TaxName = list.TaxName,
                              TaxCode = list.TaxCode,
                              //报关单，关税总价
                              TotalTariff = entity.TotalTariff,
                              TotalExciseTax = entity.TotalExciseTax,
                              TotalValueVat = entity.TotalValueVat,
                              DYJOrderID = list.DYJOrderID,
                              ClientID=list.ClientID,
                              UserID=list.UserID,
                              OrderID=list.OrderID,
                              DecHeadType = list.DecHeadType,
                              DecHeadDDate = list.DecHeadDDate,
                              FillinDate = list.FillinDate
                          };
            return result3;
        }
    }


    /// <summary>
    /// 代理订单1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SZInput1View : Needs.Linq.Generic.Query1Classics<Models.SZInput, ScCustomsReponsitory>
    {
        public SZInput1View()
        {
        }

        internal SZInput1View(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZInput> GetIQueryable(Expression<Func<Models.SZInput, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                       join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on list.DeclarationID equals dechead.ID
                       join order in this.Reponsitory.ReadTable<Orders>() on dechead.OrderID equals order.ID
                       join clientagreement  in this.Reponsitory.ReadTable<ClientAgreements>() on order.ClientAgreementID equals clientagreement.ID
                       where dechead.IsSuccess == true
                       orderby dechead.DDate descending
                       orderby dechead.ContrNo descending
                       select new Models.SZInput
                       {
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                           OrderID = list.OrderID,
                           OrderItemID = list.OrderItemID,
                           //DeclarationNoticeItem = list.DeclarationNoticeItem,
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
                           GoodsSpec = list.GoodsSpec,
                           GoodsModel = list.GoodsModel,
                           GoodsBrand = list.GoodsBrand,
                           CaseNo = list.CaseNo,
                           NetWt = list.NetWt,
                           GrossWt = list.GrossWt,
                           Purpose = list.Purpose,
                           GoodsAttr = list.GoodsAttr,

                           //海关汇率
                           CustomsRate = dechead.CustomsExchangeRate == null ? 0M : dechead.CustomsExchangeRate.Value,
                           //开票公司
                           InvoiceType = (Enums.InvoiceType)clientagreement.InvoiceType,
                           InvoiceCompany = dechead.OwnerName,
                           ConsignorCode = dechead.ConsignorCode,
                           ContrNo = dechead.ContrNo,
                           EntryId = dechead.EntryId,
                           CusDecStatus = dechead.CusDecStatus,
                           CreateDate = dechead.DDate,
                           ClientID=order.ClientID,
                           UserID=order.UserID,
                           DecHeadType = dechead.Type,
                           DecHeadDDate = dechead.DDate,
                           OrderCreateDate = order.CreateDate,

                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.SZInput, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<Models.SZInput> OnReadShips(Models.SZInput[] results)
        {
            var OrderItemIds = results.Select(o => o.OrderItemID).ToArray();
            var OrderItemCategoryView = new OrderItemCategoriesView(this.Reponsitory).Where(t => OrderItemIds.Contains(t.OrderItemID));

            var orderIds = results.Select(o => o.OrderID).Distinct().ToArray();
            var IcgooOrderMapView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => orderIds.Contains(t.OrderID)).ToArray();

            var codes = results.Select(o => o.OriginCountry).ToArray();
            var BaseCountriesView = new BaseCountriesView(this.Reponsitory).Where(t => codes.Contains(t.Code)).ToArray();

            var clientIDs = results.Select(o => o.ClientID).ToArray();
            var companyView = new ClientCompanyView(this.Reponsitory).Where(t => clientIDs.Contains(t.ID)).ToArray();

            //================================计算报关单的应交关税总价 Begin===============================================================================

            var decHeads = results.Select(o => new { ID = o.DeclarationID, o.CustomsRate }).Distinct().ToArray();
            var decTaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => decHeads.Select(dh => dh.ID).Contains(t.DecTaxID) && t.TaxType == (int)Enums.DecTaxType.AddedValueTax).ToArray();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>().Where(t => decHeads.Select(dh => dh.ID).Contains(t.DeclarationID)).ToArray();
            var orderItemTaxes = new OrderItemTaxesView(this.Reponsitory).Where(t => decLists.Select(dl => dl.OrderItemID).Contains(t.OrderItemID)).ToArray();

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
                       join BaseCountry in BaseCountriesView on list.OriginCountry equals BaseCountry.Code
                       join importTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.ImportTax } equals new { importTax.OrderItemID, importTax.Type }
                       join addedValueTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.AddedValueTax } equals new { addedValueTax.OrderItemID, addedValueTax.Type }
                       join exciseTax in orderItemTaxes on new { list.OrderItemID, Type = Enums.CustomsRateType.ConsumeTax } equals new { exciseTax.OrderItemID, exciseTax.Type } into exciseTaxes
                       from exciseTax in exciseTaxes.DefaultIfEmpty()
                       join OrderItemCategory in OrderItemCategoryView on list.OrderItemID equals OrderItemCategory.OrderItemID
                       join icgooOrderMap in IcgooOrderMapView on list.OrderID equals icgooOrderMap.OrderID into icgooOrders
                       from icgooOrder in icgooOrders.DefaultIfEmpty()
                       join client in companyView on list.ClientID equals client.ID
                       join dectaxflow in decTaxFlows on list.DeclarationID equals dectaxflow.DecTaxID into tempdectaxflow
                       from dectaxflow in tempdectaxflow.DefaultIfEmpty()
                       select new Models.SZInput
                       {
                           OrderID = list.OrderID,
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                           DeclarationNoticeItem = list.DeclarationNoticeItem,
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
                           OriginCountryName = BaseCountry.Name,
                           GoodsSpec = list.GoodsSpec,
                           GoodsModel = list.GoodsModel,
                           GoodsBrand = list.GoodsBrand,
                           CaseNo = list.CaseNo,
                           NetWt = list.NetWt,
                           GrossWt = list.GrossWt,
                           Purpose = list.Purpose,
                           GoodsAttr = list.GoodsAttr,
                           //海关汇率
                           CustomsRate = list.CustomsRate,
                           //开票公司
                           InvoiceType = list.InvoiceType,
                           //InvoiceCompany = list.InvoiceCompany,
                           InvoiceCompany = client.Company.Name,
                           ConsignorCode = list.ConsignorCode,
                           ContrNo = list.ContrNo,
                           EntryId = list.EntryId,
                           CusDecStatus = list.CusDecStatus,
                           CreateDate = list.CreateDate,
                           //关税率
                           TariffRate = importTax.Rate,
                           TariffRatePaid = importTax.ReceiptRate,
                           //消费税率
                           ExciseTaxRate = exciseTax == null? 0M : exciseTax.Rate,
                           //增值税率
                           Vat = addedValueTax.Rate,
                           TaxName = OrderItemCategory.TaxName,
                           TaxCode = OrderItemCategory.TaxCode,
                           //报关单，关税总价
                           TotalTariff = entity.TotalTariff,
                           TotalExciseTax = entity.TotalExciseTax,
                           TotalValueVat = entity.TotalValueVat,
                           //是否外单
                           IsExternalOrder = true,
                           //大赢家订单编号
                           DYJOrderID = icgooOrder?.IcgooOrder,
                           DecHeadType = list.DecHeadType,
                           DecHeadDDate = list.DecHeadDDate,
                           OrderCreateDate = list.OrderCreateDate,

                           //增值税缴款书填发日期 ryan 20220511 卢晓玲
                           FillinDate = dectaxflow == null ? null : dectaxflow.FillinDate
                       };

            return linq;
        }
    }
}
