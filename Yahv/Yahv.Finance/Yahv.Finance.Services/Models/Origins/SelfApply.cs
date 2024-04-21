using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 资金调拨
    /// </summary>
    public class SelfApply : IUnique
    {
        #region 事件

        public event SuccessHanlder Success;

        #endregion

        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 调出币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 调出金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 调入币种
        /// </summary>
        public Currency TargetCurrency { get; set; }

        /// <summary>
        /// 调入金额
        /// </summary>
        public decimal TargetPrice { get; set; }

        /// <summary>
        /// 调出对调入的汇率
        /// </summary>
        public decimal TargetERate { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 调出账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 调入账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 回调ID
        /// </summary>
        public string CallBackID { get; set; }

        /// <summary>
        /// 接口发起系统
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 申请人所属的部门名称
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        public string ExcuterID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 下一个审批人ID
        /// </summary>
        public string ApproverID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 收款账户
        /// </summary>
        public Account PayeeAccount { get; set; }
        /// <summary>
        /// 付款账户
        /// </summary>
        public Account PayerAccount { get; set; }
        /// <summary>
        /// 接口发起人
        /// </summary>
        public Sender Sender { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public Admin Applier { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public Admin Approver { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public Admin Excuter { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SelfApplies>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.SelfApply);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.SelfApplies()
                    {
                        Currency = (int)this.Currency,
                        CreateDate = DateTime.Now,
                        ID = this.ID,
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        Price = this.Price,
                        Summary = this.Summary,
                        PayeeAccountID = this.PayeeAccountID,
                        ApproverID = this.ApproverID,
                        PayerAccountID = this.PayerAccountID,
                        ExcuterID = this.ExcuterID,
                        ApplierID = this.ApplierID,
                        SenderID = this.SenderID,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        Department = this.Department,
                        TargetCurrency = (int)this.TargetCurrency,
                        TargetERate = this.TargetERate,
                        TargetPrice = this.TargetPrice,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.SelfApplies>(new
                    {
                        Currency = (int)this.Currency,
                        Status = (int)this.Status,
                        Price = this.Price,
                        Summary = this.Summary,
                        PayeeAccountID = this.PayeeAccountID,
                        ApproverID = this.ApproverID,
                        PayerAccountID = this.PayerAccountID,
                        ExcuterID = this.ExcuterID,
                        ApplierID = this.ApplierID,
                        SenderID = this.SenderID,
                        CallBackUrl = this.CallBackUrl,
                        CallBackID = this.CallBackID,
                        Department = this.Department,
                        TargetCurrency = (int)this.TargetCurrency,
                        TargetERate = this.TargetERate,
                        TargetPrice = this.TargetPrice,
                    }, item => item.ID == this.ID);
                }

                this.Success?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}