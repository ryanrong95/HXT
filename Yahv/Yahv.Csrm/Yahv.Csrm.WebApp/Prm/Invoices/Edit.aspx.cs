using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Invoices
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ///下拉绑定
                //1.发票类型
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = new InvoicesRoll()[Request.QueryString["id"]];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string supplierid = Request.QueryString["supplierid"];
            var id = Request.QueryString["id"];
            var entity = new CompaniesRoll()[supplierid].Invoices[id] ?? new Invoice();
            entity.Bank = Request.Form["Bank"].Trim();
            entity.BankAddress = Request.Form["BankAddress"].Trim();
            entity.Account = Request.Form["Account"].Trim();
            entity.Type = (InvoiceType)int.Parse(Request["Type"]);
            entity.TaxperNumber = Request.Form["TaxperNumber"];
            entity.EnterpriseID = supplierid;
            entity.Address = Request.Form["Address"].Trim();
            entity.District = (District)int.Parse(Request["District"]);
            entity.Postzip = Request.Form["Postzip"].Trim();
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Invoice_EnterSuccess;
            entity.Enter();
            //}
            //catch (Exception ex)
            //{
            //    Easyui.Reload("提示", ex.Message, Yahv.Web.Controls.Easyui.Sign.Warning);
            //}
        }

        private void Invoice_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Invoice;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "InvoiceInsert", "新增发票：" + entity.ID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Crm),
                                        "InvoiceUpdate", "修改发票信息：" + entity.ID, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Dialog);
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}