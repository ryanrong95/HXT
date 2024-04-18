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
    public delegate void MainExchangeRateChangedHanlder(object sender, MainExchangeRateChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class MainExchangeRateChangedEventArgs : EventArgs
    {
        public Models.OrderBill Bill { get; set; }
        public Enums.OrderBillType OrderBillType { get; set; }
        public decimal RealAgencyFee { get; set; }
        public MainExchangeRateChangedEventArgs(Models.OrderBill bill, Enums.OrderBillType orderBillType,decimal realAgencyFee)
        {
            this.Bill = bill;
            this.OrderBillType = orderBillType;
            this.RealAgencyFee = realAgencyFee;
        }

        public MainExchangeRateChangedEventArgs()
        {

        }
    }
}
