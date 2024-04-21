using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;

namespace Yahv.Csrm.WebApp.Crm.DebtTerms
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Data =
                    PaymentManager.Erp(Erp.Current.ID)[Request.QueryString["payerId"], Request.QueryString["payeeId"]][
                        Request.QueryString["bus"]].DebtTerm[Request.QueryString["cata"]];

                this.Model.SettlementType = ExtendsEnum.ToDictionary<SettlementType>().Select(item => new { value = item.Key, text = item.Value });
                this.Model.ExchangeType = ExtendsEnum.ToDictionary<ExchangeType>().Select(item => new { value = item.Key, text = item.Value });
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var param = new
                {
                    payerId = Request.QueryString["payerId"], //客户
                    payeeId = Request.QueryString["payeeId"], //内部公司
                    catalog = Request.QueryString["cata"],
                    business = Request.QueryString["bus"],
                    adminId = Erp.Current.ID,
                };

                var model =
                    PaymentManager.Erp(Erp.Current.ID)[param.payerId, param.payeeId][param.business].DebtTerm[param.catalog] ?? new DebtTerm();

                var eRateType = (ExchangeType)int.Parse(Request.Form["ExchangeType"]);
                var settlementType = (SettlementType)int.Parse(Request.Form["SettlementType"]);
                var months = int.Parse(Request.Form["Months"]);
                var days = int.Parse(Request.Form["Days"]);

                if (!string.IsNullOrWhiteSpace(model.ID))
                {
                    //如果没有修改，则不添加数据
                    if (model.ExchangeType == eRateType && model.SettlementType == settlementType && model.Months == months &&
                        model.Days == days)
                    {
                        Easyui.Dialog.Close("添加成功!", AutoSign.Success);
                        return;
                    }
                }

                //var model = new DebtTerm()
                //{
                //    AdminID = param.adminId,
                //    Business = param.business,
                //    Payer = param.payerId,
                //    Payee = param.payeeId,
                //    Catalog = param.catalog,
                //};


                model.ExchangeType = eRateType;
                model.SettlementType = settlementType;
                model.Months = months;
                model.Days = days;

                model.Enter();

                Easyui.Dialog.Close("添加成功!", AutoSign.Success);
                Erp.Current.Oplog(Request.Url.ToString(), nameof(Systematic.Crm), "代仓储客户列表", "设置账期", model.Json());
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", $"添加失败!{ex.Message}", Sign.Error);
            }
        }
    }
}