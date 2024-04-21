using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Payee
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1.币种
                this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //2.支付方式
                this.Model.Methord = ExtendsEnum.ToArray<Methord>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //3.所在地区
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //4.发票
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                string companyid = Request.QueryString["id"];
                string payeeid = Request.QueryString["payeeid"];
                Model.Entity = new CompaniesRoll()[companyid].Payees[payeeid];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string enterpriseid = Request.QueryString["id"];
            string payerid = Request.QueryString["payeeid"];
            var payee =new CompaniesRoll()[enterpriseid].Payees[payerid] ?? new YaHv.Csrm.Services.Models.Origins.Payee();

            string realName = Request["Enterprise"].Trim();//代收企业
            if (!string.IsNullOrEmpty(realName))
            {
                var enterprise = new Enterprise { Name = realName, AdminCode = "" };
                if (!enterprise.IsExist())
                {
                    enterprise.Enter();
                }
                payee.RealID = enterprise.ID;
            }
            if (string.IsNullOrWhiteSpace(payerid))
            {
                payee.EnterpriseID = enterpriseid;
                payee.Creator = Erp.Current.ID;
                payee.Repeat += Payee_Repeat;
            }
            payee.Place = Request["Place"];
            payee.Methord = (Methord)int.Parse(Request["Methord"]);
            payee.Currency = (Currency)int.Parse(Request["Currency"]);
            payee.Bank = Request["Bank"].Trim();
            payee.BankAddress = Request["BankAddress"].Trim();
            payee.Account = Request["Account"].Trim();
            payee.SwiftCode = Request["SwiftCode"].Trim();

            payee.Contact = Request["Name"].Trim();
            payee.Tel = Request["Tel"].Trim();
            payee.Mobile = Request["Mobile"].Trim();
            payee.Email = Request["Email"].Trim();
            payee.EnterSuccess += Payee_EnterSuccess;
            payee.Enter();
        }

        private void Payee_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Reload("提示", "收款人已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Payee_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}