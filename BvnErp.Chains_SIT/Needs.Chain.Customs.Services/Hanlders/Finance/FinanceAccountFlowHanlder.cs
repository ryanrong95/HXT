using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Hanlders.Finance
{
    /// <summary>
    /// 收付款流水金额变更事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AccountFlowAmountUpdatedHanlder(object sender, AccountFlowAmountUpdatedEventArgs e);

    /// <summary>
    /// 财务收款更新事件
    /// </summary>
    public class AccountFlowAmountUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// 差额
        /// </summary>
        public decimal Difference { get; private set; }

        public FinanceAccountFlow FinanceAccountFlow { get; private set; }

        public AccountFlowAmountUpdatedEventArgs(FinanceAccountFlow flow)
        {
            this.FinanceAccountFlow = flow;
        }

        public AccountFlowAmountUpdatedEventArgs(FinanceAccountFlow flow, decimal difference)
        {
            this.FinanceAccountFlow = flow;
            this.Difference = difference;
        }

        public AccountFlowAmountUpdatedEventArgs() { }
    }
}
