using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 付款通知付款事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PaymentNoticePaidHanlder(object sender, PaymentNoticePaidEventArgs e);


    /// <summary>
    /// 付款通知付款事件参数
    /// </summary>
    public class PaymentNoticePaidEventArgs : EventArgs
    {
        public PaymentNotice PaymentNotice { get; private set; }

        public PaymentNoticePaidEventArgs(PaymentNotice notice)
        {
            this.PaymentNotice = notice;
        }

        public PaymentNoticePaidEventArgs() { }
    }
}
