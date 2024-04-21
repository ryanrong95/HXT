using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class PayeeLeftsRoll : QueryView<PayeeLeft, PvFinanceReponsitory>
    {
        public PayeeLeftsRoll()
        {
        }



        protected PayeeLeftsRoll(PvFinanceReponsitory reponsitory, IQueryable<PayeeLeft> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public PayeeLeftsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayeeLeft> GetIQueryable()
        {
            var payeeLeftsOrigin = new PayeeLeftsOrigin(this.Reponsitory);
            var flowAccountsOrigin = new FlowAccountsOrigin(this.Reponsitory);
            var accountsOrigin = new AccountsOrigin(this.Reponsitory);

            var iQuery = from payeeLeft in payeeLeftsOrigin
                         join flowAccount in flowAccountsOrigin on payeeLeft.FlowID equals flowAccount.ID
                         join account in accountsOrigin on payeeLeft.AccountID equals account.ID
                         where payeeLeft.Status == GeneralStatus.Normal
                         select new PayeeLeft
                         {
                             ID = payeeLeft.ID,
                             AccountCatalogID = payeeLeft.AccountCatalogID,
                             AccountID = payeeLeft.AccountID,
                             PayerName = payeeLeft.PayerName,
                             Currency = payeeLeft.Currency,
                             Price = payeeLeft.Price,
                             Currency1 = payeeLeft.Currency1,
                             ERate1 = payeeLeft.ERate1,
                             Price1 = payeeLeft.Price1,
                             CreatorID = payeeLeft.CreatorID,
                             CreateDate = payeeLeft.CreateDate,
                             FlowID = payeeLeft.FlowID,
                             Status = payeeLeft.Status,
                             Summary = payeeLeft.Summary,

                             FormCode = flowAccount.FormCode,
                             ReceiptDate = flowAccount.PaymentDate,
                             PayerNature = payeeLeft.PayerNature,

                             AccountCode = account.Code,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<PayeeLeft> iquery = this.IQueryable.Cast<PayeeLeft>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myPayeeLeft = iquery.ToArray();

            //AccountCatalogID
            var accountCatalogIDs = ienum_myPayeeLeft.Select(item => item.AccountCatalogID).Distinct();

            //AccountID
            var accountIDs = ienum_myPayeeLeft.Select(item => item.AccountID).Distinct();

            //创建人ID
            var creatorIds = ienum_myPayeeLeft.Select(item => item.CreatorID).Distinct();

            #region 收款类型

            var accountCatalogsOrigin = new AccountCatalogsOrigin(this.Reponsitory);

            var linq_accountCatalog = from accountCatalog in accountCatalogsOrigin
                                      where accountCatalog.Status == Underly.GeneralStatus.Normal
                                         && accountCatalogIDs.Contains(accountCatalog.ID)
                                      select new
                                      {
                                          AccountCatalogID = accountCatalog.ID,
                                          AccountCatalogName = accountCatalog.Name,
                                      };

            var ienums_accountCatalog = linq_accountCatalog.ToArray();

            #endregion

            #region 银行名称、银行账号

            var accountsOrigin = new AccountsOrigin(this.Reponsitory);

            var linq_account = from account in accountsOrigin
                               where account.Status == Underly.GeneralStatus.Normal
                                  && accountIDs.Contains(account.ID)
                               select new
                               {
                                   AccountID = account.ID,
                                   BankName = account.BankName,
                                   AccountCode = account.Code,
                                   ShortName = account.ShortName,
                               };

            var ienums_account = linq_account.ToArray();

            #endregion

            #region 创建人

            var applierAdminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_creator = from creator in applierAdminsTopView
                               where creatorIds.Contains(creator.ID)
                               select new
                               {
                                   ID = creator.ID,
                                   Name = creator.RealName,
                               };

            var ienums_creator = linq_creator.ToArray();

            #endregion

            var ienums_linq = from payeeLeft in ienum_myPayeeLeft
                              join accountCatalog in ienums_accountCatalog on payeeLeft.AccountCatalogID equals accountCatalog.AccountCatalogID into ienums_accountCatalog2
                              from accountCatalog in ienums_accountCatalog2.DefaultIfEmpty()
                              join account in ienums_account on payeeLeft.AccountID equals account.AccountID into ienums_account2
                              from account in ienums_account2.DefaultIfEmpty()
                              join creator in ienums_creator on payeeLeft.CreatorID equals creator.ID into _creator
                              from creator in _creator.DefaultIfEmpty()
                              select new PayeeLeft
                              {
                                  ID = payeeLeft.ID,
                                  AccountCatalogID = payeeLeft.AccountCatalogID,
                                  AccountID = payeeLeft.AccountID,
                                  PayerName = payeeLeft.PayerName,
                                  Currency = payeeLeft.Currency,
                                  Price = payeeLeft.Price,
                                  Currency1 = payeeLeft.Currency1,
                                  ERate1 = payeeLeft.ERate1,
                                  Price1 = payeeLeft.Price1,
                                  CreatorID = payeeLeft.CreatorID,
                                  CreateDate = payeeLeft.CreateDate,
                                  FlowID = payeeLeft.FlowID,
                                  Status = payeeLeft.Status,
                                  Summary = payeeLeft.Summary,

                                  FormCode = payeeLeft.FormCode,
                                  AccountCatalogName = accountCatalog != null ? accountCatalog.AccountCatalogName : "",
                                  BankName = account != null ? account.BankName : "",
                                  AccountCode = account != null ? account.AccountCode : "",
                                  PayerNature = payeeLeft.PayerNature,
                                  CreatorName = creator?.Name,
                                  AccountName = account?.ShortName,
                              };

            var results = ienums_linq.ToArray();

            Func<PayeeLeft, object> convert = item => new
            {
                PayeeLeftID = item.ID,
                AccountCatalogName = item.AccountCatalogName,
                FormCode = item.FormCode,
                BankName = item.BankName,
                AccountCode = item.AccountCode,
                CurrencyDes = item.Currency.GetDescription(),
                Price = item.Price.ToRound1(2),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.CreatorName,
                item.AccountName,
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据流水号查询
        /// </summary>
        /// <param name="formCode"></param>
        /// <returns></returns>
        public PayeeLeftsRoll SearchByFormCode(string formCode)
        {
            var linq = from query in this.IQueryable
                       where query.FormCode.Contains(formCode)
                       select query;

            var view = new PayeeLeftsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 AccountID 查询
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public PayeeLeftsRoll SearchByAccountID(string accountID)
        {
            var linq = from query in this.IQueryable
                       where query.AccountID == accountID
                       select query;

            var view = new PayeeLeftsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 创建人ID 查询
        /// </summary>
        /// <returns></returns>
        public PayeeLeftsRoll SearchByCreatorID(string creatorId)
        {
            var linq = from query in this.IQueryable
                       where query.CreatorID == creatorId
                       select query;

            var view = new PayeeLeftsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PayeeLeft this[string payeeLeftId]
        {
            get { return this.Single(item => item.ID == payeeLeftId); }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public void AddRange(IEnumerable<PayeeLeft> list)
        {
            if (list == null || !list.Any())
            {
                return;
            }

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var array = list.Select(item => new Layers.Data.Sqls.PvFinance.PayeeLefts()
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.PayeeLeft),
                    AccountCatalogID = item.AccountCatalogID,
                    AccountID = item.AccountID,
                    CreateDate = item.CreateDate,
                    CreatorID = item.CreatorID,
                    Currency = (int)item.Currency,
                    Currency1 = (int)item.Currency1,
                    ERate1 = item.ERate1,
                    FlowID = item.FlowID,
                    Summary = item.Summary,
                    PayerName = item.PayerName,
                    Price = item.Price,
                    PayerNature = (int)item.PayerNature,
                    Price1 = item.Price1,
                    Status = (int)item.Status,
                }).ToList();

                reponsitory.Insert((IEnumerable<Layers.Data.Sqls.PvFinance.PayeeLefts>)array);
            }
        }

        /// <summary>
        /// 获取汇票账户收款单据（可用余额大于0）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PayeeLeft> GetPayeeLeftsForMoneyOrder()
        {
            //收款原始视图
            var payeeLeftsOrigin = new PayeeLeftsOrigin(this.Reponsitory).ToArray();
            //汇票账户流水表 原始视图
            var flowAccountsOrigin = new FlowAccountsOrigin(this.Reponsitory)
                .Where(item => item.Type == FlowAccountType.MoneyOrder).ToArray();

            //汇票账户ID
            var flowMoIds = flowAccountsOrigin.Where(item => payeeLeftsOrigin.Select(p => p.FlowID)
                    .Contains(item.ID)).Select(item => item.MoneyOrderID).Distinct().ToArray();

            //汇票账户流水
            var flowAccounts = flowAccountsOrigin.Where(item => flowMoIds.Contains(item.MoneyOrderID)).ToArray();
            var flows = from f in flowAccounts
                        where f.Type == FlowAccountType.MoneyOrder
                        group f by f.MoneyOrderID into g
                        select new
                        {
                            MoneyOrderID = g.Key,
                            Balance = g.Sum(item => item.Price)
                        };

            //账户视图
            var accounts = new AccountsOrigin(this.Reponsitory)
                .Where(item => payeeLeftsOrigin.Select(p => p.AccountID).Contains(item.ID)).ToArray();

            var linq = from payeeLeft in payeeLeftsOrigin
                       join flowAccount in flowAccounts on payeeLeft.FlowID equals flowAccount.ID
                       join account in accounts on payeeLeft.AccountID equals account.ID
                       join flow in flows on flowAccount.MoneyOrderID equals flow.MoneyOrderID
                       where payeeLeft.Status == GeneralStatus.Normal && flow.Balance > 0
                       select new PayeeLeft
                       {
                           ID = payeeLeft.ID,
                           AccountCatalogID = payeeLeft.AccountCatalogID,
                           AccountID = payeeLeft.AccountID,
                           PayerName = payeeLeft.PayerName,
                           Currency = payeeLeft.Currency,
                           Price = payeeLeft.Price,
                           Currency1 = payeeLeft.Currency1,
                           ERate1 = payeeLeft.ERate1,
                           Price1 = payeeLeft.Price1,
                           CreatorID = payeeLeft.CreatorID,
                           CreateDate = payeeLeft.CreateDate,
                           FlowID = payeeLeft.FlowID,
                           Status = payeeLeft.Status,
                           Summary = payeeLeft.Summary,

                           FormCode = flowAccount.FormCode,
                           ReceiptDate = flowAccount.PaymentDate,
                           PayerNature = payeeLeft.PayerNature,

                           AccountCode = account.Code,
                           Balance = flow.Balance,
                           AccountName = account.ShortName ?? account.Name,
                       };

            return linq;
        }

        /// <summary>
        /// 获取银行账户收款单据（可用余额大于0）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PayeeLeft> GetPayeeLeftsForBank()
        {
            //收款原始视图
            var payeeLeftsOrigin = new PayeeLeftsOrigin(this.Reponsitory).ToArray();
            //银行账户 流水表 原始视图
            var flowAccountsOrigin = new FlowAccountsOrigin(this.Reponsitory)
                .Where(item => item.Type == FlowAccountType.BankStatement).ToArray();

            //流水号
            var formCodes = flowAccountsOrigin.Where(item => payeeLeftsOrigin.Select(p => p.FlowID)
                    .Contains(item.ID)).Select(item => item.FormCode).Distinct().ToArray();

            //银行账户流水
            var flowAccounts = flowAccountsOrigin.Where(item => formCodes.Contains(item.FormCode)).ToArray();
            var flows = from f in flowAccounts
                        where f.Type == FlowAccountType.BankStatement
                        group f by new { f.FormCode, f.AccountID } into g
                        select new
                        {
                            AccountID = g.Key.AccountID,
                            FormCode = g.Key.FormCode,
                            Balance = g.Sum(item => item.Price)
                        };

            //账户视图
            var accounts = new AccountsOrigin(this.Reponsitory)
                .Where(item => payeeLeftsOrigin.Select(p => p.AccountID).Contains(item.ID)).ToArray();

            var linq = from payeeLeft in payeeLeftsOrigin
                       join flowAccount in flowAccounts on payeeLeft.FlowID equals flowAccount.ID
                       join account in accounts on payeeLeft.AccountID equals account.ID
                       join flow in flows on flowAccount.FormCode equals flow.FormCode
                       where payeeLeft.Status == GeneralStatus.Normal && flow.Balance > 0
                       select new PayeeLeft
                       {
                           ID = payeeLeft.ID,
                           AccountCatalogID = payeeLeft.AccountCatalogID,
                           AccountID = payeeLeft.AccountID,
                           PayerName = payeeLeft.PayerName,
                           Currency = payeeLeft.Currency,
                           Price = payeeLeft.Price,
                           Currency1 = payeeLeft.Currency1,
                           ERate1 = payeeLeft.ERate1,
                           Price1 = payeeLeft.Price1,
                           CreatorID = payeeLeft.CreatorID,
                           CreateDate = payeeLeft.CreateDate,
                           FlowID = payeeLeft.FlowID,
                           Status = payeeLeft.Status,
                           Summary = payeeLeft.Summary,

                           FormCode = flowAccount.FormCode,
                           ReceiptDate = flowAccount.PaymentDate,
                           PayerNature = payeeLeft.PayerNature,

                           AccountCode = account.Code,
                           Balance = flow.Balance,
                           AccountName = account.ShortName ?? account.Name,
                       };

            return linq;
        }
    }
}
