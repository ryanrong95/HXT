using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Payments;
using Yahv.Payments.Models.Origins;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.CreditsGrant
{
    public partial class Edit : BasePage
    {
        private string catalog = CatalogConsts.仓储服务费;
        private string business = ConductConsts.供应链;
        Currency currency = Currency.USD;


        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            {
                this.Model.Catalog = catalog;
                this.Model.Payer = Request.QueryString["payer"];
                this.Model.Business = business;
                this.Model.Currencies = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Value == currency.GetDescription()).Select(item => new { text = item.Value, value = item.Key });
                this.Model.Currency = ExtendsEnum.ToDictionary<Currency>().FirstOrDefault(item => item.Value == currency.GetDescription()).Key;

                var flows = Erp.Current.Crm.FlowAccounts?.Where(GetPredicate());
                if (flows != null && flows.Any())
                {
                    this.Model.Total = flows.Sum(item => item.Price);
                }
                else
                {
                    this.Model.Total = 0;
                }
            }
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (Request.QueryString["isShow"] != "1")
            {
                var entity = new
                {
                    payerId = Request.QueryString["payerId"], //客户
                    payeeId = Request.QueryString["payeeId"], //内部公司
                    currency = currency,
                    catalog = catalog,
                    business = business,
                    price = decimal.Parse(Request.Form["Price"]),
                    adminId = Erp.Current.ID,
                };

                var result = PaymentManager.Erp(Erp.Current.ID)[entity.payerId, entity.payeeId][entity.business]
                     .Credit[entity.catalog]
                     .Credit(entity.currency, entity.price);

                if (string.IsNullOrWhiteSpace(result))
                {
                    Erp.Current.Oplog(Request.Url.ToString(), nameof(Systematic.Crm), "代仓储客户列表", "添加信用", entity.Json());
                    Easyui.Dialog.Close("保存成功", AutoSign.Success);
                }
                else
                {
                    Easyui.Alert("操作提示", result, Sign.Error);
                }
            }
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            try
            {
                var query = Erp.Current.Crm.FlowAccounts.Where(GetPredicate()).ToList();

                return from entity in query
                       where entity.Type == AccountType.CreditRecharge
                       orderby entity.CreateDate descending
                       select new
                       {
                           CreateDate = entity.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           entity.FormCode,
                           entity.Price,
                           CurrencyName = entity.Currency.GetDescription(),
                           entity.Business,
                           entity.Catalog,
                           entity.Subject,
                           Project = entity.Catalog ?? entity.Subject,
                           AdminName = Erp.Choice[entity.AdminID].RealName,
                           entity.ID,
                           entity.Currency,
                       };
            }
            catch (Exception)
            {
                return null;
            }
        }

        Expression<Func<Yahv.Services.Models.FlowAccount, bool>> GetPredicate()
        {
            return item => item.Type == AccountType.CreditRecharge
                    && item.Payer == Request.QueryString["payerId"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Payee == Request.QueryString["payeeId"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Business == business
                    && item.Catalog == catalog
                    && item.Currency == currency;
        }
    }
}