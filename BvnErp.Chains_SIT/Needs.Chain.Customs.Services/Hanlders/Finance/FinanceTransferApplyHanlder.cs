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
    public delegate void FinanceTransferApplyHanlder(object sender, FinanceTransferApplyEventArgs e);


    /// <summary>
    /// 财务付款更新事件
    /// </summary>
    public class FinanceTransferApplyEventArgs : EventArgs
    {
        public FundTransferApplies FinanceTransferApply { get; private set; }

        public FinanceTransferApplyEventArgs(FundTransferApplies finance)
        {
            this.FinanceTransferApply = finance;
        }

        public FinanceTransferApplyEventArgs() { }
    }

   
}
