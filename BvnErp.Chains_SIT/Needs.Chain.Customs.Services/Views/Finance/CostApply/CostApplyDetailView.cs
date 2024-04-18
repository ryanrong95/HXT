using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付款申请详情视图
    /// </summary>
    public class CostApplyDetailView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public CostApplyDetailView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public CostApplyDetailView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public CostApplyDetailViewModel GetResult(string costApplyID)
        {
            var costApplies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.CostApplies>();
            var applicantAdminView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            var result = from costApply in costApplies
                         join applicantAdmin in applicantAdminView on costApply.AdminID equals applicantAdmin.ID into applicantAdminView2
                         from applicantAdmin in applicantAdminView2.DefaultIfEmpty()
                         where costApply.ID == costApplyID
                         select new CostApplyDetailViewModel
                         {
                             CostApplyID = costApply.ID,
                             PayeeName = costApply.PayeeName,
                             PayeeAccount = costApply.PayeeAccount,
                             PayeeBank = costApply.PayeeBank,   
                             PayeeAccountID = costApply.PayeeAccountID,
                             Amount = costApply.Amount,
                             Currency = costApply.Currency,
                             ApplicantID = costApply.AdminID,
                             ApplicantName = applicantAdmin.RealName,
                             Summary = costApply.Summary,
                             CostStatus = (Enums.CostStatusEnum)costApply.CostStatus,
                             CreateDate = costApply.CreateDate,
                             MoneyType = (Enums.MoneyTypeEnum)costApply.MoneyType,
                             CashType = (Enums.CashTypeEnum)costApply.CashType,
                             DyjID = costApply.DyjID,
                             DyjCheckID = costApply.DyjCheckID
                         };


            return result.FirstOrDefault();
        }
    }

    public class CostApplyDetailViewModel
    {
        /// <summary>
        /// CostApplyID
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 收款方名称
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// 收款方账号
        /// </summary>
        public string PayeeAccount { get; set; } = string.Empty;

        /// <summary>
        /// 收款方银行
        /// </summary>
        public string PayeeBank { get; set; } = string.Empty;
        /// <summary>
        /// 收款方账号ID
        /// </summary>
        public string PayeeAccountID { get; set; } = string.Empty;

        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.CostTypeEnum CostType { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        public Enums.FeeTypeEnum FeeType { get; set; }

        /// <summary>
        /// 其它
        /// </summary>
        public string FeeDesc { get; set; } = string.Empty;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplicantID { get; set; } = string.Empty;

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplicantName { get; set; } = string.Empty;

        /// <summary>
        /// 申请备注
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// CostApply 状态
        /// </summary>
        public Enums.CostStatusEnum CostStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 个人申请费用/银行自动扣款
        /// </summary>
        public Enums.MoneyTypeEnum MoneyType { get; set; }

        /// <summary>
        /// 是否是现金账户
        /// </summary>
        public Enums.CashTypeEnum CashType { get; set; }

        public int? DyjID { get; set; }

        public int? DyjCheckID { get; set; }
    }
}
