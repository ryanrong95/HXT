using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 芯达通收款调用大赢家接口事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CenterFinanceReceiptEnterHanlder(object sender, CenterFinanceReceiptEnterEventArgs e);

    /// <summary>
    /// 芯达通收款调用大赢家接口事件
    /// </summary>
    public class CenterFinanceReceiptEnterEventArgs : EventArgs
    {
        public FinanceReceipt CenterReceipt { get; private set; }       
        public string OldSeqNo { get; set; }
        public CenterFinanceReceiptEnterEventArgs(FinanceReceipt receipt,string oldSeqNo)
        {
            this.CenterReceipt = receipt;           
            this.OldSeqNo = oldSeqNo;
        }

        public CenterFinanceReceiptEnterEventArgs() { }
    }
}
