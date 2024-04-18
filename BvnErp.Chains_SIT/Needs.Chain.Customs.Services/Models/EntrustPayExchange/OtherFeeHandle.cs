using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OtherFeeHandle
    {
        /// <summary>
        /// 小订单号
        /// </summary>
        private string TinyOrderID { get; set; }
        
        /// <summary>
        /// 实收金额
        /// </summary>
        private decimal Amount { get; set; }

        public OtherFeeHandle(string tinyOrderID, decimal amount)
        {
            this.TinyOrderID = tinyOrderID;
            this.Amount = amount;
        }

        public void Execute()
        {
            if (!this.TinyOrderID.StartsWith("XL002"))
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == this.TinyOrderID)
                    .Select(item => new
                    {
                        OrderID = item.ID,
                        ClientID = item.ClientID,
                    }).FirstOrDefault();

                if (order != null)
                {
                    IcgooBalance icgooBalanceRMB = new IcgooBalance();
                    icgooBalanceRMB.ClientID = order.ClientID;
                    icgooBalanceRMB.Currency = "RMB";
                    icgooBalanceRMB.Balance = 0 - this.Amount;
                    icgooBalanceRMB.TriggerSource = "税代杂更新余额人民币";
                    icgooBalanceRMB.UpdateBalance();
                }
            }
        }


    }
}
