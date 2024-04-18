using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 付款申请取消事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PaymentApplyCancelHanlder(object sender, PaymentApplyCancelEventArgs e);
    
    /// <summary>
    /// 付款申请审批事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PaymentApplyApprovalHanlder(object sender, PaymentApplyApprovalEventArgs e);

    /// <summary>
    /// 付款申请完成事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PaymentApplyCompletedHanlder(object sender, PaymentApplyCompletedEventArgs e);

    /// <summary>
    /// 付款申请取消事件
    /// </summary>
    public class PaymentApplyCancelEventArgs : EventArgs
    {
        public Admin admin { get; set; }
        public PaymentApply PaymentApply { get; private set; }

        public PaymentApplyCancelEventArgs(PaymentApply apply,Admin admin)
        {
            this.PaymentApply = apply;
            this.admin = admin;
        }

        public PaymentApplyCancelEventArgs() { }
    }

    /// <summary>
    /// 付款申请审批事件
    /// </summary>
    public class PaymentApplyApprovalEventArgs : EventArgs
    {
        public Admin admin { get; set; }
        public PaymentApply PaymentApply { get; private set; }
        //备注
        public string Summary { get; private set; }

        public PaymentApplyApprovalEventArgs(PaymentApply apply, Admin admin,string Summary)
        {
            this.PaymentApply = apply;
            this.admin = admin;
            this.Summary = Summary;
        }

        public PaymentApplyApprovalEventArgs() { }
    }

    /// <summary>
    /// 付款申请完成事件
    /// </summary>
    public class PaymentApplyCompletedEventArgs : EventArgs
    {
        public PaymentApply PaymentApply { get; private set; }

        public Admin admin { get; set; }

        public PaymentApplyCompletedEventArgs(PaymentApply apply,Admin admin)
        {
            this.PaymentApply = apply;
            this.admin = admin;
        }

        public PaymentApplyCompletedEventArgs() { }
    }
}
