
using System;
using System.Collections.Generic;


namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 会员付汇申请
    /// 完成付汇及取消（删除付汇）
    /// </summary>
    public sealed class UserPayExchangeApply : PayExchangeApply
    {
        #region 属性

        /// <summary>
        /// 付款日期、完成日期
        /// 通过Logs中的完成日期确定
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// 付汇申请的付汇总金额
        /// </summary>
        public decimal? TotalAmount { get; set; }

        #endregion

        #region 构造函数

        public UserPayExchangeApply()
        {
            this.Canceled += UserPayExchangeApply_Canceled;
        }

        public UserPayExchangeApply(IEnumerable<UnPayExchangeOrder> order) : base(order)
        {
            this.Applyed += UserPayExchangeApply_Applyed;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 当申请付汇申请后发生
        /// </summary>
        public event Hanlders.PayExchangeApplyedHanlder Applyed;

        /// <summary>
        /// 当删除付汇申请后发生
        /// </summary>
        public event Hanlders.PayExchangeApplyDeletedHanlder Canceled;

        private void UserPayExchangeApply_Canceled(object sender, Hanlders.PayExchangeApplyDeletedEventArgs e)
        {
            e.PayExchangeApply.Log("用户[" + e.PayExchangeApply.User.RealName + "]取消了付汇申请");
        }

        private void UserPayExchangeApply_Applyed(object sender, Hanlders.PayExchangeApplyedEventArgs e)
        {
            e.PayExchangeApply.Log("用户[" + e.PayExchangeApply.User.RealName + "]提交了付汇申请");
        }

        #endregion

        #region 持久化

        public override void Enter()
        {
            base.Enter();
            this.OnEntered();
        }

        /// <summary>
        /// 取消
        /// </summary>
        public override void Abandon()
        {
            base.Abandon();
            this.OnAbandoned();
        }

        public void OnEntered()
        {
            if (this != null && this.Applyed != null)
            {
                //成功后触发事件
                this.Applyed(this, new Hanlders.PayExchangeApplyedEventArgs(this));
            }
        }

        public void OnAbandoned()
        {
            if (this != null && this.Canceled != null)
            {
                this.Canceled(this, new Hanlders.PayExchangeApplyDeletedEventArgs(this));
            }
        }

        #endregion       
    }
}