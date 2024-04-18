using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class UnCollected : ICollect
    {
        public string ReceivableID { get; set; }
        public List<CollectData> CheckData { get; set; }
        public List<PayExchangeData> PayExchangeInfo { get; set; }
        public List<OrderReceiptData> OrderReceipts { get; set; }
        public UnCollected(List<CollectData> data, List<PayExchangeData> payExchangeInfo, List<OrderReceiptData> orderReceipts,string receivableID)
        {
            this.CheckData = data;
            this.PayExchangeInfo = payExchangeInfo;
            this.OrderReceipts = orderReceipts;
            this.ReceivableID = receivableID;
        }
        public CheckReturnData Collect()
        {
            CheckReturnData data = new CheckReturnData();
            decimal totalPrice = 0, totalCNYPrice = 0, goodsFee=0;
           
            List<string> orderIds = this.CheckData.Select(t => t.OrderID).ToList();

            try
            {                
                foreach (var item in CheckData)
                {
                    var payExchangs = PayExchangeInfo.Where(t => t.OrderID == item.OrderID).ToList();
                    foreach(var payExchang in payExchangs)
                    {
                        decimal exchangeRate = payExchang.ExchangeRate;
                        var outPrice = payExchang.Amount;
                        var CNYPrice = Math.Round(outPrice * exchangeRate, 2, MidpointRounding.AwayFromZero);
                        totalPrice += outPrice;
                        totalCNYPrice += CNYPrice;
                        goodsFee += CNYPrice;
                    }
                   

                    var orderReceipt = OrderReceipts.Where(t => t.OrderID == item.OrderID);

                    ReceivablesMap receivablesMap = new ReceivablesMap();
                    receivablesMap.ID = ChainsGuid.NewGuidUp();
                    receivablesMap.OrderID = item.OrderID;
                    receivablesMap.ReceivableID = ReceivableID;
                    receivablesMap.GoodsFee = goodsFee;
                    receivablesMap.TaxFee = orderReceipt.Where(t => t.FeeType == Enums.FeeType.Tax).FirstOrDefault().Amount;
                    receivablesMap.AgencyFee = orderReceipt.Where(t => t.FeeType == Enums.FeeType.AgencyFee).FirstOrDefault().Amount;
                    receivablesMap.IncidentalFee = orderReceipt.Where(t => t.FeeType == Enums.FeeType.Incidental).FirstOrDefault().Amount;
                    receivablesMap.Enter();

                    totalCNYPrice += receivablesMap.TaxFee;
                    totalCNYPrice += receivablesMap.AgencyFee;
                    totalCNYPrice += receivablesMap.IncidentalFee;

                }
            }
            catch(Exception ex)
            {
                data.Success = false;
                data.Data = ex.ToString();
                return data;
            }
           

            data.Success = true;
            data.Data = totalCNYPrice.ToString();
            return data;
        }
    }
}
