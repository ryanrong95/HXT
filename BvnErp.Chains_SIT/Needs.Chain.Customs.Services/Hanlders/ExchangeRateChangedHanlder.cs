using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 OrderBill.ChangeExchangeRate 事件
    ///  订单取消
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ExchangeRateChangedHanlder(object sender, ExchangeRateChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ExchangeRateChangedEventArgs : EventArgs
    {
        public Models.OrderBill Bill { get; set; }

        public ExchangeRateChangedEventArgs(Models.OrderBill bill)
        {
            this.Bill = bill;
        }

        public ExchangeRateChangedEventArgs()
        {

        }
    }
}
