using System;
using System.Linq;
using System.Security.AccessControl;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 退款申请
    /// </summary>
    public class RefundApply : IUnique, IDataEntity
    {
        #region 数据库属性
        public string ID { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public FlowAccountType Type { get; set; }

        /// <summary>
        /// 收款ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

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
        /// 系统
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }

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

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!repository.ReadTable<Layers.Data.Sqls.PvFinance.RefundApplies>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.RefundApplies);

                    repository.Insert(new Layers.Data.Sqls.PvFinance.RefundApplies()
                    {
                        ID = this.ID,
                        Currency = (int)this.Currency,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        AccountCatalogID = this.AccountCatalogID,
                        ApplierID = this.ApplierID,
                        ApproverID = this.ApproverID,
                        ExcuterID = this.ExcuterID,
                        FlowID = this.FlowID,
                        PayeeAccountID = this.PayeeAccountID,
                        PayeeLeftID = this.PayeeLeftID,
                        PayerAccountID = this.PayerAccountID,
                        Price = this.Price,
                        SenderID = this.SenderID,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        Type = (int)this.Type,
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvFinance.RefundApplies>(new
                    {
                        Currency = (int)this.Currency,
                        ApplierID = this.ApplierID,
                        ApproverID = this.ApproverID,
                        ExcuterID = this.ExcuterID,
                        FlowID = this.FlowID,
                        PayeeAccountID = this.PayeeAccountID,
                        PayeeLeftID = this.PayeeLeftID,
                        PayerAccountID = this.PayerAccountID,
                        Price = this.Price,
                        SenderID = this.SenderID,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        Type = (int)this.Type,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}