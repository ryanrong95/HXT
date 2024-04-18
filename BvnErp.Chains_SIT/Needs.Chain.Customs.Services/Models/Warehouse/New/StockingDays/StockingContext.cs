using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StockingContext
    {
        private StockingStrategy stocking { get; set; }

        /// <summary>
        /// 传的是小订单ID
        /// </summary>
        /// <param name="orderID"></param>
        public StockingContext(string orderID)
        {
            string[] orders = orderID.Split('-');
            var Order = new PvWsOrderBaseOrderView().Where(t => t.ID == orders[0]).FirstOrDefault();                     
            switch (Order.OrderType)
            {
                case Enums.ClientOrderType.Declare:
                    this.stocking = new DeclareStrategy(orderID);
                    break;

                case Enums.ClientOrderType.TransferDeclare:
                    this.stocking = new ToDeclareStrategy(orderID);
                    break;
            }
        }

        public int calculate()
        {
           return  stocking.CalculateDays();
        }
    }
}
