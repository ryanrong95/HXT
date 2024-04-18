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
    public delegate void DyjFinanceReceiptEnterHanlder(object sender, DyjFinanceReceiptEnterEventArgs e);

    /// <summary>
    /// 芯达通收款调用大赢家接口事件
    /// </summary>
    public class DyjFinanceReceiptEnterEventArgs : EventArgs
    {
        public FinanceReceipt DyjReceipt { get; private set; }

        public DyjFinanceReceiptEnterEventArgs(FinanceReceipt receipt)
        {
            this.DyjReceipt = receipt;
        }

        public DyjFinanceReceiptEnterEventArgs() { }
    }




    /// <summary>
    /// 芯达通费用申请调用大赢家接口事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DyjFeeApplyEnterHanlder(object sender, DyjFeeApplyEnterEventArgs e);

    /// <summary>
    /// 芯达通费用申请调用大赢家接口事件
    /// </summary>
    public class DyjFeeApplyEnterEventArgs : EventArgs
    {
        public CostApply DyjFeeApply { get; private set; }

        public DyjFeeApplyEnterEventArgs(CostApply apply)
        {
            this.DyjFeeApply = apply;
        }

        public DyjFeeApplyEnterEventArgs() { }
    }



    /// <summary>
    /// 芯达通付汇申请调用大赢家接口事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DyjPayExchangeApplyHanlder(object sender, DyjPayExchangeApplyEventArgs e);

    /// <summary>
    /// 芯达通费用申请调用大赢家接口事件
    /// </summary>
    public class DyjPayExchangeApplyEventArgs : EventArgs
    {
        public UnAuditedPayExchangeApply DyjPayExchangeApply { get; private set; }

        public DyjPayExchangeApplyEventArgs(UnAuditedPayExchangeApply apply)
        {
            this.DyjPayExchangeApply = apply;
        }

        public DyjPayExchangeApplyEventArgs() { }
    }



    /// <summary>
    /// 芯达通付汇审批调用大赢家接口事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DyjPayExchangeApprovalHanlder(object sender, DyjPayExchangeApprovalEventArgs e);

    /// <summary>
    /// 芯达通费用申请调用大赢家接口事件
    /// </summary>
    public class DyjPayExchangeApprovalEventArgs : EventArgs
    {
        public UnApprovalPayExchangeApply DyjPayExchangeApply { get; private set; }

        public string Summary { get; set; }

        public bool IsPass { get; set; }

        public DyjPayExchangeApprovalEventArgs(UnApprovalPayExchangeApply apply, string summary, bool IsPass)
        {
            this.DyjPayExchangeApply = apply;
            this.Summary = summary;
            this.IsPass = IsPass;
        }

        public DyjPayExchangeApprovalEventArgs() { }
    }

}
