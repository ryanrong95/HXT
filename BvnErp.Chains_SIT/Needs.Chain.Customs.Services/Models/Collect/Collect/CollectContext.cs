using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CollectContext
    {
        public List<CollectData> CheckData { get; set; }
        public CollectContext(List<CollectData> data)
        {
            this.CheckData = data;
        }

        public void Collect()
        {
            List<PayExchangeData> PayExchangeInfo = new List<PayExchangeData>();
            List<OrderReceiptData> OrderReceipts = new List<OrderReceiptData>();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                List<string> orderIds = this.CheckData.Select(t => t.OrderID).ToList();
                PayExchangeInfo = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                                       join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                                       on c.PayExchangeApplyID equals d.ID
                                       where c.Status == (int)Enums.Status.Normal && d.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled
                                       && c.IsCollected == null
                                       && orderIds.Contains(c.OrderID)
                                       select new PayExchangeData
                                       {
                                           OrderID = c.OrderID,
                                           PayExchangeID = d.ID,
                                           PayExchangeItemID = c.ID,
                                           Amount = c.Amount,
                                           ExchangeRate = d.ExchangeRate,
                                       }).ToList();

                OrderReceipts = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                     where orderIds.Contains(c.OrderID) && c.Type == (int)Enums.OrderReceiptType.Receivable
                                     && c.Status == (int)Enums.Status.Normal
                                     select new OrderReceiptData
                                     {
                                         OrderID = c.OrderID,
                                         FeeType = (Enums.FeeType)c.FeeType,
                                         Amount = c.Amount
                                     }).ToList();
            }

            var unCollected = this.CheckData.Where(t => t.ReceivedAmount == 0).ToList();
            var partCollected = this.CheckData.Where(t => t.ReceivedAmount > 0).ToList();

            Receivables receivables = new Receivables();
            receivables.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ReceivableCode);

            CheckReturnData unCollect = new UnCollected(unCollected, PayExchangeInfo, OrderReceipts, receivables.ID).Collect();
            CheckReturnData partCollect = new PartCollected(partCollected, PayExchangeInfo, receivables.ID).Collect();

            if (unCollect.Success && partCollect.Success)
            {
                receivables.Amount = this.CheckData.Sum(t => (t.PaidAmount - t.ReceivedAmount));
                receivables.Currency = this.CheckData.FirstOrDefault().Currency;
                receivables.ClientID = this.CheckData.FirstOrDefault().ClientID;
                receivables.CNYAmount = Convert.ToDecimal(unCollect.Data) + Convert.ToDecimal(partCollect.Data);
                receivables.Enter();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    foreach (var item in this.CheckData)
                    {
                        var CollectStatus = Enums.CollectStatusEnums.Collected;
                        if (item.TotalAmount != item.PaidAmount)
                        {
                            CollectStatus = Enums.CollectStatusEnums.PartCollected;
                        }
                        var Order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == item.OrderID).FirstOrDefault();
                        decimal collectedAmount = Order.CollectedAmount == null ? 0 : Order.CollectedAmount.Value + item.PaidAmount - item.ReceivedAmount;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { CollectStatus = (int)CollectStatus, CollectedAmount = collectedAmount }, t => t.ID == item.OrderID);
                    }
                    foreach(var item in PayExchangeInfo)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { IsCollected = (int)Enums.ItemCollectStatusEnums.Matched }, t => t.ID == item.PayExchangeItemID);
                    }
                }
            }
        }
    }
}
