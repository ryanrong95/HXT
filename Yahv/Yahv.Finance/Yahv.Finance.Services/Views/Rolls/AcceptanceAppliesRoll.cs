using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Extends;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 承兑调拨申请
    /// </summary>
    public class AcceptanceAppliesRoll : vDepthView<AcceptanceApply, AcceptanceApplyDto, PvFinanceReponsitory>
    {
        public AcceptanceAppliesRoll()
        {
        }

        public AcceptanceAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        public AcceptanceAppliesRoll(PvFinanceReponsitory reponsitory, IQueryable<AcceptanceApply> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<AcceptanceApply> GetIQueryable()
        {
            return new AcceptanceAppliesOrigin(this.Reponsitory);
        }

        #region 分页查询
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <returns></returns>
        protected override IEnumerable<AcceptanceApplyDto> OnMyPage(IQueryable<AcceptanceApply> iquery)
        {
            var query = iquery.ToArray();

            var accountIds = query.Select(item => item.PayeeAccountID)
                .Concat(query.Select(item => item.PayerAccountID));
            var adminIds = query.Select(item => item.CreatorID)
                .Concat(query.Select(item => item.ApproverID))
                .Concat(query.Select(item => item.ApplierID))
                .Concat(query.Select(item => item.ExcuterID))
                .ToArray();

            var accounts = new AccountsOrigin(this.Reponsitory).Where(item => accountIds.Contains(item.ID));
            var admins = new AdminsTopView(this.Reponsitory).Where(item => adminIds.Contains(item.ID));

            return from entity in query
                   join payeeAccount in accounts on entity.PayeeAccountID equals payeeAccount.ID into _payeeAccount
                   from payeeAccount in _payeeAccount.DefaultIfEmpty()
                   join payerAccount in accounts on entity.PayerAccountID equals payerAccount.ID into _payerAccount
                   from payerAccount in _payerAccount.DefaultIfEmpty()
                   join admin in admins on entity.ApplierID equals admin.ID
                   join creator in admins on entity.ApplierID equals creator.ID
                   join approver in admins on entity.ApproverID equals approver.ID into _approver
                   from approver in _approver.DefaultIfEmpty()
                   join excuter in admins on entity.ExcuterID equals excuter.ID into _excuter
                   from excuter in _excuter.DefaultIfEmpty()
                   orderby entity.Status ascending, entity.CreateDate descending
                   select new AcceptanceApplyDto
                   {
                       ID = entity.ID,
                       PayerAccountID = entity.PayerAccountID,
                       PayerAccountName = payerAccount?.ShortName ?? payerAccount?.Name,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayeeAccountName = payeeAccount?.ShortName ?? payeeAccount?.Name,
                       PayerPrice = entity.Price,
                       Type = entity.Type,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Status = entity.Status,
                       ApplierName = admin?.RealName,
                       StatusName = GetStatusName(admin?.RealName, excuter?.RealName, approver?.RealName, entity.Status),
                       Currency = entity.Currency,
                       MoneyOrderID = entity.MoneyOrderID,
                       Summary = entity.Summary,
                       ApproverID = entity.ApproverID,
                       PayeeCode = payeeAccount?.Code,
                       ApplierID = entity.ApplierID,
                       ExcuterID = entity.ExcuterID,
                       SenderID = entity.SenderID,
                       ApproverName = approver?.RealName,
                       CreatorName = creator?.RealName,
                       ExcuterName = excuter?.RealName,
                       PayeeBank = payeeAccount?.BankName,
                       PayerBank = payerAccount?.BankName,
                       PayerCode = payerAccount?.Code,
                   };
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public AcceptanceAppliesRoll Where(Expression<Func<AcceptanceApply, bool>> predicate)
        {
            return new AcceptanceAppliesRoll(this.Reponsitory, this.GetIQueryable().Where(predicate));
        }

        /// <summary>
        /// 根据调出账户名称查询
        /// </summary>
        /// <param name="payerName">调出账户名称</param>
        /// <returns></returns>
        public AcceptanceAppliesRoll SearchByPayerName(string payerName)
        {
            if (payerName.IsNullOrEmpty())
            {
                return this;
            }

            var accounts = new AccountsOrigin(this.Reponsitory);
            var linq = from entity in this.IQueryable
                       join account in accounts on entity.PayerAccountID equals account.ID into _account
                       from account in _account.DefaultIfEmpty()
                       where account.Name.Contains(payerName) || account.ShortName.Contains(payerName)
                       select entity;
            return new AcceptanceAppliesRoll(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据调入账户名称查询
        /// </summary>
        /// <param name="payeeName">调入账户名称</param>
        /// <returns></returns>
        public AcceptanceAppliesRoll SearchByPayeeName(string payeeName)
        {
            if (payeeName.IsNullOrEmpty())
            {
                return this;
            }

            var accounts = new AccountsOrigin(this.Reponsitory);
            var linq = from entity in this.IQueryable
                       join account in accounts on entity.PayeeAccountID equals account.ID into _account
                       from account in _account.DefaultIfEmpty()
                       where account.Name.Contains(payeeName) || account.ShortName.Contains(payeeName)
                       select entity;
            return new AcceptanceAppliesRoll(this.Reponsitory, linq);
        }
        #endregion

        #region 索引器
        public AcceptanceApply this[string id]
        {
            get { return this.IQueryable.FirstOrDefault(item => item.ID == id); }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AcceptanceApplyDto Find(string id)
        {
            return this.OnMyPage(this.IQueryable.Where(item => item.ID == id)).FirstOrDefault();
        }
        #endregion

        #region 对外函数
        /// <summary>
        /// 调出、调入 左表
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="methord"></param>
        /// <returns></returns>
        public AcceptanceLeft GetAcceptanceLeft(string applyId, AccountMethord methord)
        {
            var lefts = new AcceptanceLeftsOrigin(this.Reponsitory)
                .Where(item => item.AccountMethord == methord);
            return lefts.FirstOrDefault(item => item.ApplyID == applyId);
        }

        public IQueryable<AcceptanceLeft> GetAcceptanceLeftsByApplyId(string applyId)
        {
            return new AcceptanceLeftsOrigin(this.Reponsitory).Where(item => item.ApplyID == applyId);
        }

        public IQueryable<AcceptanceRight> GetAcceptanceRightsByApplyId(string applyId)
        {
            var leftIds = GetAcceptanceLeftsByApplyId(applyId).Select(item => item.ID).ToArray();
            return new AcceptanceRightsOrigin(this.Reponsitory).Where(item => leftIds.Contains(item.AcceptanceLeftID));
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取状态名称
        /// </summary>
        private string GetStatusName(string applierName, string excuterName, string approverName, ApplyStauts status)
        {
            string result = String.Empty;
            switch (status)
            {
                case ApplyStauts.Completed:
                    result = status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = applierName != null ? $"{status.GetDescription()}({applierName})" : status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = excuterName != null ? $"{status.GetDescription()}({excuterName})" : status.GetDescription();
                    break;
                default:
                    result = approverName != null ? $"{status.GetDescription()}({approverName})" : status.GetDescription();
                    break;
            }
            return result;
        }
        #endregion
    }
}