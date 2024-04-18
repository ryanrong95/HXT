using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单的视图
    /// </summary>
    public class OrderChangeView : Needs.Linq.Generic.Unique1Classics<Models.OrderChangeNotice, ScCustomsReponsitory>
    {
        public OrderChangeView()
        {

        }

        internal OrderChangeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderChangeNotice> GetIQueryable(Expression<Func<OrderChangeNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            var decHeadView = new Views.DecHeadsView(this.Reponsitory).Where(x => x.CusDecStatus != "04");

            var linq =
                   from changeNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>()
                       // join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on changeNotice.OderID equals order.ID
                   join decHead in decHeadView on changeNotice.OderID equals decHead.OrderID into decHeads
                   from decHead in decHeads.DefaultIfEmpty()
                   orderby changeNotice.CreateDate descending
                   select new OrderChangeNotice()
                   {
                       ID = changeNotice.ID,
                       DecHead = decHead,
                       //ClientID=order.ClientID,
                       OrderID = changeNotice.OderID,
                       Type = (Enums.OrderChangeType)changeNotice.Type,
                       processState = (Enums.ProcessState)changeNotice.ProcessState,
                       Status = (Enums.Status)changeNotice.Status,
                       CreateDate = changeNotice.CreateDate,
                       UpdateDate = changeNotice.UpdateDate,
                   };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.OrderChangeNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderChangeNotice> OnReadShips(OrderChangeNotice[] results)
        {
            var orderids = results.Select(item => item.OrderID).Distinct();
            var clients = new Views.ClientsView(this.Reponsitory);

            var companys = (from orderid in orderids
                    join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderid equals order.ID
                    join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                    join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                    select new
                    {
                        orderid = orderid,
                        adminID = order.AdminID,
                        clientID = order.ClientID,
                        companyname = company.Name,
                        clientcode = client.ClientCode,
                    }).ToArray();

            //var companys = (from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
            //                join client in clients on order.ClientID equals
            //                    client.ID
            //                where orderids.Contains(order.ID)
            //                select new
            //                {
            //                    orderid = order.ID,
            //                    adminID = order.AdminID,
            //                    clientID=order.ClientID,
            //                    companyname = client.Company.Name,
            //                    clientcode = client.ClientCode,
            //                }).ToArray();


            return from changeNotice in results
                   join company in companys on changeNotice.OrderID equals company.orderid
                  
                   select new OrderChangeNotice()
                   {
                       ID = changeNotice.ID,
                       // AdminID = company.adminID,
                       ClientID= company.clientID,
                       ClientCode = company.clientcode,
                       ClientName = company.companyname,
                       DecHead = changeNotice.DecHead,
                       OrderID = changeNotice.OrderID,
                       Type = changeNotice.Type,
                       processState = changeNotice.processState,
                       CreateDate = changeNotice.CreateDate,
                       UpdateDate = changeNotice.UpdateDate,
                   };
        }
    }


    /// <summary>
    /// 税费变更详情
    /// </summary>
    public class OrderChangeDetalView : UniqueView<OrderChangeDetail, ScCustomsReponsitory>
    {
        public OrderChangeDetalView()
        {
        }

        internal OrderChangeDetalView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.OrderChangeDetail> GetIQueryable()
        {

            var decHeadView = new Views.DecHeadsView(this.Reponsitory); ;
            var decListView = new Views.DecOriginListsView(this.Reponsitory);
            var flowView = new DecTaxFlowsView(this.Reponsitory);
            return from orderChange in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderChange.OderID equals order.ID
                   join dechead in decHeadView on orderChange.OderID equals dechead.OrderID
                   join decTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>() on dechead.ID equals decTax.ID
                   join addedValueflow in flowView.Where(x => x.TaxType == Enums.DecTaxType.AddedValueTax) on decTax.ID equals addedValueflow.DecheadID into addedValueflows
                   from addedValueflow in addedValueflows.DefaultIfEmpty()
                   join TariffFlow in flowView.Where(x => x.TaxType == Enums.DecTaxType.Tariff) on decTax.ID equals TariffFlow.DecheadID into tariffFlows
                   from TariffFlow in tariffFlows.DefaultIfEmpty()
                   join exciseTaxFlow in flowView.Where(x => x.TaxType == Enums.DecTaxType.ExciseTax) on decTax.ID equals exciseTaxFlow.DecheadID into exciseTaxFlows
                   from exciseTaxFlow in exciseTaxFlows.DefaultIfEmpty()
                   join declist in decListView on decTax.ID equals declist.DeclarationID into declists
                   select new OrderChangeDetail
                   {
                       OrderID = orderChange.OderID,
                       ID = dechead.ID,
                       ContrNo = dechead.ContrNo,
                       EntryId = dechead.EntryId,
                       DDate = dechead.DDate,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                       DecAmount = declists.Sum(x=>x.DeclTotal),//报关总价 取报关单中 各项报关总价的的和
                       DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                       OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                       AddedValue = addedValueflow.Amount,
                       TariffValue = TariffFlow.Amount,
                       ExciseTaxValue = exciseTaxFlow.Amount,
                       CreateDate = order.CreateDate
                   };


        }
    }

    /// <summary>
    /// 产品变更详情
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public class OrderItemChangeView : UniqueView<OrderItemChangeNotice, ScCustomsReponsitory>
    {

        public OrderItemChangeView()
        {
        }

        internal OrderItemChangeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<Models.OrderItemChangeNotice> GetIQueryable()
        {
            var adminView = new Views.AdminsTopView(this.Reponsitory);
            var orderItemView = new Views.OrderItemsView(this.Reponsitory);
            var linq =
                   from changeItemNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>()
                   join orderItem in orderItemView on changeItemNotice.OrderItemID equals orderItem.ID
                   join admin in adminView on changeItemNotice.AdminID equals admin.ID
                   select new OrderItemChangeNotice()
                   {
                       ID = changeItemNotice.ID,
                       Type = (Enums.OrderItemChangeType)changeItemNotice.Type,
                       OrderID = orderItem.OrderID,
                       ProductModel = orderItem.Model,
                       CreateDate = changeItemNotice.CreateDate,
                       UpdateDate = changeItemNotice.UpdateDate,
                       Sorter = admin,
                       IsSplited = changeItemNotice.IsSplited
                   };
            return linq;

        }


    }

    public class OrderChangeProductView : UniqueView<OrderChangeProduct, ScCustomsReponsitory>
    {
        public OrderChangeProductView()
        {
        }

        internal OrderChangeProductView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<Models.OrderChangeProduct> GetIQueryable()
        {
            var decListView = new Views.DecOriginListsView(this.Reponsitory);
            var taxesView = new OrderItemTaxesView(this.Reponsitory);
            var linq =
                   from decList in decListView
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decList.OrderID equals order.ID
                   join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on decList.OrderItemID equals orderItem.ID
                   join importTax in taxesView.Where(t => t.Type == Enums.CustomsRateType.ImportTax) on orderItem.ID equals importTax.OrderItemID into importTaxes
                   from importTax in importTaxes.DefaultIfEmpty()
                   join exciseTax in taxesView.Where(t => t.Type == Enums.CustomsRateType.ConsumeTax) on orderItem.ID equals exciseTax.OrderItemID into exciseTaxes
                   from exciseTax in exciseTaxes.DefaultIfEmpty()
                   join addedValueTax in taxesView.Where(t => t.Type == Enums.CustomsRateType.AddedValueTax) on orderItem.ID equals addedValueTax.OrderItemID into addedValueTaxes
                   from addedValueTax in addedValueTaxes.DefaultIfEmpty()
                   where decList.CusDecStatus !=Enums.CusItemDecStatus.Cancel
                   select new OrderChangeProduct()
                   {
                       ID = decList.ID,
                       OrderItemID=orderItem.ID,
                       OrderID=decList.OrderID,
                       HSCode = decList.CodeTS,
                       ProductName = decList.GName,
                       ProductModel = decList.GoodsModel,
                       Origin = orderItem.Origin,
                       TotalPrice = decList.DeclTotal,
                       ImportRate=importTax.Rate,
                       ExciseTaxRate = exciseTax == null ? 0M : exciseTax.Rate,
                       AddedValueRate=addedValueTax.Rate,
                       CustomsExchangeRate=order.CustomsExchangeRate,

                       //OrderImportTax = importTax.Value,
                       //OrderAddedValueTax = addedValueTax.Value,

                       ImportTax = importTax.Value,
                       ExciseTax = exciseTax == null ? 0M : exciseTax.Value,
                       AddedValueTax = addedValueTax.Value,
                       GNo =decList.GNo
                   };
            return linq;

        }


    }



}