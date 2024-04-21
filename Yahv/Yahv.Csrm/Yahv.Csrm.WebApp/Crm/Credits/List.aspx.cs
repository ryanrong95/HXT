using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Payments;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Models.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Credits
{
    public partial class List : BasePage
    {
        private string companyID = "8C7BF4F7F1DE9F69E1D96C96DAF6768E";      //香港畅运国际物流有限公司

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Subjects = SubjectManager.Current[ConductConsts.供应链].ToList();        //业务下的分类科目列表
                this.Model.client = Erp.Current.Crm.Enterprises[Request.QueryString["id"]].Name;
                this.Model.conduct = SubjectCollection.Current.Keys.Select(item => new { text = item, value = item });
                this.Model.company = Erp.Current.Crm.Enterprises.Where(item => item.ID == Request.QueryString["CompanyID"]).Select(item => new
                {
                    text = item.Name,
                    value = item.ID,
                });
            }
        }

        protected object data()
        {
            var payee = Request.QueryString["company"];
            var business = Request.QueryString["conduct"];

            //分类、币种列表
            var catalogCurrencyList = (from catalog in SubjectManager.Current[business]
                                       from currency in ExtendsEnum.ToArray<Currency>()
                                           //.Where(item => item == Currency.CNY)
                                       select new
                                       {
                                           Catalog = catalog.Name,
                                           Currency = currency
                                       }).ToList();

            //从数据库读取信用数据
            var list =
                Erp.Current.Crm.Credits.Where(
                    item => item.Payer == Request.QueryString["id"] && item.Business == business && item.Payee == payee)
                    .ToList();

            var currCurrency = Currency.CNY;
            //如果是香港畅运，只显示美元
            if (payee == companyID)
            {
                currCurrency = Currency.USD;
            }

            return from c in catalogCurrencyList
                   join l in list on new { c.Catalog, c.Currency } equals new { l.Catalog, l.Currency } into joinList
                   from l in joinList.DefaultIfEmpty()
                   where c.Currency == currCurrency || (l?.Total ?? 0) > 0
                   select new
                   {
                       Catalog = c.Catalog,
                       Total = l?.Total ?? 0,
                       Cost = l?.Cost ?? 0,
                       Currency = c.Currency,
                       CurrencyName = c.Currency.GetDescription(),
                   };
        }

        /// <summary>
        /// 该分类下是否有信用
        /// </summary>
        /// <returns></returns>
        protected object isHaveCredit()
        {
            string payer = Request.QueryString["payer"];
            string business = Request.QueryString["business"];
            string catalog = Request.QueryString["catalog"];

            using (var view = Erp.Current.Crm.FlowAccounts)
            {
                if (view.Any(item =>
                            item.Type == AccountType.CreditRecharge
                            && item.Business == business
                            && item.Catalog == catalog
                            && item.Payer == payer))
                {
                    return true;
                }
            }

            return false;
        }
    }
}