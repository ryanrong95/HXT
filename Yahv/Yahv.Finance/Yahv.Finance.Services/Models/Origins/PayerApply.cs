using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.ScCustoms;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 货款申请
    /// </summary>
    public class PayerApply : IUnique
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

        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 收款账户
        /// </summary>
        public string PayeeAccountID { get; set; }
        /// <summary>
        /// 付款账户
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string PayerID { get; set; }

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
        /// 是否已支付（true 银行自动扣款或已扣款、false 需要出纳付款）
        /// </summary>
        public bool IsPaid { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallBackUrl { get; set; }
        /// <summary>
        /// 回调参数
        /// </summary>
        public string CallBackID { get; set; }
        /// <summary>
        /// 接口发起人
        /// </summary>
        public string SenderID { get; set; }
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }
        /// <summary>
        /// 申请人所属部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 执行人ID（付款人ID）
        /// </summary>
        public string ExcuterID { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建时间
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
        /// <summary>
        /// 货款应付
        /// </summary>
        public PayerLeft PayerLeft { get; set; }

        /// <summary>
        /// 付款公司名称
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 临时
        /// </summary>
        public string Temp1 { get; set; }

        /// <summary>
        /// 临时
        /// </summary>
        public string Temp2 { get; set; }

        /// <summary>
        /// 临时
        /// </summary>
        public object Temp3 { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加
        /// </summary>
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //增加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerApplies>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.PayerApply);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerApplies()
                    {
                        ID = this.ID,
                        ApplierID = this.ApplierID,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        Price = this.Price,
                        ApproverID = this.ApproverID,
                        CallBackID = this.CallBackID,
                        CallBackUrl = this.CallBackUrl,
                        Currency = (int)this.Currency,
                        Department = this.Department,
                        ExcuterID = this.ExcuterID,
                        IsPaid = this.IsPaid,
                        SenderID = this.SenderID,
                        Summary = this.Summary,
                        PayeeAccountID = this.PayeeAccountID,
                        PayerAccountID = this.PayerAccountID,
                        PayerID = this.PayerID,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.PayerApplies>(new
                    {
                        PayerAccountID = this.PayerAccountID,
                        Status = (int)this.Status,
                        Price = this.Price,
                        ApproverID = this.ApproverID,
                        CallBackID = this.CallBackID,
                        CallBackUrl = this.CallBackUrl,
                        Currency = (int)this.Currency,
                        ExcuterID = this.ExcuterID,
                        Summary = this.Summary,
                        IsPaid = this.IsPaid,
                        SenderID = this.SenderID,
                        PayerID = this.PayerID,
                    }, item => item.ID == this.ID);
                }

                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}