using System;
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
    /// 承兑调拨申请
    /// </summary>
    public class AcceptanceApply : IUnique, IDataEntity
    {
        #region 事件
        public event SuccessHanlder UpdateSuccess;
        #endregion

        #region 数据库属性
        public string ID { get; internal set; }

        /// <summary>
        /// 类型
        /// </summary>
        public AcceptanceType Type { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }

        /// <summary>
        /// 付款人ID
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
        /// 审批人
        /// </summary>
        public string ApproverID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }

        /// <summary>
        /// 汇票ID
        /// </summary>
        public string MoneyOrderID { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (string.IsNullOrEmpty(this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.AcceptanceApplies);
                    repository.Insert(new Layers.Data.Sqls.PvFinance.AcceptanceApplies()
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Summary = this.Summary,
                        SenderID = this.SenderID,
                        ApplierID = this.ApplierID,
                        ExcuterID = this.ExcuterID,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        ApproverID = this.ApproverID,
                        Status = (int)this.Status,
                        MoneyOrderID = this.MoneyOrderID,
                    });

                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvFinance.AcceptanceApplies>(new
                    {
                        Type = (int)this.Type,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Summary = this.Summary,
                        SenderID = this.SenderID,
                        ApplierID = this.ApplierID,
                        ExcuterID = this.ExcuterID,
                        ApproverID = this.ApproverID,
                        Status = (int)this.Status,
                        MoneyOrderID = this.MoneyOrderID,
                    }, item => item.ID == this.ID);

                    this.UpdateSuccess?.Invoke(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}