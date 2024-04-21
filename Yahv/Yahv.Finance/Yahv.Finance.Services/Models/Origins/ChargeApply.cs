using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 费用申请
    /// </summary>
    public class ChargeApply : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 付款企业ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public CostApplyType Type { get; set; }

        /// <summary>
        /// 是否加急
        /// </summary>
        public bool? IsImmediately { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        public string CallBackID { get; set; }

        /// <summary>
        /// 接口发起人ID
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        public string ExcuterID { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproverID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }

        #endregion

        #region 其它属性

        /// <summary>
        /// 接口发起人名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 收款账户名称
        /// </summary>
        public string PayeeAccountName { get; set; }

        /// <summary>
        /// 付款款账户名称
        /// </summary>
        public string PayerAccountName { get; set; }

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplierName { get; set; }

        /// <summary>
        /// 审批人姓名
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        public string ExcuterName { get; set; }

        /// <summary>
        /// 收款账户账号
        /// </summary>
        public string PayeeAccountCode { get; set; }

        /// <summary>
        /// 收款账户银行
        /// </summary>
        public string PayeeAccountBankName { get; set; }

        /// <summary>
        /// 收款账户币种显示
        /// </summary>
        public string PayeeAccountCurrencyDes { get; set; }

        /// <summary>
        /// 收款账户币种
        /// </summary>
        public Currency PayeeAccountCurrency { get; set; }

        /// <summary>
        /// 付款账户账号
        /// </summary>
        public string PayerAccountCode { get; set; }

        /// <summary>
        /// 付款账户银行
        /// </summary>
        public string PayerAccountBankName { get; set; }

        /// <summary>
        /// 付款账户币种
        /// </summary>
        public string PayerAccountCurrencyDes { get; set; }

        /// <summary>
        /// 付款账户币种
        /// </summary>
        public Currency PayerAccountCurrency { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 是否加急显示
        /// </summary>
        public string IsImmediatelyDes { get; set; }

        /// <summary>
        /// 是否已付款
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// 是否已付款显示
        /// </summary>
        public string IsPaidDes { get; set; }

        /// <summary>
        /// 付款日期显示
        /// </summary>
        public string PaymentDateDes { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 付款方式显示
        /// </summary>
        public string PaymentMethordDes { get; set; }
        /// <summary>
        /// 付款公司名称
        /// </summary>
        public string PayerName { get; set; }

        public string CurrencyName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.ChargeApplies>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(Yahv.Finance.Services.PKeyType.ChargeApply);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.ChargeApplies()
                    {
                        ID = this.ID,
                        PayeeAccountID = this.PayeeAccountID,
                        PayerAccountID = this.PayerAccountID,
                        Type = (int)this.Type,
                        IsImmediately = this.IsImmediately,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Summary = this.Summary,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        SenderID = this.SenderID,
                        Department = this.Department,
                        ApplierID = this.ApplierID,
                        ExcuterID = this.ExcuterID,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ApproverID = this.ApproverID,
                        Status = (int)this.Status,
                        PayerID = this.PayerID,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.ChargeApplies>(new
                    {
                        PayeeAccountID = this.PayeeAccountID,
                        PayerAccountID = this.PayerAccountID,
                        Type = (int)this.Type,
                        IsImmediately = this.IsImmediately,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Summary = this.Summary,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        SenderID = this.SenderID,
                        Department = this.Department,
                        ApplierID = this.ApplierID,
                        ExcuterID = this.ExcuterID,
                        ApproverID = this.ApproverID,
                        Status = (int)this.Status,
                        PayerID = this.PayerID,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}