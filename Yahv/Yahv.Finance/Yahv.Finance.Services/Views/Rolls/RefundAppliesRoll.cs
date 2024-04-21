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
    /// 退款申请
    /// </summary>
    public class RefundAppliesRoll : vDepthView<RefundApply, RefundApplyDto, PvFinanceReponsitory>
    {
        public RefundAppliesRoll()
        {
        }

        public RefundAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        public RefundAppliesRoll(PvFinanceReponsitory reponsitory, IQueryable<RefundApply> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<RefundApply> GetIQueryable()
        {
            return new RefundAppliesOrigin(this.Reponsitory);
        }

        protected override IEnumerable<RefundApplyDto> OnMyPage(IQueryable<RefundApply> iquery)
        {
            var query = iquery.ToArray();

            var accountIds = query.Select(item => item.PayeeAccountID)
                .Concat(query.Select(item => item.PayerAccountID)).ToArray();
            var accounts = new AccountsOrigin(this.Reponsitory).Where(item => accountIds.Contains(item.ID));

            var senders = new SendersOrigin(this.Reponsitory);

            var adminIds = query.Select(item => item.CreatorID)
                .Concat(query.Select(item => item.ApplierID))
                .Concat(query.Select(item => item.ExcuterID))
                .Concat(query.Select(item => item.ApproverID)).Distinct().ToArray();
            var admins = new AdminsTopView(this.Reponsitory).Where(item => adminIds.Contains(item.ID));

            var leftIds = query.Select(item => item.PayeeLeftID).ToArray();
            var lefts = new PayeeLeftsOrigin(this.Reponsitory).Where(item => leftIds.Contains(item.ID)).ToArray();



            var result = from entity in query
                         join sender in senders on entity.SenderID equals sender.ID
                         join payerAccount in accounts on entity.PayerAccountID equals payerAccount.ID into _payerAccount
                         from payerAccount in _payerAccount.DefaultIfEmpty()
                         join payeeAccount in accounts on entity.PayeeAccountID equals payeeAccount.ID
                         join applier in admins on entity.ApplierID equals applier.ID into _applier
                         from applier in _applier.DefaultIfEmpty()
                         join approver in admins on entity.ApproverID equals approver.ID into _approver
                         from approver in _approver.DefaultIfEmpty()
                         join excuter in admins on entity.ExcuterID equals excuter.ID into _excuter
                         from excuter in _excuter.DefaultIfEmpty()
                         join payeeLeft in lefts on entity.PayeeLeftID equals payeeLeft.ID into _payeeLeft
                         from payeeLeft in _payeeLeft.DefaultIfEmpty()
                         orderby entity.Status ascending, entity.CreateDate descending
                         select new RefundApplyDto()
                         {
                             AccountCatalogID = entity.AccountCatalogID,
                             ApplierID = entity.ApplierID,
                             ApproverID = entity.ApproverID,
                             CreateDate = entity.CreateDate,
                             CreatorID = entity.CreatorID,
                             Currency = entity.Currency,
                             ExcuterID = entity.ExcuterID,
                             FlowID = entity.FlowID,
                             ID = entity.ID,
                             Status = entity.Status,
                             PayeeAccountID = entity.PayeeAccountID,
                             Price = entity.Price,
                             PayerAccountID = entity.PayerAccountID,
                             PayeeLeftID = entity.PayeeLeftID,
                             SenderID = entity.SenderID,
                             Summary = entity.Summary,
                             Type = entity.Type,

                             SenderName = sender?.Name,
                             PayerAccountName = payerAccount?.ShortName ?? payerAccount?.Name,
                             PayeeAccountName = payeeAccount?.ShortName ?? payeeAccount?.Name,
                             ApplierName = applier?.RealName,
                             ExcuterName = excuter?.RealName,
                             ApproverName = approver?.RealName,
                             StatusName = GetStatusName(entity.Status, applier?.RealName, approver?.RealName, excuter?.RealName),
                             PayeeAccountCode = payeeAccount?.Code,
                             PayerAccountCode = payerAccount?.Code,
                             PayeeLeftAccountID = payeeLeft.AccountID,
                         };

            return result;
        }

        #region 查询
        public RefundAppliesRoll Where(Expression<Func<RefundApply, bool>> predicate)
        {
            return new RefundAppliesRoll(this.Reponsitory, this.IQueryable.Where(predicate));
        }

        /// <summary>
        /// 根据付款账户查询
        /// </summary>
        /// <param name="payerName"></param>
        /// <returns></returns>
        public RefundAppliesRoll SearchByPayerName(string payerName)
        {
            if (payerName.IsNullOrEmpty())
            {
                return this;
            }

            var accounts = new AccountsOrigin(this.Reponsitory);

            var linq = from entity in this.IQueryable
                       join account in accounts on entity.PayerAccountID equals account.ID
                       where account.Name.Contains(payerName) || account.ShortName.Contains(payerName)
                       select entity;

            return new RefundAppliesRoll(this.Reponsitory, linq);
        }

        public RefundApply FindRefundApply(string id)
        {
            return this.IQueryable.FirstOrDefault(item => item.ID == id);
        }
        #endregion

        #region 索引器

        public RefundApplyDto this[string id]
        {
            get { return this.OnMyPage(this.IQueryable.Where(item => item.ID == id)).FirstOrDefault(); }
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetStatusName(ApplyStauts status, string applier, string approver, string excuter)
        {
            string result = String.Empty;
            switch (status)
            {
                case ApplyStauts.Completed:
                    result = status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = !string.IsNullOrEmpty(applier) ? $"{status.GetDescription()}({applier})" : status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = !string.IsNullOrEmpty(excuter) ? $"{status.GetDescription()}({excuter})" : status.GetDescription();
                    break;
                default:
                    result = !string.IsNullOrEmpty(approver) ? $"{status.GetDescription()}({approver})" : status.GetDescription();
                    break;
            }
            return result;
        }
        #endregion
    }
}