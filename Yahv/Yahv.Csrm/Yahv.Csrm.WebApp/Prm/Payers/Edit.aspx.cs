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

namespace Yahv.Csrm.WebApp.Prm.Payers
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
                string id = Request.QueryString["id"];
                string payerid = Request.QueryString["payerid"];
                var company = Erp.Current.Crm.Companies[id];
                Model.Entity = company.Payers[payerid];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string enterpriseid = Request.QueryString["id"];
            string payerid = Request.QueryString["payerid"];
            var payer = Erp.Current.Crm.Companies[enterpriseid].Payers[payerid] ?? new Payer();

            string realName = Request["Enterprise"].Trim();//代付企业
            if (!string.IsNullOrEmpty(realName))
            {
                var enterprise = new Enterprise { Name = realName, AdminCode = "" };
                if (!enterprise.IsExist())
                {
                    enterprise.Enter();
                }
                payer.RealID = enterprise.ID;
            }
            if (string.IsNullOrWhiteSpace(payerid))
            {
                payer.EnterpriseID = enterpriseid;
                payer.Creator = Erp.Current.ID;
                payer.Repeat += Payer_Repeat;
            }
            payer.Place = Request["Place"];
            payer.Methord = (Methord)int.Parse(Request["Methord"]);
            payer.Currency = (Currency)int.Parse(Request["Currency"]);
            payer.Bank = Request["Bank"].Trim();
            payer.BankAddress = Request["BankAddress"].Trim();
            payer.Account = Request["Account"].Trim();
            payer.SwiftCode = Request["SwiftCode"].Trim();
            payer.Contact = Request["Name"].Trim();
            payer.Tel = Request["Tel"].Trim();
            payer.Mobile = Request["Mobile"].Trim();
            payer.Email = Request["Email"].Trim();
            //payer.Place = Request["Place"];
            payer.EnterSuccess += Payer_EnterSuccess;
            payer.Enter();
        }
        private void Payer_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Reload("提示", "收款人已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }
        private void Payer_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}