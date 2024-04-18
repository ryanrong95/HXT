using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  付汇申请
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyedHanlder(object sender, PayExchangeApplyedEventArgs e);

    /// <summary>
    ///  付汇申请取消
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyCanceledHanlder(object sender, PayExchangeApplyCanceledEventArgs e);

    /// <summary>
    ///  付汇申请删除
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyDeletedHanlder(object sender, PayExchangeApplyDeletedEventArgs e);

    /// <summary>
    /// 付汇申请审核事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyAuditedHanlder(object sender, PayExchangeApplyAuditedEventArgs e);

    /// <summary>
    /// 付汇申请审批事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyApprovaledHanlder(object sender, PayExchangeApplyApprovaledEventArgs e);

    /// <summary>
    /// 付汇申请审批取消事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyApprovalCanceledHanlder(object sender, PayExchangeApplyApprovalCanceledEventArgs e);

    /// <summary>
    /// 付汇申请已付款事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyPaidHanlder(object sender, PayExchangeApplyPaidEventArgs e);

    /// <summary>
    /// 申请事件
    /// </summary>
    public class PayExchangeApplyedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string PayExchangeApplyID { get; private set; }

        public PayExchangeApplyedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeApplyedEventArgs()
        {

        }
    }

    /// <summary>
    /// 取消事件
    /// </summary>
    public class PayExchangeApplyCanceledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string PayExchangeApplyID { get; private set; }

        public PayExchangeApplyCanceledEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeApplyCanceledEventArgs()
        {

        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    public class PayExchangeApplyDeletedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeApplyDeletedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeApplyDeletedEventArgs()
        {

        }
    }

    /// <summary>
    /// 审核事件
    /// </summary>
    public class PayExchangeApplyAuditedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeApplyAuditedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeApplyAuditedEventArgs()
        {

        }
    }

    /// <summary>
    /// 审批事件
    /// </summary>
    public class PayExchangeApplyApprovaledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public string Summary { get; set; }

        public PayExchangeApplyApprovaledEventArgs(PayExchangeApply entity, string summary)
        {
            this.PayExchangeApply = entity;
            this.Summary = summary;
        }

        public PayExchangeApplyApprovaledEventArgs()
        {

        }
    }

    /// <summary>
    /// 审批取消事件
    /// </summary>
    public class PayExchangeApplyApprovalCanceledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public string Summary { get; set; }

        public PayExchangeApplyApprovalCanceledEventArgs(PayExchangeApply entity, string summary)
        {
            this.PayExchangeApply = entity;
            this.Summary = summary;
        }

        public PayExchangeApplyApprovalCanceledEventArgs()
        {

        }
    }

    /// <summary>
    /// 已付款事件
    /// </summary>
    public class PayExchangeApplyPaidEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeApplyPaidEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeApplyPaidEventArgs()
        {

        }
    }
}