using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 生成对账单委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GenerateOrderBillHanlder(object sender, GenerateOrderBillEventArgs e);

    /// <summary>
    /// 生成对账单参数
    /// </summary>
    public class GenerateOrderBillEventArgs : EventArgs
    {
        public Models.Order Order { get; private set; }

        public string OrderID { get; private set; }

        public GenerateOrderBillEventArgs(Models.Order order)
        {
            this.Order = order;
        }

        public GenerateOrderBillEventArgs(string orderID)
        {
            this.OrderID = orderID;
        }

        public GenerateOrderBillEventArgs() { }
    }
}
