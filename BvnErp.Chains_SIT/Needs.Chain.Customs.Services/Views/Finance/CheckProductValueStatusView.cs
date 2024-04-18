using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CheckProductValueStatusView
    {
        ScCustomsReponsitory _reponsitory { get; set; }

        public CheckProductValueStatusView()
        {
            this._reponsitory = new ScCustomsReponsitory();
        }

        public CheckProductValueStatusView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        /// <summary>
        /// 检查订单是否有正确状态的付汇审核
        /// </summary>
        /// <returns></returns>
        public bool CheckIsRightPayExchangeApply(string orderID)
        {
            var payExchangeApplies = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var payExchangeApplyItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

            var rightCount = (from payExchangeApply in payExchangeApplies
                              join payExchangeApplyItem in payExchangeApplyItems on payExchangeApply.ID equals payExchangeApplyItem.PayExchangeApplyID
                              where payExchangeApply.Status == (int)Enums.Status.Normal
                                 && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                                 && payExchangeApplyItem.OrderID == orderID
                                 && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Audited
                                  || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                                  || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                              select new
                              {
                                  PayExchangeApplyItemID = payExchangeApplyItem.ID,
                              }).Count();

            if (rightCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取订单跟单审核通过后的付汇申请的总金额
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public List<AmountModel> GetRightPayExchangeApplyItemsRMBAmount(string[] orderIDs)
        {
            var payExchangeApplies = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var payExchangeApplyItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

            var rightSum = (from payExchangeApply in payExchangeApplies
                            join payExchangeApplyItem in payExchangeApplyItems on payExchangeApply.ID equals payExchangeApplyItem.PayExchangeApplyID
                            where payExchangeApply.Status == (int)Enums.Status.Normal
                               && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                               && orderIDs.Contains(payExchangeApplyItem.OrderID)
                               && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Audited
                                || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                                || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                            group new { payExchangeApplyItem, payExchangeApply } by new { payExchangeApplyItem.OrderID, } into g
                            select new AmountModel
                            {
                                OrderID = g.Key.OrderID,
                                RMBAmount = g.Sum(t => t.payExchangeApplyItem.Amount * t.payExchangeApply.ExchangeRate),
                            }).ToList();

            return rightSum;
        }

        /// <summary>
        /// 获取订单已收货款的总金额
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <returns></returns>
        public List<AmountModel> GetReceivedProductFeeAmount(string[] orderIDs)
        {
            var orderReceipts = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var receivedProductFeeAmount = (from orderReceipt in orderReceipts
                                            where orderReceipt.Status == (int)Enums.Status.Normal
                                               && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                                               && orderReceipt.FeeType == (int)Enums.OrderFeeType.Product
                                               && orderIDs.Contains(orderReceipt.OrderID)
                                               && orderReceipt.Amount < 0
                                            group orderReceipt by new { orderReceipt.OrderID } into g
                                            select new AmountModel
                                            {
                                                OrderID = g.Key.OrderID,
                                                RMBAmount = g.Sum(t => 0 - t.Amount)
                                            }).ToList();

            return receivedProductFeeAmount;
        }

        public class AmountModel
        {
            public string OrderID { get; set; }

            public decimal RMBAmount { get; set; }

            public string PayExchangeApplyID { get; set; }
        }

        /// <summary>
        /// 货款应收-根据付汇申请
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <returns></returns>
        public List<AmountModel> GetRightPayExchangeApplyItems(string[] orderIDs)
        {
            var payExchangeApplies = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var payExchangeApplyItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

            var rightSum = (from payExchangeApply in payExchangeApplies
                            join payExchangeApplyItem in payExchangeApplyItems on payExchangeApply.ID equals payExchangeApplyItem.PayExchangeApplyID
                            where payExchangeApply.Status == (int)Enums.Status.Normal
                               && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                               && orderIDs.Contains(payExchangeApplyItem.OrderID)
                               && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Audited
                                || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                                || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                            group new { payExchangeApplyItem, payExchangeApply } by new { payExchangeApplyItem.ID, payExchangeApplyItem.OrderID, payExchangeApplyItem.PayExchangeApplyID } into g
                            select new AmountModel
                            {
                                //ID = g.Key.ID,
                                OrderID = g.Key.OrderID,
                                PayExchangeApplyID = g.Key.PayExchangeApplyID,
                                RMBAmount = g.Sum(t => t.payExchangeApplyItem.Amount * t.payExchangeApply.ExchangeRate),
                            }).ToList();

            return rightSum;
        }

    }

}
