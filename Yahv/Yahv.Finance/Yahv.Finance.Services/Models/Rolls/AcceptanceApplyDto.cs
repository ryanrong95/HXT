using System;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    public class AcceptanceApplyDto : IEntity
    {
        public string ID { get; internal set; }

        public string TypeName => this.Type.GetDescription();

        /// <summary>
        /// 类型
        /// </summary>
        public AcceptanceType Type { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }
        public string PayerCode { get; set; }
        public string PayerBank { get; set; }

        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        public string PayeeCode { get; set; }
        public string PayeeBank { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 调出金额
        /// </summary>
        public decimal PayerPrice { get; set; }

        /// <summary>
        /// 调入金额
        /// </summary>
        public decimal PayeePrice { get; set; }

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
        /// 申请人
        /// </summary>
        public string ApplierName { get; set; }

        /// <summary>
        /// 付款人ID
        /// </summary>
        public string ExcuterID { get; set; }

        public string ExcuterName { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string CreateDateString => this.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproverID { get; set; }

        public string ApproverName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApplyStauts Status { get; set; }

        public string StatusName { get; set; }

        /// <summary>
        /// 汇票ID
        /// </summary>
        public string MoneyOrderID { get; set; }

        /// <summary>
        /// 调出账户名称
        /// </summary>
        public string PayerAccountName { get; set; }

        /// <summary>
        /// 调入账户名称
        /// </summary>
        public string PayeeAccountName { get; set; }
    }
}