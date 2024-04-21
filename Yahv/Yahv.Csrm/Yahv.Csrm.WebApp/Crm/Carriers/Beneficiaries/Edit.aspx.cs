using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Beneficiaries
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var carrier = Erp.Current.Crm.Carriers[Request.QueryString["carrierid"]];
                //this.Model.Enterprise = carrier.Enterprise;
                this.Model.Entity = carrier.Beneficiaries[Request.QueryString["benefitid"]];
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
                this.Model.InvoiceType = ExtendsEnum.ToArray(InvoiceType.Unkonwn).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var carrierid = Request.QueryString["carrierid"];
            var carrier = Erp.Current.Crm.Carriers[carrierid];
            string benefitid = Request.QueryString["benefitid"];
            var entity = carrier.Beneficiaries[Request.QueryString["benefitid"]] ?? new Beneficiary();
            entity.EnterpriseID = carrier.Enterprise.ID;
            entity.Enterprise = carrier.Enterprise;
            entity.Currency = (Currency)int.Parse(Request["Currency"]);
            entity.District = (District)int.Parse(Request["selDistrict"]);
            entity.Methord = (Methord)int.Parse(Request["Methord"]);
            entity.Bank = Request.Form["Bank"].Trim();
            entity.BankAddress = Request.Form["BankAddress"].Trim();
            entity.RealName = Request.Form["RealName"].Trim();
            entity.Account = Request.Form["Account"].Trim();
            entity.SwiftCode = Request.Form["SwiftCode"].Trim();
            entity.InvoiceType = (InvoiceType)int.Parse(Request["InvoiceType"]);
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            //entity.IsDefault = Request["IsDefault"] == null ? false : true;
            if (string.IsNullOrWhiteSpace(benefitid))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();


        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}