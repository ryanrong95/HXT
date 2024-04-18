using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
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
    public class ReceiveDetailReportView : UniqueView<Models.ReceiveDetail, ScCustomsReponsitory>
    {
        public ReceiveDetailReportView()
        {
        }

        internal ReceiveDetailReportView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ReceiveDetail> GetIQueryable()
        {
            throw new NotImplementedException();
        }



        public IQueryable<Models.ReceiveDetail> GetIQueryableResult(Expression<Func<ReceiveDetail, bool>> expression)
        {
            return null;
        }

        public IEnumerable<Models.ReceiveDetail> GetResult(out int totalCount, int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeList = GetList(pageIndex, pageSize, expressions).ToList();
            var count = GetCount(pageIndex, pageSize, expressions);            

            totalCount = count;

            return decNoticeList;
        }

        private IQueryable<Models.ReceiveDetail> GetList(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var decNoticeLists = GetCommon(pageIndex, pageSize, expressions);
            decNoticeLists = decNoticeLists.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return decNoticeLists;
        }

        private IQueryable<Models.ReceiveDetail> GetCommon(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var originalData = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                               select new ReceiveDetail
                               {
                                   OrderID = c.OrderID,
                                   FinanceReceiptID = c.FinanceReceiptID,
                                   Type = (OrderReceiptType)c.Type
                               };

            foreach (var expression in expressions)
            {
                originalData = originalData.Where(expression as Expression<Func<Needs.Ccs.Services.Models.ReceiveDetail, bool>>);
            }

            var OrderLists = originalData.Select(t => t.OrderID).Distinct().ToList();

            var fees = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                       where OrderLists.Contains(c.OrderID) && c.Type == (int)OrderReceiptType.Received
                       group c by c.OrderID into m                      
                       select new
                       {
                           OrderID = m.Key,
                           Tariff = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Count()==0?0: -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Sum(n=>n.Amount)),
                           AddedValueTax = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Count()==0?0: -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Sum(n => n.Amount)),
                           GoodsAmount = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Product).Count()==0?0: -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Product).Sum(n => n.Amount)),
                           AgencyFee = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Count()==0?0: -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Sum(n => n.Amount)),
                           Incidental = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Count()==0?0: -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Sum(n => n.Amount)),
                       };

            var orderInfo = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on c.ID equals d.OrderID
                            into e
                            from f in e.DefaultIfEmpty()
                            where OrderLists.Contains(c.ID)
                            select new
                            {
                                OrderID = c.ID,
                                RealExchangeRate = c.RealExchangeRate,
                                ContrNo = f.ContrNo,                              
                            };

            var payExchangeInfo = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                                  join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on c.PayExchangeApplyID equals d.ID
                                  into e
                                  from f in e.DefaultIfEmpty()
                                  where OrderLists.Contains(c.OrderID)
                                  select new
                                  {
                                      OrderID = c.OrderID,
                                      PaymentExchangeRate = f.ExchangeRate
                                  };


            var data = from c in fees
                       join d in orderInfo on c.OrderID equals d.OrderID                      
                       join g in payExchangeInfo on c.OrderID equals g.OrderID into h
                       from k in h.DefaultIfEmpty()
                       select new ReceiveDetail
                       {
                           OrderID = c.OrderID,
                           ContrNo = d.ContrNo,
                           AddedValueTax = c.AddedValueTax,
                           Tariff = c.Tariff,
                           GoodsAmount = c.GoodsAmount,
                           AgencyFee = c.AgencyFee,
                           Incidental = c.Incidental,
                           PaymentExchangeRate = k.PaymentExchangeRate,
                           RealExchangeRate = d.RealExchangeRate,
                       };

            return data;           
        }

        private int GetCount(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            return GetCommon(pageIndex, pageSize, expressions).Count();
        }
    }
}
