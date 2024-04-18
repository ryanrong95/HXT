using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ReceiveDetailReportViewNew : QueryView<ReceiveDetailReportViewNewModel, ScCustomsReponsitory>
    {
        public ReceiveDetailReportViewNew()
        {
        }

        protected ReceiveDetailReportViewNew(ScCustomsReponsitory reponsitory, IQueryable<ReceiveDetailReportViewNewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ReceiveDetailReportViewNewModel> GetIQueryable()
        {
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var iQuery = from orderReceipt in orderReceipts
                         select new ReceiveDetailReportViewNewModel
                         {
                             OrderID = orderReceipt.OrderID,
                             FinanceReceiptID = orderReceipt.FinanceReceiptID,
                             Amount = orderReceipt.Amount,
                             FeeType = (Enums.OrderFeeType)orderReceipt.FeeType,
                             Type = (Enums.OrderReceiptType)orderReceipt.Type,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ReceiveDetailReportViewNewModel> iquery = this.IQueryable.Cast<ReceiveDetailReportViewNewModel>(); //.OrderByDescending(item => item.OrderID);
            //int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_orderReceipts = iquery.ToArray();

            //订单ID
            var orderIDs = ienum_orderReceipts.Select(item => item.OrderID).Distinct();

            //(1)
            var fees = from c in ienum_orderReceipts
                       group c by c.OrderID into m
                       select new
                       {
                           OrderID = m.Key,
                           Tariff = m.Where(n => n.FeeType == Enums.OrderFeeType.Tariff).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.Tariff).Sum(n => n.Amount)),
                           ExciseTax = m.Where(n => n.FeeType == Enums.OrderFeeType.ExciseTax).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.ExciseTax).Sum(n => n.Amount)),
                           AddedValueTax = m.Where(n => n.FeeType == Enums.OrderFeeType.AddedValueTax).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.AddedValueTax).Sum(n => n.Amount)),
                           GoodsAmount = m.Where(n => n.FeeType == Enums.OrderFeeType.Product).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.Product).Sum(n => n.Amount)),
                           AgencyFee = m.Where(n => n.FeeType == Enums.OrderFeeType.AgencyFee).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.AgencyFee).Sum(n => n.Amount)),
                           Incidental = m.Where(n => n.FeeType == Enums.OrderFeeType.Incidental).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == Enums.OrderFeeType.Incidental).Sum(n => n.Amount)),
                       };

            int total = fees.Count();

            //(2)
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var orderInfo = from c in orders
                            join d in decHeads on c.ID equals d.OrderID
                            into e
                            from f in e.DefaultIfEmpty()
                            where orderIDs.Contains(c.ID)
                            select new
                            {
                                OrderID = c.ID,
                                RealExchangeRate = c.RealExchangeRate,
                                ContrNo = f.ContrNo,
                            };

            //(3)
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            var payExchangeInfo = from c in payExchangeApplyItems
                                  join d in payExchangeApplies on c.PayExchangeApplyID equals d.ID
                                  into e
                                  from f in e.DefaultIfEmpty()
                                  where orderIDs.Contains(c.OrderID) && f.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled
                                  select new
                                  {
                                      OrderID = c.OrderID,
                                      PaymentExchangeRate = f.ExchangeRate
                                  };

            var ienums_linq = from c in fees
                              join d in orderInfo on c.OrderID equals d.OrderID
                              join g in payExchangeInfo on c.OrderID equals g.OrderID into h
                              from k in h.DefaultIfEmpty()
                              select new ReceiveDetailReportViewNewModel
                              {
                                  OrderID = c.OrderID,
                                  ContrNo = d.ContrNo,
                                  AddedValueTax = c.AddedValueTax,
                                  ExciseTax = c.ExciseTax,
                                  Tariff = c.Tariff,
                                  GoodsAmount = c.GoodsAmount,
                                  AgencyFee = c.AgencyFee,
                                  Incidental = c.Incidental,
                                  PaymentExchangeRate = k != null ? (decimal?)k.PaymentExchangeRate : null,
                                  RealExchangeRate = d.RealExchangeRate,
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

            Func<ReceiveDetailReportViewNewModel, object> convert = item => new
            {
                OrderID = item.OrderID,
                ContrNo = item.ContrNo,
                AddedValueTax = item.AddedValueTax,
                ExciseTax = item.ExciseTax,
                Tariff = item.Tariff,
                ShowAgencyFee = item.ShowAgencyFee,
                GoodsAmount = item.GoodsAmount,
                PaymentExchangeRate = item.PaymentExchangeRate,
                FCAmount = item.FCAmount != null ? ((decimal)item.FCAmount).ToRound(2).ToString("0.00") : "",
                RealExchangeRate = item.RealExchangeRate,
                DueGoods = item.DueGoods != null ? ((decimal)item.DueGoods).ToRound(2).ToString("0.00") : "",
                Gains = item.Gains != null ? ((decimal)item.Gains).ToRound(2).ToString("0.00") : "",
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据 FinanceReceiptID 和 Type = OrderReceiptType.Received(实收) 查询
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>视图</returns>
        public ReceiveDetailReportViewNew SearchByFinanceReceiptIDAndReceived(string financeReceiptID)
        {
            var linq = from query in this.IQueryable
                       where query.FinanceReceiptID == financeReceiptID
                          && query.Type == Enums.OrderReceiptType.Received
                       select query;

            var view = new ReceiveDetailReportViewNew(this.Reponsitory, linq);
            return view;
        }

    }

    public class ReceiveDetailReportViewNewModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal? AddedValueTax { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public decimal? ExciseTax { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 代理费
        /// </summary>
        public decimal? AgencyFee { get; set; }

        /// <summary>
        /// 杂费
        /// </summary>
        public decimal? Incidental { get; set; }

        /// <summary>
        /// 货款
        /// </summary>
        public decimal? GoodsAmount { get; set; }

        /// <summary>
        /// 付款汇率
        /// </summary>
        public decimal? PaymentExchangeRate { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// FinanceReceiptID
        /// </summary>
        public string FinanceReceiptID { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// FeeType
        /// </summary>
        public Enums.OrderFeeType FeeType { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public Enums.OrderReceiptType Type { get; set; }

        //----------------------------------------------------

        /// <summary>
        /// 显示用得代理费
        /// </summary>
        public decimal? ShowAgencyFee
        {
            get
            {
                return this.AgencyFee + this.Incidental;
            }
        }

        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal? FCAmount
        {
            get
            {
                return this.GoodsAmount / this.PaymentExchangeRate;

                //if (this.PaymentExchangeRate != null)
                //{

                //}
                //else
                //{
                //    return null;
                //}                
            }
        }

        /// <summary>
        /// 应收账款-货款
        /// </summary>
        public decimal? DueGoods
        {
            get
            {
                return this.FCAmount * this.RealExchangeRate;
            }
        }

        /// <summary>
        /// 损益
        /// </summary>
        public decimal? Gains
        {
            get
            {
                return this.DueGoods - this.GoodsAmount;
            }
        }
    }

}
