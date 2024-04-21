using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Extends;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.ReportForm.ReceivePayment
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.GoldStores = new GoldStoresRoll().Select(item => new
                {
                    text = item.Name,
                    value = item.Name,
                }).ToArray();

                //付款账户 内部公司
                var accounts = GetMyAccounts();
                this.Model.PayerAccounts = accounts.Where(item => (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
                    .Select(item => new
                    {
                        item.ID,
                        ShortName = item.ShortName ?? item.Name,
                        CompanyName = item?.Enterprise?.Name,
                        item.BankName,
                        Currency = item.Currency.GetDescription(),
                        CurrencyID = (int)item.Currency,
                        item.Code,
                    });
            }
        }

        protected object data()
        {
            var query = new ReceivePaymentView().Where(GetExpression()).ToArray();
            string summary = GetSummary(query);
            return this.Paging(query.OrderBy(item => item.CreateDate), item => new
            {
                Currency = item.Currency.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                AccountMethord = item.AccountMethord.GetDescription(),
                item.AccountName,
                item.EnterpriseName,
                item.Target,
                item.Amount,
                Balance = item.Balance ?? 0,
                item.CreatorName,
                item.GoldStore,
                Summary = summary,
            });
        }

        private Expression<Func<ReceivePaymentDto, bool>> GetExpression()
        {
            Expression<Func<ReceivePaymentDto, bool>> predicate = item => true;

            if (!Erp.Current.IsSuper)
            {
                var myaccounts = GetMyAccounts().Select(item => item.ID).ToArray();
                predicate = predicate.And(item => myaccounts.Contains(item.AccountID));
            }

            var begin = Request.QueryString["s_begin"];
            var end = Request.QueryString["s_end"];
            var gold = Request.QueryString["s_goldStore"];
            var accountId = Request.QueryString["s_account"];

            //创建日期
            if (!string.IsNullOrEmpty(begin))
            {
                predicate = predicate.And(item => item.CreateDate >= DateTime.Parse(begin));
            }
            if (!string.IsNullOrEmpty(end))
            {
                predicate = predicate.And(item => item.CreateDate < DateTime.Parse(end + " 23:59:59"));
            }

            //金库
            if (!string.IsNullOrEmpty(gold))
            {
                predicate = predicate.And(item => item.GoldStore == gold);
            }

            //账户
            if (!string.IsNullOrEmpty(accountId))
            {
                predicate = predicate.And(item => item.AccountID == accountId);
            }

            return predicate;
        }

        /// <summary>
        /// 获取我的账户
        /// </summary>
        /// <returns></returns>
        private IQueryable<Account> GetMyAccounts()
        {
            var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID != null && item.NatureType == NatureType.Public);

            if (!Erp.Current.IsSuper)
            {
                accounts = accounts.Where(item => item.OwnerID == Erp.Current.ID);
            }

            return accounts;
        }

        /// <summary>
        /// 计算汇总
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private string GetSummary(ReceivePaymentDto[] query)
        {
            string summary = string.Empty;      //汇总

            //是否选择了单个账号，选择单个账号的话，显示余额
            string accountId = Request.QueryString["s_account"];

            var inQuery = from q in query
                          where q.Amount > 0
                          group q by new { q.Currency } into g
                          select new
                          {
                              Currency = g.Key.Currency,
                              Amount = g.Sum(item => item.Amount),
                              Balance = g.LastOrDefault().Balance,
                          };

            var outQuery = from q in query
                           where q.Amount < 0
                           group q by new { q.Currency } into g
                           select new
                           {
                               Currency = g.Key.Currency,
                               Amount = g.Sum(item => item.Amount),
                               Balance = g.LastOrDefault().Balance,
                           };

            if (inQuery != null && inQuery.Any())
            {
                foreach (var q in inQuery)
                {
                    if (summary.IsNullOrEmpty())
                    {
                        summary += $"收入：{q.Currency.GetCurrency().ShortSymbol}{q.Amount.ToString("N").TrimEnd('0')}";
                    }
                    else
                    {
                        summary += $"&nbsp;&nbsp;&nbsp;&nbsp;{q.Currency.GetCurrency().ShortSymbol}{q.Amount.ToString("N").TrimEnd('0')}";
                    }


                }
            }

            if (outQuery != null && outQuery.Any())
            {
                foreach (var q in outQuery)
                {
                    if (!summary.Contains("支出"))
                    {
                        summary += $"&nbsp;&nbsp;&nbsp;&nbsp;支出：{q.Currency.GetCurrency().ShortSymbol}{(-q.Amount).ToString("N").TrimEnd('0')}";
                    }
                    else
                    {
                        summary += $"&nbsp;&nbsp;&nbsp;&nbsp;{q.Currency.GetCurrency().ShortSymbol}{(-q.Amount).ToString("N").TrimEnd('0')}";
                    }

                }
            }

            if (query.Length > 0 && !accountId.IsNullOrEmpty())
            {
                var last = query.LastOrDefault(item => item.AccountID == accountId);
                if (last != null)
                {
                    summary += $"&nbsp;&nbsp;&nbsp;&nbsp;余额：{last.Currency.GetCurrency().ShortSymbol}{last.Balance?.ToString("N").TrimEnd('0')}";
                }
            }

            return summary;
        }
    }
}