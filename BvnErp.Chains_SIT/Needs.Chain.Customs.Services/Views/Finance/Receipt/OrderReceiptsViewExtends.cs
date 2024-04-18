using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 订单实收费用统计的视图
    /// </summary>
    public class OrderReceivedFeesView : UniqueView<Models.OrderReceivedFee, ScCustomsReponsitory>
    {
        public OrderReceivedFeesView()
        {

        }

        protected override IQueryable<OrderReceivedFee> GetIQueryable()
        {
            var receivedsAllsView = new OrderReceiptsAllsView(this.Reponsitory);
            var linq = from received in receivedsAllsView
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on received.OrderID equals order.ID
                       group received by new { received.OrderID, received.ClientID, order.OrderStatus, order.CreateDate } into g
                       select new Models.OrderReceivedFee
                       {
                           ID = g.Key.OrderID,
                           ClientID = g.Key.ClientID,
                           OrderStatus = (Enums.OrderStatus)g.Key.OrderStatus,
                           CreateDate = g.Key.CreateDate,
                           ProductFee = g.Where(item => item.FeeType == Enums.OrderFeeType.Product && item.Type == Enums.OrderReceiptType.Received).Sum(item => -item.Amount * item.Rate),
                           Tariff = g.Where(item => item.FeeType == Enums.OrderFeeType.Tariff && item.Type == Enums.OrderReceiptType.Received).Sum(item => -item.Amount * item.Rate),
                           AddedValueTax = g.Where(item => item.FeeType == Enums.OrderFeeType.AddedValueTax && item.Type == Enums.OrderReceiptType.Received).Sum(item => -item.Amount * item.Rate),
                           AgencyFee = g.Where(item => item.FeeType == Enums.OrderFeeType.AgencyFee && item.Type == Enums.OrderReceiptType.Received).Sum(item => -item.Amount * item.Rate),
                           IncidentalFee = g.Where(item => item.FeeType == Enums.OrderFeeType.Incidental && item.Type == Enums.OrderReceiptType.Received).Sum(item => -item.Amount * item.Rate),
                           IsCompleted = g.Sum(item => item.Amount * item.Rate) == 0
                       };

            return linq;
        }
    }

    /// <summary>
    /// 订单费用明细的视图（用于订单实收费用维护）
    /// </summary>
    public class OrderReceivedDetailsView : UniqueView<Models.OrderReceivedDetail, ScCustomsReponsitory>
    {
        public OrderReceivedDetailsView()
        {

        }

        protected override IQueryable<OrderReceivedDetail> GetIQueryable()
        {
            var receivedsAllsView = new OrderReceiptsAllsView(this.Reponsitory);
            var linq = from received in receivedsAllsView
                       group received by new { received.OrderID } into gOrder
                       select new Models.OrderReceivedDetail
                       {
                           ID = gOrder.Key.OrderID,

                           ReceivedFees = gOrder.Where(item => item.Type == Enums.OrderReceiptType.Received)
                           .Select(item => new Models.OrderFeeModel
                           {
                               FeeSourceID = item.FeeSourceID,
                               Type = item.FeeType,
                               Amount = -item.Amount * item.Rate,
                               ReceiptDate = item.CreateDate,
                               Ispaid = true
                           }).OrderBy(item => item.ReceiptDate),

                           UnReceiveFees = from unreceive in gOrder
                                           group unreceive by new { unreceive.FeeSourceID, unreceive.FeeType } into gFeeSource
                                           where gFeeSource.Sum(x => x.Amount) > 0
                                           select new Models.OrderFeeModel
                                           {
                                               FeeSourceID = gFeeSource.Key.FeeSourceID,
                                               Type = gFeeSource.Key.FeeType,
                                               Amount = gFeeSource.Key.FeeSourceID == null ?
                                                        gOrder.Where(item => item.FeeType == gFeeSource.Key.FeeType && item.FeeSourceID == null).Sum(item => item.Amount * item.Rate) :
                                                        gOrder.Where(item => item.FeeSourceID == gFeeSource.Key.FeeSourceID).Sum(item => item.Amount * item.Rate),
                                               Ispaid = false
                                           }
                       };

            return linq;
        }
    }

    #region 待收款中弹框使用

    /// <summary>
    /// 可收款的 OrderReceipt 视图
    /// </summary>
    public class ReceivableOrderReceiptView : UniqueView<Models.ReceivableOrderReceiptModel, ScCustomsReponsitory>
    {
        public ReceivableOrderReceiptView()
        {

        }

        protected override IQueryable<Models.ReceivableOrderReceiptModel> GetIQueryable()
        {
            var orderReceiptsAllsView = new OrderReceiptsAllsView(this.Reponsitory);

            var result = from orderResult in (from orderReceipt in orderReceiptsAllsView
                                              group orderReceipt by new { orderReceipt.OrderID, orderReceipt.FeeType, orderReceipt.FeeSourceID } into g
                                              select new Models.ReceivableOrderReceiptModel
                                              {
                                                  ID = g.Where(i => i.Type == Enums.OrderReceiptType.Receivable).FirstOrDefault().ID,
                                                  OrderID = g.Key.OrderID,
                                                  FeeType = g.Key.FeeType,
                                                  FeeSourceID = g.Key.FeeSourceID,

                                                  ReceivableAmount = g.Sum(item => item.Amount * item.Rate),
                                              })
                         select new Models.ReceivableOrderReceiptModel
                         {
                             ID = orderResult.ID,
                             OrderID = orderResult.OrderID,
                             FeeType = orderResult.FeeType,
                             FeeSourceID = orderResult.FeeSourceID,

                             ReceivableAmount = orderResult.ReceivableAmount < 0 ? 0 : orderResult.ReceivableAmount,
                         };

            return result;
        }

        /// <summary>
        /// 获取应收列表 货款按照付汇申请展示 --20201114 ryan
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public List<Models.ReceivableOrderReceiptModel> GetOrderReceiptForMerchandiser(string OrderID)
        {
            var results = this.GetIQueryable().Where(t => t.OrderID == OrderID).ToList();
            var resultProduct = results.Where(t => t.FeeType == Enums.OrderFeeType.Product).FirstOrDefault();

            //已经收了的货款，有付汇申请ID的
            var orderReceiptsAllsView = new OrderReceiptsAllsView(this.Reponsitory);
            var hadProduct = orderReceiptsAllsView.Where(t => t.OrderID == OrderID).ToList();

            var payexchange = (from payItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                               join pay in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on payItem.PayExchangeApplyID equals pay.ID
                               where payItem.OrderID == OrderID && pay.FatherID == null 
                               && payItem.Status == (int)Enums.Status.Normal
                               && pay.Status == (int)Enums.Status.Normal
                               && pay.PayExchangeApplyStatus >= (int)Enums.PayExchangeApplyStatus.Audited && pay.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled
                               select new
                               {
                                   OrderID = payItem.OrderID,
                                   payItem.PayExchangeApplyID,
                                   pay.ExchangeRate,
                                   payItem.Amount
                               }).ToList();

            //
            foreach (var pay in payexchange)
            {
                //货款实收
                var receivedProduct = hadProduct.Where(t => t.FeeSourceID == pay.PayExchangeApplyID).Sum(t => -t.Amount);

                //货款应收
                var hadAmount = (pay.ExchangeRate * pay.Amount).ToRound(2);

                if (results.Any(t=>t.FeeSourceID != pay.PayExchangeApplyID) || (results.Any(t => t.FeeSourceID == pay.PayExchangeApplyID) && receivedProduct < hadAmount))
                {
                    var r = new ReceivableOrderReceiptModel
                    {
                        ID = resultProduct.ID,
                        OrderID = pay.OrderID,
                        FeeType = resultProduct.FeeType,
                        FeeSourceID = pay.PayExchangeApplyID,

                        ReceivableAmount = receivedProduct <= 0 ? hadAmount : (hadAmount - receivedProduct),
                    };

                    results.Add(r);
                }

                
            }

            var res = (from rest in results.Where(t => t.FeeType == Enums.OrderFeeType.Product)
                       group rest by new { rest.FeeType, rest.FeeSourceID, rest.OrderID } into g
                       select new ReceivableOrderReceiptModel
                       {
                           //ID = g.Key.ID,
                           OrderID = g.Key.OrderID,
                           FeeType = g.Key.FeeType,
                           FeeSourceID = g.Key.FeeSourceID,

                           ReceivableAmount = g.Sum(t => t.ReceivableAmount)
                       }).ToList();

            results = results.Where(t => t.FeeType != Enums.OrderFeeType.Product).ToList();

            foreach (var r in res.Where(t=>t.FeeType == Enums.OrderFeeType.Product))
            {
                r.ID = resultProduct.ID;
                results.Add(r);
            }


            if (payexchange.Count == 0)
            {
                results.Where(t => t.FeeType == Enums.OrderFeeType.Product).FirstOrDefault().ReceivableAmount = 0;
            }
            else
            {
                results = results.Where(t => t.FeeType != Enums.OrderFeeType.Product || (t.FeeSourceID != null && t.FeeType == Enums.OrderFeeType.Product)).OrderBy(t => t.FeeType).ToList();
            }

            return results.OrderBy(t=>t.FeeType).ToList();
        }


        public List<Models.ReceivableOrderReceiptModel> GetForOneKeyReceipt(string[] orderIDs)
        {
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            //var adminsView2 = new Views.AdminsTopView2(this.Reponsitory);

            #region 关税、消费税、增值税小于 50 的应收

            var notNeedReceipt = from orderReceipt in orderReceipts
                                 where orderReceipt.Status == (int)Enums.Status.Normal
                                    && orderIDs.Contains(orderReceipt.OrderID)
                                    && orderReceipt.Type == (int)Enums.OrderReceiptType.Receivable
                                    && (orderReceipt.FeeType == (int)Enums.OrderFeeType.Tariff || orderReceipt.FeeType == (int)Enums.OrderFeeType.ExciseTax || orderReceipt.FeeType == (int)Enums.OrderFeeType.AddedValueTax)
                                    && (orderReceipt.Amount > 0 && orderReceipt.Amount < 50)
                                 select new
                                 {
                                     OrderReceiptID = orderReceipt.ID,
                                 };

            string[] notNeedReceiptIDs = notNeedReceipt.Select(t => t.OrderReceiptID).ToArray();

            #endregion

            var result = from orderResult in (from orderReceipt in orderReceipts
                                                  //join admin in adminsView2 on orderReceipt.AdminID equals admin.OriginID
                                              where orderReceipt.Status == (int)Enums.Status.Normal
                                                 && orderIDs.Contains(orderReceipt.OrderID)
                                                 && !notNeedReceiptIDs.Contains(orderReceipt.ID)
                                              group new { orderReceipt, } by new { orderReceipt.OrderID, orderReceipt.ClientID, orderReceipt.FeeType, orderReceipt.FeeSourceID, } into g
                                              select new Models.ReceivableOrderReceiptModel
                                              {
                                                  //Admin = g.FirstOrDefault().admin,
                                                  ID = g.Where(i => i.orderReceipt.Type == (int)Enums.OrderReceiptType.Receivable).FirstOrDefault().orderReceipt.ID,
                                                  OrderID = g.Key.OrderID,
                                                  ClientID = g.Key.ClientID,
                                                  FeeType = (Enums.OrderFeeType)g.Key.FeeType,
                                                  FeeSourceID = g.Key.FeeSourceID,

                                                  ReceivableAmount = g.Sum(item => item.orderReceipt.Amount * item.orderReceipt.Rate),
                                              })
                         select new Models.ReceivableOrderReceiptModel
                         {
                             //Admin = orderResult.Admin,
                             ID = orderResult.ID,
                             OrderID = orderResult.OrderID,
                             ClientID = orderResult.ClientID,
                             FeeType = orderResult.FeeType,
                             FeeSourceID = orderResult.FeeSourceID,

                             ReceivableAmount = orderResult.ReceivableAmount < 0 ? 0 : orderResult.ReceivableAmount,
                         };

            return result.ToList();
        }

    }

    #endregion
}
