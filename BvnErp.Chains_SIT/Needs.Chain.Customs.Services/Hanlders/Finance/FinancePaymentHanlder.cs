using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 财务付款更新事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FinancePaymentUpdateHanlder(object sender, FinancePaymentUpdateEventArgs e);

    /// <summary>
    /// 财务付款更新事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FinancePaymentDeleteHanlder(object sender, FinancePaymentDeleteEventArgs e);


    /// <summary>
    /// 财务付款更新事件
    /// </summary>
    public class FinancePaymentUpdateEventArgs : EventArgs
    {
        public FinancePayment FinancePayment { get; private set; }

        public FinancePaymentUpdateEventArgs(FinancePayment finance)
        {
            this.FinancePayment = finance;
        }

        public FinancePaymentUpdateEventArgs() { }
    }

    /// <summary>
    /// 财务付款更新事件
    /// </summary>
    public class FinancePaymentDeleteEventArgs : EventArgs
    {
        public FinancePayment FinancePayment { get; private set; }

        public FinancePaymentDeleteEventArgs(FinancePayment finance)
        {
            this.FinancePayment = finance;
        }

        public FinancePaymentDeleteEventArgs() { }
    }
}
