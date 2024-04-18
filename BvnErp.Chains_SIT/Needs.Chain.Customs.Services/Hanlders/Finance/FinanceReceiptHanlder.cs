using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 财务收款更新事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FinanceReceiptUpdatedHanlder(object sender, FinanceReceiptUpdatedEventArgs e);

    /// <summary>
    /// 财务收款更新事件
    /// </summary>
    public class FinanceReceiptUpdatedEventArgs : EventArgs
    {
        public FinanceReceipt FinanceReceipt { get; private set; }

        public FinanceReceiptUpdatedEventArgs(FinanceReceipt receipt)
        {
            this.FinanceReceipt = receipt;
        }

        public FinanceReceiptUpdatedEventArgs() { }
    }
}
