using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DailyDeclareView : UniqueView<Models.DailyDeclare, ScCustomsReponsitory>
    {
        public DailyDeclareView()
        { }

        internal DailyDeclareView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
       
        protected override IQueryable<Models.DailyDeclare> GetIQueryable()
        {

            var result = from list in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         join country in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>() on list.OriginCountry equals country.Code into countries
                         from country in countries.DefaultIfEmpty()
                         join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on list.DeclarationID equals dechead.ID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on list.OrderID equals order.ID
                         join category in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on list.OrderItemID equals category.OrderItemID
                         join import in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Where(item => item.Type == (int)Enums.CustomsRateType.ImportTax) on list.OrderItemID equals import.OrderItemID
                         join icgooOrderMap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>() on order.ID equals icgooOrderMap.OrderID into icgooOrderMaps
                         from icgooOrderMap in icgooOrderMaps.DefaultIfEmpty()
                         select new DailyDeclare
                         {
                             ID = list.ID,
                             DeclarationID = list.DeclarationID,
                             DeclarationNoticeItemID = list.DeclarationNoticeItemID,
                             GNo = list.GNo,
                             CodeTS = list.CodeTS,
                             CiqCode = list.CiqCode,
                             GName = list.GName,
                             GModel = list.GModel,
                             GQty = list.GQty,
                             GUnit = list.GUnit,
                             DeclPrice = list.DeclPrice,
                             DeclTotal = list.DeclTotal,
                             TradeCurr = list.TradeCurr,
                             OriginCountry = list.OriginCountry,
                             OriginCountryName = country.Name,
                             GoodsModel = list.GoodsModel,
                             GoodsBrand = list.GoodsBrand,
                             CaseNo = list.CaseNo,
                             NetWt = list.NetWt,
                             GrossWt = list.GrossWt,

                             InvoiceCompany = dechead.OwnerName,
                             ContrNo = dechead.ContrNo,
                             DeclareDate = dechead.DDate,
                             EntryId = dechead.EntryId,
                             OrderID = dechead.OrderID,

                             TaxName = category.TaxName,
                             TaxCode = category.TaxCode,
                             TariffRate = import.ReceiptRate,

                             OrderType = (Enums.OrderType)order.Type,

                             GoodsAttr = list.GoodsAttr,
                             Purpose = list.Purpose,
                             IcgooOrderID = icgooOrderMap.IcgooOrder,
                             IsInspection = dechead.IsInspection,
                             IsQuarantine = dechead.IsQuarantine,
                             FirstQty = list.FirstQty,
                             FirstUnit = list.FirstUnit,
                             SecondQty = list.SecondQty,
                             SecondUnit = list.SecondUnit,
                         };

            return result;
        }
    }


    public class DailyDeclareView1 : Needs.Linq.Generic.Unique1Classics<Models.DailyDeclare, ScCustomsReponsitory>
    {
        public DailyDeclareView1()
        {

        }

        internal DailyDeclareView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DailyDeclare> GetIQueryable(Expression<Func<DailyDeclare, bool>> expression, params LambdaExpression[] expressions)
        {


            var DecOriginListView = new DecOriginListsView(this.Reponsitory);
            var DecHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decLicenseDocuView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>().Where(t=>t.DocuCode == "0");//0：反制措施排除代码

            //订单的进口关税
            var ImportTaxView = new OrderItemTaxesView(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.ImportTax);
            //产品项分类
            var OrderItemCategoryView = new OrderItemCategoriesView(this.Reponsitory);


            var linq = from list in DecOriginListView
                       join DecHead in DecHeadsView on list.DeclarationID equals DecHead.ID
                       join ImportTax in ImportTaxView on list.OrderItemID equals ImportTax.OrderItemID
                       join OrderItemCategory in OrderItemCategoryView on list.OrderItemID equals OrderItemCategory.OrderItemID
                       join icgooOrderMap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>() on DecHead.OrderID equals icgooOrderMap.OrderID into icgooOrderMaps
                       from icgooOrderMap in icgooOrderMaps.DefaultIfEmpty()
                       join docu in decLicenseDocuView on DecHead.ID equals docu.DeclarationID into t_docu
                       from docu in t_docu.DefaultIfEmpty()
                       orderby DecHead.DDate descending
                       select new Models.DailyDeclare
                       {
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           //DeclarationNoticeItemID = list.DeclarationNoticeItemID,
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
                           GoodsAttr = list.GoodsAttrName,
                           CaseNo = list.CaseNo,
                           NetWt = list.NetWt,
                           GrossWt = list.GrossWt,
                           Purpose = list.PurposeName,
                           // GoodsAttr = list.GoodsAttr,
                           OrderItemID = list.OrderItemID,
                           TaxName = OrderItemCategory.TaxName,
                           TaxCode = OrderItemCategory.TaxCode,
                           InvoiceCompany = DecHead.OwnerName,//开票公司
                           ContrNo = DecHead.ContrNo,
                           DeclareDate = DecHead.DDate,
                           EntryId = DecHead.EntryId,
                           VoyNo = DecHead.VoyNo,
                           CusDecStatus = list.CusDecStatus,
                           ProductName = list.GName,
                           TotalAmount = list.DeclTotal,
                           OrderID = list.OrderID,
                           //Order = new Models.Order { ID = order.ID, Client = client, Type = (Enums.OrderType)order.Type },
                           //关税率
                           TariffRate = ImportTax.Rate,
                           // 海关汇率
                           // CustomsRate = DecHead.CustomsExchangeRate.Value,
                           CustomsRate = DecHead.CustomsExchangeRate == null ? 0M : DecHead.CustomsExchangeRate.Value,
                           Tariff = ImportTax.Value.Value,
                           IcgooOrderID = icgooOrderMap.IcgooOrder,
                           CertCode0 = docu.CertCode
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.DailyDeclare, bool>>);
            }

            return linq;
        }

        protected override IEnumerable<DailyDeclare> OnReadShips(DailyDeclare[] results)
        {
            var orderids = results.Select(item => item.OrderID).Distinct().ToArray();
            var clients = new Views.ClientsView(this.Reponsitory);
            var companys = (from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            join client in clients on order.ClientID equals
                                client.ID
                            where orderids.Contains(order.ID)
                            select new
                            {
                                orderid = order.ID,
                                OrderType = order.Type,
                                companyname = client.Company.Name,
                                clientcode = client.ClientCode,
                            }).ToArray();


            // var clientView = new ClientsView(this.Reponsitory);

            return from list in results

                   join company in companys on list.OrderID equals company.orderid
                   select new DailyDeclare()
                   {
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
                       OriginCountryName = list.OriginCountryName,
                       GoodsSpec = list.GoodsSpec,
                       GoodsModel = list.GoodsModel,
                       GoodsBrand = list.GoodsBrand,
                       CaseNo = list.CaseNo,
                       NetWt = list.NetWt,
                       GrossWt = list.GrossWt,
                       Purpose = list.Purpose,
                       GoodsAttr = list.GoodsAttr,
                       TaxName = list.TaxName,
                       TaxCode = list.TaxCode,
                       InvoiceCompany = list.InvoiceCompany,//开票公司
                       ContrNo = list.ContrNo,
                       DeclareDate = list.DeclareDate,
                       EntryId = list.EntryId,
                       ProductName = list.GName,
                       TotalAmount = list.DeclTotal,
                       OrderType = (Enums.OrderType)company.OrderType,
                       //关税率
                       TariffRate = list.TariffRate,
                       //海关汇率
                       CustomsRate = list.CustomsRate,
                       Tariff = list.Tariff,
                       ClientCode = list.ClientCode,
                       ClientName = list.ClientName,
                       OrderID = list.OrderID,
                       CusDecStatus = list.CusDecStatus,
                       IcgooOrderID = list.IcgooOrderID,
                       CertCode0 = list.CertCode0
                   };
        }
    }

    public class DailyDeclareView2 : UniqueView<Models.DailyDeclare, ScCustomsReponsitory>
    {
        public DailyDeclareView2()
        {

        }

        internal DailyDeclareView2(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DailyDeclare> GetIQueryable()
        {


            var DecOriginListView = new DecOriginListsView(this.Reponsitory);
            var DecHeadsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var decLicenseDocuView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>().Where(t => t.DocuCode == "0");//0：反制措施排除代码

            //订单的进口关税
            var ImportTaxView = new OrderItemTaxesView(this.Reponsitory).Where(item => item.Type == Enums.CustomsRateType.ImportTax);
            //产品项分类
            var OrderItemCategoryView = new OrderItemCategoriesView(this.Reponsitory);


            var linq = from list in DecOriginListView
                       join DecHead in DecHeadsView on list.DeclarationID equals DecHead.ID
                       join ImportTax in ImportTaxView on list.OrderItemID equals ImportTax.OrderItemID
                       join OrderItemCategory in OrderItemCategoryView on list.OrderItemID equals OrderItemCategory.OrderItemID
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on list.OrderID equals order.ID
                       join icgooOrderMap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>() on DecHead.OrderID equals icgooOrderMap.OrderID into icgooOrderMaps
                       from icgooOrderMap in icgooOrderMaps.DefaultIfEmpty()
                       join docu in decLicenseDocuView on DecHead.ID equals docu.DeclarationID into t_docu
                       from docu in t_docu.DefaultIfEmpty()
                       orderby DecHead.DDate descending
                       select new Models.DailyDeclare
                       {
                           ID = list.ID,
                           DeclarationID = list.DeclarationID,
                           DeclarationNoticeItemID = list.DeclarationNoticeItemID,                           
                           GNo = list.GNo,
                           CodeTS = list.CodeTS,
                           CiqCode = list.CiqCode,
                           GName = list.GName,
                           GModel = list.GModel,
                           GQty = list.GQty,
                           GUnit = list.GUnit,
                           DeclPrice = list.DeclPrice,                                                     
                           DeclTotal = list.DeclTotal,
                           TradeCurr = list.TradeCurr,
                           OriginCountry = list.OriginCountry,
                           OriginCountryName = list.OriginCountryName,                           
                           GoodsModel = list.GoodsModel,
                           GoodsBrand = list.GoodsBrand,
                           CaseNo = list.CaseNo,                                                   
                           NetWt = list.NetWt,
                           GrossWt = list.GrossWt,
                           InvoiceCompany = DecHead.OwnerName,//开票公司                        
                           
                           ContrNo = DecHead.ContrNo,
                           DeclareDate = DecHead.DDate,
                           EntryId = DecHead.EntryId,
                           VoyNo = DecHead.VoyNo,
                           OrderID = list.OrderID,

                           TaxName = OrderItemCategory.TaxName,
                           TaxCode = OrderItemCategory.TaxCode,
                           TariffRate = ImportTax.ReceiptRate,

                           OrderType = (Enums.OrderType)order.Type,

                           GoodsAttr = list.GoodsAttr,
                           Purpose = list.Purpose,
                           IcgooOrderID = icgooOrderMap.IcgooOrder,
                           IsInspection = DecHead.IsInspection,
                           IsQuarantine = DecHead.IsQuarantine,

                           FirstUnit = list.FirstUnit,
                           FirstQty = list.FirstQty,
                           SecondUnit = list.SecondUnit,
                           SecondQty = list.SecondQty,
                           CertCode0 = docu.CertCode
                       };


            return linq;
        }
       
    }
}
