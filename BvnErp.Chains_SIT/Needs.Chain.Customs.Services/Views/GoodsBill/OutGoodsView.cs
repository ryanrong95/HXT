using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OutGoodsView : QueryView<Models.OutGoodsSummary, ScCustomsReponsitory>
    {
        public OutGoodsView()
        {
        }

        protected OutGoodsView(ScCustomsReponsitory reponsitory, IQueryable<Models.OutGoodsSummary> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.OutGoodsSummary> GetIQueryable()
        {
            var iQuery = from outGoods in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OutGoods>()
                         join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on outGoods.OrderItemID equals decList.OrderItemID
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decList.OrderID equals order.ID
                         where outGoods.Status == (int)Enums.Status.Normal
                         select new Models.OutGoodsSummary
                         {
                             OrderItemID = outGoods.OrderItemID,
                             TaxedPrice = decList.TaxedPrice,
                             declTotal = decList.DeclTotal,
                             RealExchangeRate = order.RealExchangeRate,
                             ClientID = order.ClientID,
                             StorageDate = outGoods.StorageDate
                         };
            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var tempResult = from c in this.IQueryable
                             group c by c.ClientID into b
                             select new Models.OutGoodsSummary
                             {
                                 ClientID = b.Key,
                                 TaxedPrice = b.Sum(e => e.TaxedPrice),
                                 InvoicePrice = b.Sum(e => e.declTotal * e.RealExchangeRate)
                             };

            IQueryable<Models.OutGoodsSummary> iquery = tempResult.Cast<Models.OutGoodsSummary>().OrderByDescending(item => item.ClientID);
            int total = iquery.Count();

           
            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }
           

            //获取数据
            var ienum_myDeclares = iquery.ToArray();

            //获取申报的ID
            var clientIDs = ienum_myDeclares.Select(item => item.ClientID);

            var clientNames = (from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                              join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                              where clientIDs.Contains(client.ID) && client.Status == (int)Enums.Status.Normal
                              select new
                              {
                                  clientID = client.ID,
                                  clientName = company.Name
                              }).ToList();

            var ienums_linq = from decList in ienum_myDeclares
                              join clientName in clientNames on decList.ClientID equals clientName.clientID
                              select new Models.OutGoodsSummary
                              {
                                  ClientID = decList.ClientID,
                                  ClientName = clientName.clientName,
                                  TaxedPrice = decList.TaxedPrice,
                                  InvoicePrice = decList.InvoicePrice
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
           
            Func<Needs.Ccs.Services.Models.OutGoodsSummary, object> convert = decList => new
            {
                ClientID = decList.ClientID,
                ClientName = decList.ClientName,
                TaxedPrice = decList.TaxedPrice,
                InvoicePrice = Math.Round(decList.InvoicePrice.Value,4,MidpointRounding.AwayFromZero)
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        public OutGoodsView SearchByCurrent(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.StorageDate >= fromtime
                       select query;

            var view = new OutGoodsView(this.Reponsitory, linq);
            return view;
        }

        public OutGoodsView SearchByPrevious(DateTime fromtime)
        {
            var linq = from query in this.IQueryable
                       where query.StorageDate < fromtime
                       select query;

            var view = new OutGoodsView(this.Reponsitory, linq);
            return view;
        }
    }
}
