using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 承兑汇票 视图
    /// </summary>
    public class MoneyOrdersRoll : vDepthView<MoneyOrder, MoneyOrderDto, PvFinanceReponsitory>
    {
        public MoneyOrdersRoll()
        {
        }

        public MoneyOrdersRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        public MoneyOrdersRoll(PvFinanceReponsitory reponsitory, IQueryable<MoneyOrder> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<MoneyOrder> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MoneyOrders>()
                   orderby entity.Status ascending, entity.CreateDate descending
                   select new MoneyOrder()
                   {
                       BankCode = entity.BankCode,
                       BankName = entity.BankName,
                       BankNo = entity.BankNo,
                       Code = entity.Code,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Currency = (Currency)entity.Currency,
                       EndDate = entity.EndDate,
                       ExchangeDate = entity.ExchangeDate,
                       ExchangePrice = entity.ExchangePrice,
                       ID = entity.ID,
                       Name = entity.Name,
                       PayerAccountID = entity.PayerAccountID,
                       Price = entity.Price,
                       IsTransfer = entity.IsTransfer,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       Nature = (MoneyOrderNature)entity.Nature,
                       PayeeAccountID = entity.PayeeAccountID,
                       StartDate = entity.StartDate,
                       Status = (MoneyOrderStatus)entity.Status,
                       Type = (MoneyOrderType)entity.Type,
                       IsMoney = entity.IsMoney,
                   };
        }

        protected override IEnumerable<MoneyOrderDto> OnMyPage(IQueryable<MoneyOrder> iquery)
        {
            return GetIEnumerable(item => true, iquery);
        }

        #region 查询
        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public MoneyOrdersRoll Search(Expression<Func<MoneyOrder, bool>> predicate)
        {
            return new MoneyOrdersRoll(this.Reponsitory, this.GetIQueryable().Where(predicate));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public RecordsDtoView SearchByOtherName(string name)
        //{
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        return this;
        //    }

        //    var clientView = new Yahv.Services.Views.ClientsAll<HvRFQReponsitory>(this.Reponsitory);
        //    var linq = from record in this.IQueryable
        //        join client in clientView on record.OtherID equals client.ID
        //        where client.Name.Contains(name)
        //        select record;
        //    return new RecordsDtoView(this.Reponsitory, linq);
        //}

        #endregion

        #region 索引器
        public MoneyOrder this[string id]
        {
            get { return this.GetIQueryable().Single(item => item.ID == id); }
        }
        #endregion

        #region 检查票据号码是否存在
        /// <summary>
        /// 检查票据号码是否已经存在
        /// </summary>
        /// <param name="code">票据号码</param>
        /// <returns></returns>
        public bool CheckCode(string code)
        {
            return this.GetIQueryable().Any(item => item.Code == code.Trim());
        }
        #endregion

        #region 获取数据

        public IEnumerable<MoneyOrderDto> GetIEnumerable(Expression<Func<MoneyOrder, bool>> predicate, IQueryable<MoneyOrder> iquery = null)
        {
            iquery = iquery ?? this.GetIQueryable();
            iquery = iquery.Where(predicate);
            IEnumerable<MoneyOrder> query = iquery.ToArray();

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
                   orderby entity.Status ascending, entity.CreateDate descending
                   select new MoneyOrderDto()
                   {
                       Name = entity.Name,
                       ID = entity.ID,
                       BankCode = entity.BankCode,
                       BankName = entity.BankName,
                       BankNo = entity.BankNo,
                       Code = entity.Code,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayeeAccountName = payeeAccount?.ShortName ?? payeeAccount?.Name,
                       PayerAccountID = entity.PayerAccountID,
                       Price = entity.Price,
                       PayerAccountName = payerAccount?.Name,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       CreatorName = admin.RealName,
                       StatusName = entity?.Status.GetDescription(),
                       Type = entity.Type,
                       IsTransfer = entity.IsTransfer,
                       Nature = entity.Nature,
                       IsMoney = entity.IsMoney,
                       Currency = entity.Currency,
                       ModifyDate = entity.ModifyDate,
                       ExchangeDate = entity.ExchangeDate,
                       ExchangePrice = entity.ExchangePrice,
                       ModifierID = entity.ModifierID,
                   };
        }

        public MoneyOrderDto Find(string id)
        {
            return GetIEnumerable(item => item.ID == id, this.IQueryable).FirstOrDefault();
        }

        public MoneyOrder FirstOrDefault(Expression<Func<MoneyOrder, bool>> predicate)
        {
            return this.GetIQueryable().FirstOrDefault(predicate);
        }
        #endregion
    }
}