using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SFRequestPara
    {
        public string OrderID { get; set; }       
        public int ExpType { get; set; }
        /// <summary>
        /// 支付方式，库房那边的月结是1
        /// </summary>
        public int ExPayType { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
        public double? Volume { get; set; }
        public double? Weight { get; set; }
        public List<Commodity> Commodities { get; set; }

        public SFRequestPara()
        {           
            this.ExPayType = 1;
            this.Quantity = 1;
            this.Remark = "小心轻放";
            this.Volume = 0.01;
            this.Weight = 0.01;
            Commodity com = new Commodity
            {
                GoodsName = "客户发票"
            };

            this.Commodities = new List<Commodity>();
            this.Commodities.Add(com);
        }
    }
}
