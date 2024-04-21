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

namespace Yahv.Csrm.WebApp.Crm.Credits
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            {
                this.Model.Catalog = Request.QueryString["cata"];
                this.Model.Payer = Request.QueryString["payer"];
                this.Model.Business = Request.QueryString["bus"];

                //if (Request.QueryString["cata"] == CatalogConsts.仓储费)
                if (Request.QueryString["payee"] == "香港畅运国际物流有限公司")
                {
                    this.Model.Currencies =
                        ExtendsEnum.ToDictionary<Currency>().Where(item => item.Value == Currency.USD.GetDescription()).Select(item => new { text = item.Value, value = item.Key });
                }
                else
                {
                    this.Model.Currencies =
                        ExtendsEnum.ToDictionary<Currency>().Where(item => item.Value == Currency.CNY.GetDescription()).Select(item => new { text = item.Value, value = item.Key });
                }

                this.Model.Currency = ExtendsEnum.ToDictionary<Currency>().FirstOrDefault(item => item.Value == Request.QueryString["cur"]).Key;
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
                    //currency = (Currency)int.Parse(Request.QueryString["currency"]),
                    currency = (Currency)int.Parse(Request.Form["Currency"]),
                    catalog = Request.QueryString["cata"],
                    business = Request.QueryString["bus"],
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
                Expression<Func<Yahv.Services.Models.FlowAccount, bool>> predicate = item => item.Type == AccountType.CreditRecharge
                    && item.Payer == Request.QueryString["payerId"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Payee == Request.QueryString["payeeId"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Business == Request.QueryString["bus"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Catalog == Request.QueryString["cata"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]
                    && item.Currency == (Currency)int.Parse(Request.QueryString["currency"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]);

                var query = Erp.Current.Crm.FlowAccounts.Where(predicate).ToList();


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
    }
}