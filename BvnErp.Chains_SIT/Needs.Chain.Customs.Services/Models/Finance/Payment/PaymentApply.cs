using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请
    /// </summary>
    public class PaymentApply : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        public string OrderID { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public Admin Applier { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public Admin Approver { get; set; }

        public Enums.FinanceFeeType PayFeeType { get; set; }

        public string FeeDesc { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        public DateTime PayDate { get; set; }

        public Enums.PaymentType PayType { get; set; }

        public Enums.PaymentApplyStatus ApplyStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        //操作人
        protected Admin Operator { get; set; }
        //付款人
        protected Admin Payer { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        public void SetPayer(Admin admin)
        {
            this.Payer = admin;
        }

        #region 事件
        //新增时发生
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder UpdateSuccess;
        //删除时发生
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        //审批时发生
        public event PaymentApplyApprovalHanlder PaymentApplyApproval;
        //审批取消时发生
        public event PaymentApplyCancelHanlder PaymentApplyCancel;
        //付款完成时发生
        public event PaymentApplyCompletedHanlder PaymentApplyCompleted;

        private void PaymentApply_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var apply = (PaymentApply)e.Object;
            if (apply == null)
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //获取汇率
                decimal? exchangeRate;
                if (apply.Currency == MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY))
                {
                    exchangeRate = 1;
                }
                else
                {
                    var realExchange = new Views.RealTimeExchangeRatesView(this.Currency).ToRate();
                    exchangeRate = realExchange?.Rate;
                }

                //生成付款通知
                PaymentNotice notice = new PaymentNotice();
                notice.Admin = apply.Operator;
                notice.Payer = apply.Payer;
                notice.PaymentApply = apply;
                notice.PayFeeType = apply.PayFeeType;
                notice.FeeDesc = apply.FeeDesc;
                notice.PayeeName = apply.PayeeName;
                notice.BankName = apply.BankName;
                notice.BankAccount = apply.BankAccount;
                notice.Amount = apply.Amount;
                notice.Currency = apply.Currency;
                notice.ExchangeRate = exchangeRate;
                notice.PayDate = apply.PayDate;
                notice.PayType = apply.PayType;
                notice.Status = Enums.PaymentNoticeStatus.Paid;
                notice.Enter();
                //生成付款通知项
                PaymentNoticeItem noticeItem = new PaymentNoticeItem();
                noticeItem.PaymentNoticeID = notice.ID;
                noticeItem.OrderID = apply.OrderID;
                noticeItem.PayFeeType = apply.PayFeeType;
                noticeItem.Amount = apply.Amount;
                noticeItem.Currency = apply.Currency;
                noticeItem.Status = Enums.PaymentNoticeStatus.Paid;
                noticeItem.Enter();
            }
            if (apply.Operator != null)
            {
                apply.Log(apply.Operator, "跟单员[" + apply.Operator.RealName + "]新增付款申请");
            }
        }

        private void PaymentApply_UpdateSuccess(object sender, SuccessEventArgs e)
        {
            var apply = (PaymentApply)e.Object;
            if (apply == null)
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新付款通知
                var notice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>()
                    .Where(item => item.PaymentApplyID == apply.ID).FirstOrDefault();
                //获取汇率
                decimal? exchangeRate;
                if (apply.Currency == MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY))
                {
                    exchangeRate = 1;
                }
                else
                {
                    var realExchange = new Views.RealTimeExchangeRatesView(this.Currency).ToRate();
                    exchangeRate = realExchange?.Rate;
                }
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(
                        new
                        {
                            FeeType = (int)apply.PayFeeType,
                            FeeDesc = apply.FeeDesc,
                            PayeeName = apply.PayeeName,
                            BankName = apply.BankName,
                            BankAccount = apply.BankAccount,
                            Amount = apply.Amount,
                            Currency = apply.Currency,
                            PayDate = apply.PayDate,
                            PayType = (int)apply.PayType,
                            ExchangeRate = exchangeRate
                        }, item => item.PaymentApplyID == apply.ID);
                //更新付款通知项
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>(
                    new
                    {
                        FeeType = (int)apply.PayFeeType,
                        Amount = apply.Amount,
                        Currency = apply.Currency,
                    }, item => item.PaymentNoticeID == notice.ID);
            }
            if (apply.Operator != null)
            {
                apply.Log(apply.Operator, "跟单员[" + apply.Operator.RealName + "]编辑付款申请");
            }
        }

        private void PaymentApply_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var apply = (PaymentApply)e.Object;
            if (apply == null)
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //删除付款通知(物理删除)
                var notice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>()
                    .Where(item => item.PaymentApplyID == apply.ID).FirstOrDefault();

                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>(item => item.PaymentNoticeID == notice.ID);
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PaymentNotices>(item => item.PaymentApplyID == apply.ID);
            }
            if (apply.Operator != null)
            {
                apply.Log(apply.Operator, "跟单员[" + apply.Operator.RealName + "]删除付款申请");
            }
        }

        private void PaymentApply_Canceled(object sender, PaymentApplyCancelEventArgs e)
        {
            if (e.admin != null)
            {
                this.Log(e.admin, "财务经理[" + e.admin.RealName + "]取消付款申请");
            }
        }

        private void PaymentApply_Approvaled(object sender, PaymentApplyApprovalEventArgs e)
        {
            var apply = e.PaymentApply;
            if (apply != null)
            {
                return;
            }
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //生成付款通知
                PaymentNotice notice = new PaymentNotice();
                notice.Admin = apply.Operator;
                notice.Payer = apply.Payer;
                notice.PaymentApply = apply;
                notice.PayFeeType = apply.PayFeeType;
                notice.FeeDesc = apply.FeeDesc;
                notice.PayeeName = apply.PayeeName;
                notice.BankName = apply.BankName;
                notice.BankAccount = apply.BankAccount;
                notice.Amount = apply.Amount;
                notice.Currency = apply.Currency;
                notice.PayDate = apply.PayDate;
                notice.PayType = apply.PayType;
                notice.Summary = e.Summary;
                notice.Enter();
                //生成付款通知项
                PaymentNoticeItem noticeItem = new PaymentNoticeItem();
                noticeItem.PaymentNoticeID = notice.ID;
                noticeItem.OrderID = apply.OrderID;
                noticeItem.PayFeeType = apply.PayFeeType;
                noticeItem.Amount = apply.Amount;
                noticeItem.Currency = apply.Currency;
                noticeItem.Enter();
            }
            if (e.admin != null)
            {
                apply.Log(e.admin, "财务经理[" + e.admin.RealName + "]审批付款申请");
            }
        }

        private void PaymentApply_Completed(object sender, PaymentApplyCompletedEventArgs e)
        {
            if (e.admin != null)
            {
                e.PaymentApply.Log(e.admin, "财务[" + e.admin.ByName + "]完成付款申请");
            }
        }

        #endregion

        public PaymentApply()
        {
            //跟单新增时就表示已经付款完成
            this.ApplyStatus = Enums.PaymentApplyStatus.Completed;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.EnterSuccess += PaymentApply_EnterSuccess;
            this.UpdateSuccess += PaymentApply_UpdateSuccess;
            this.AbandonSuccess += PaymentApply_AbandonSuccess;
            this.PaymentApplyCancel += PaymentApply_Canceled;
            this.PaymentApplyApproval += PaymentApply_Approvaled;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentApplies>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentApply);
                        reponsitory.Insert(this.ToLinq());

                        this.OnEnterSuccess();
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);

                        this.OnUpdateSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentApplies>(
                        new
                        {
                            ApplyStatus = Enums.PaymentApplyStatus.Canceled,
                            UpdateDate = DateTime.Now,
                        }, item => item.ID == this.ID);
                }
                this.ApplyStatus = Enums.PaymentApplyStatus.Canceled;
                this.OnAbandonSuccess();
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentApplies>(
                        new
                        {
                            ApplyStatus = Enums.PaymentApplyStatus.Canceled,
                            UpdateDate = DateTime.Now,
                        }, item => item.ID == this.ID);
                }
                this.ApplyStatus = Enums.PaymentApplyStatus.Canceled;
                this.OnCancel();
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void Approval(string Summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentApplies>(
                    new
                    {
                        ApplyStatus = Enums.PaymentApplyStatus.Audited,
                        ApproverID = this.Operator.ID,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
            }
            this.ApplyStatus = Enums.PaymentApplyStatus.Audited;
            this.OnApproval(Summary);
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void Complete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentApplies>(
                    new
                    {
                        ApplyStatus = Enums.PaymentApplyStatus.Completed,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
            }
            this.ApplyStatus = Enums.PaymentApplyStatus.Completed;
            this.OnComplete();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void OnUpdateSuccess()
        {
            if (this != null && this.UpdateSuccess != null)
            {
                //成功后触发事件
                this.UpdateSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual public void OnCancel()
        {
            if (this != null && this.PaymentApplyCancel != null)
            {
                //成功后触发事件
                this.PaymentApplyCancel(this, new PaymentApplyCancelEventArgs(this, this.Operator));
            }
        }

        virtual public void OnApproval(string Summary)
        {
            if (this != null && this.PaymentApplyApproval != null)
            {
                //成功后触发事件
                this.PaymentApplyApproval(this, new PaymentApplyApprovalEventArgs(this, this.Operator, Summary));
            }
        }

        virtual public void OnComplete()
        {
            if (this != null && this.PaymentApplyCompleted != null)
            {
                //成功后触发事件
                this.PaymentApplyCompleted(this, new PaymentApplyCompletedEventArgs(this, this.Operator));
            }
        }
    }
}
