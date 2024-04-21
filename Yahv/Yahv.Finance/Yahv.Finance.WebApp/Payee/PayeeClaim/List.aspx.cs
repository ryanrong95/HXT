using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payee.PayeeClaim
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region 功能函数
        protected object data()
        {
            var query = new AccountWorksView().Where(GetExpression()).ToArray();
            return this.Paging(query.OrderBy(item => item.ClaimantName)
                    .ThenByDescending(item => item.CreateDate),
                item => new
                {
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.ClaimantName,
                    item.AccountCatalogName,
                    item.AccountCode,
                    item.BankName,
                    item.Company,
                    CurrencyDes = item.Currency.GetDescription(),
                    item.PayeeLeftID,
                    item.FormCode,
                    item.Price,
                    item.PayerName,
                    item.ID,
                });
        }


        #endregion

        #region 私有函数
        private Expression<Func<AccountWorkDto, bool>> GetExpression()
        {
            Expression<Func<AccountWorkDto, bool>> predicate = item => item.ClaimantName == null || item.ClaimantName == "";

            string code = Request.QueryString["s_formcode"];
            var payee = Request.QueryString["s_payee"];
            var payer = Request.QueryString["s_payer"];

            //流水号
            if (!string.IsNullOrEmpty(code))
            {
                predicate = predicate.And(item => item.FormCode.Contains(code));
            }

            if (!string.IsNullOrEmpty(payee))
            {
                predicate = predicate.And(item => item.AccountShortName.Contains(payee) || item.AccountEnterprise.Contains(payee));
            }

            if (!string.IsNullOrEmpty(payer))
            {
                predicate = predicate.And(item => item.PayerName.Contains(payer));
            }

            return predicate;
        }

        #endregion
    }
}