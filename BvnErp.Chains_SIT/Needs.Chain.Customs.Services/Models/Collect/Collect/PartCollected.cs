using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PartCollected : ICollect
    {
        public string ReceivableID { get; set; }
        public List<CollectData> CheckData { get; set; }
        public List<PayExchangeData> PayExchangeInfo { get; set; }        
        public PartCollected(List<CollectData> data, List<PayExchangeData> payExchangeInfo, string receivableID)
        {
            this.CheckData = data;
            this.PayExchangeInfo = payExchangeInfo;            
            this.ReceivableID = receivableID;
        }
        public CheckReturnData Collect()
        {
            CheckReturnData data = new CheckReturnData();
            decimal totalPrice = 0, totalCNYPrice = 0, goodsFee = 0; ;

            List<string> orderIds = this.CheckData.Select(t => t.OrderID).ToList();

            try
            {
                foreach (var item in CheckData)
                {
                    var payExchangs = PayExchangeInfo.Where(t => t.OrderID == item.OrderID).ToList();
                    foreach (var payExchang in payExchangs)
                    {
                        decimal exchangeRate = payExchang.ExchangeRate;
                        var outPrice = payExchang.Amount;
                        var CNYPrice = Math.Round(outPrice * exchangeRate, 2, MidpointRounding.AwayFromZero);
                        totalPrice += outPrice;
                        totalCNYPrice += CNYPrice;
                        goodsFee += CNYPrice;
                    }


                    ReceivablesMap receivablesMap = new ReceivablesMap();
                    receivablesMap.ID = ChainsGuid.NewGuidUp();
                    receivablesMap.OrderID = item.OrderID;
                    receivablesMap.ReceivableID = ReceivableID;
                    receivablesMap.GoodsFee = goodsFee;
                    receivablesMap.TaxFee = 0;
                    receivablesMap.AgencyFee = 0;
                    receivablesMap.IncidentalFee = 0;
                    receivablesMap.Enter();
                }
            }
            catch (Exception ex)
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
