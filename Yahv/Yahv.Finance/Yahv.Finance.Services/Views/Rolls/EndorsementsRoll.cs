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

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 背书
    /// </summary>
    public class EndorsementsRoll : vDepthView<Endorsement, EndorsementDto, PvFinanceReponsitory>
    {
        public EndorsementsRoll()
        {
        }

        public EndorsementsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        public EndorsementsRoll(PvFinanceReponsitory reponsitory, IQueryable<Endorsement> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Endorsement> GetIQueryable()
        {
            return new EndorsementsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public EndorsementsRoll Where(Expression<Func<Endorsement, bool>> predicate)
        {
            return new EndorsementsRoll(this.Reponsitory, this.GetIQueryable().Where(predicate));
        }

        protected override IEnumerable<EndorsementDto> OnMyPage(IQueryable<Endorsement> iquery)
        {
            var query = iquery.ToArray();

            var accountIds = query.Select(item => item.PayeeAccountID)
                .Concat(query.Select(item => item.PayerAccountID));
            var adminIds = query.Select(item => item.CreatorID).ToArray();


            var accounts = new AccountsOrigin(this.Reponsitory).Where(item => accountIds.Contains(item.ID));
            var admins = new AdminsTopView(this.Reponsitory).Where(item => adminIds.Contains(item.ID));

            return from entity in query
                   join payeeAccount in accounts on entity.PayeeAccountID equals payeeAccount.ID into _payeeAccount
                   from payeeAccount in _payeeAccount.DefaultIfEmpty()
                   join payerAccount in accounts on entity.PayerAccountID equals payerAccount.ID into _payerAccount
                   from payerAccount in _payerAccount.DefaultIfEmpty()
                   join admin in admins on entity.CreatorID equals admin.ID
                   orderby entity.CreateDate ascending
                   select new EndorsementDto()
                   {
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       CreatorName = admin.RealName,
                       EndorseDate = entity.EndorseDate,
                       ID = entity.ID,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayerAccountID = entity.PayerAccountID,
                       IsTransfer = entity.IsTransfer,
                       MoneyOrderID = entity.MoneyOrderID,
                       Summary = entity.Summary,
                       PayeeAccountName = payeeAccount?.ShortName ?? payeeAccount?.Name,
                       PayerAccountName = payerAccount?.ShortName ?? payerAccount?.Name,
                   };
        }

        #region 索引器
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moneyOrderId">汇票Id</param>
        /// <param name="payerAccountId">背书转让账户Id</param>
        /// <returns></returns>
        public Endorsement this[string moneyOrderId, string payerAccountId]
        {
            get
            {
                return this.GetIQueryable().FirstOrDefault(item => item.MoneyOrderID == moneyOrderId
                                                                   && item.PayerAccountID == payerAccountId);
            }
        }

        public override EndorsementDto this[string endorsId]
        {
            get { return this.OnMyPage(this.IQueryable.Where(item => item.ID == endorsId)).FirstOrDefault(); }
        }
        #endregion
    }
}