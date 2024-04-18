using System;

namespace Needs.Wl.Models.Hanlders
{
    /// <summary>
    ///  付汇申请提交成功句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeSubmitedHanlder(object sender, PayExchangeSubmitedEventArgs e);

    /// <summary>
    ///  付汇申请取消
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeCanceledHanlder(object sender, PayExchangeCanceledEventArgs e);

    /// <summary>
    ///  付汇申请删除
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeDeletedHanlder(object sender, PayExchangeDeletedEventArgs e);

    /// <summary>
    /// 付汇申请审核事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyAuditedHanlder(object sender, PayExchangeAuditedEventArgs e);

    /// <summary>
    /// 付汇申请审批事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyApprovaledHanlder(object sender, PayExchangeApprovaledEventArgs e);

    /// <summary>
    /// 付汇申请审批取消事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyApprovalCanceledHanlder(object sender, PayExchangeApprovalCanceledEventArgs e);

    /// <summary>
    /// 付汇申请已付款事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void PayExchangeApplyPaidHanlder(object sender, PayExchangePaidEventArgs e);

    /// <summary>
    /// 付汇申请提交成功事件参数
    /// </summary>
    public class PayExchangeSubmitedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeSubmitedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeSubmitedEventArgs()
        {

        }
    }

    /// <summary>
    /// 取消事件
    /// </summary>
    public class PayExchangeCanceledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeCanceledEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeCanceledEventArgs()
        {

        }
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    public class PayExchangeDeletedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeDeletedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeDeletedEventArgs()
        {

        }
    }

    /// <summary>
    /// 审核事件
    /// </summary>
    public class PayExchangeAuditedEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangeAuditedEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangeAuditedEventArgs()
        {

        }
    }

    /// <summary>
    /// 审批事件
    /// </summary>
    public class PayExchangeApprovaledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public string Summary { get; set; }

        public PayExchangeApprovaledEventArgs(PayExchangeApply entity, string summary)
        {
            this.PayExchangeApply = entity;
            this.Summary = summary;
        }

        public PayExchangeApprovaledEventArgs()
        {

        }
    }

    /// <summary>
    /// 审批取消事件
    /// </summary>
    public class PayExchangeApprovalCanceledEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public string Summary { get; set; }

        public PayExchangeApprovalCanceledEventArgs(PayExchangeApply entity, string summary)
        {
            this.PayExchangeApply = entity;
            this.Summary = summary;
        }

        public PayExchangeApprovalCanceledEventArgs()
        {

        }
    }

    /// <summary>
    /// 已付款事件
    /// </summary>
    public class PayExchangePaidEventArgs : EventArgs
    {
        public PayExchangeApply PayExchangeApply;

        public PayExchangePaidEventArgs(PayExchangeApply entity)
        {
            this.PayExchangeApply = entity;
        }

        public PayExchangePaidEventArgs()
        {

        }
    }
}