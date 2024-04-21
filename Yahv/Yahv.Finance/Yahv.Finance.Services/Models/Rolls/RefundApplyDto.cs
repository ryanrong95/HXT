using System;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 预收退款申请 传输类
    /// </summary>
    public class RefundApplyDto : IEntity
    {
        public string ID { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public FlowAccountType Type { get; set; }

        public string TypeName => this.Type.GetDescription();

        /// <summary>
        /// 收款ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeLeftAccountID { get; set; }

        /// <summary>
        /// 类型ID
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        public string PayerAccountName { get; set; }
        public string PayerAccountCode { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        public string PayeeAccountName { get; set; }
        public string PayeeAccountCode { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        public string CurrencyName => this.Currency.GetDescription();

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

        public string SenderName { get; set; }
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplierID { get; set; }

        public string ApplierName { get; set; }

        /// <summary>
        /// 执行人ID（付款人ID）
        /// </summary>
        public string ExcuterID { get; set; }

        public string ExcuterName { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string CreateDateString => this.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproverID { get; set; }

        public string ApproverName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }

        public string StatusName { get; set; }
    }
}