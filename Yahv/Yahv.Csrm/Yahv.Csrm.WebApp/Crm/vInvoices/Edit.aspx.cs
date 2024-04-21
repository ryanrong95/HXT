using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.vInvoices
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //下拉绑定
                //1.发票类型
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>(InvoiceType.Unkonwn, InvoiceType.None, InvoiceType.Customs).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.DeliveryType = ExtendsEnum.ToArray<InvoiceDeliveryType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                var client = Erp.Current.Whs.WsClients[Request.QueryString["EnterpriseID"]];
                this.Model.Client = new
                {
                    ID = client.ID,
                    Name = client.Enterprise.Name
                };
                this.Model.Entity = new vInvoicesRoll()[Request.QueryString["id"]];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string enterprieid = Request.QueryString["EnterpriseID"];
            bool ispersonal = bool.Parse(Request.Form["IsPersonal"]);
            InvoiceType type = (InvoiceType)int.Parse(Request.Form["Type"]);
            string title = Request.Form["Title"];
            string taxnumber = Request.Form["TaxNumber"];
            string regaddress = Request.Form["RegAddress"];
            string tel = Request.Form["Tel"];
            string bankname = Request.Form["BankName"];
            string bankaAccount = Request.Form["BankAccount"];
            string postaddress = Request.Form["PostAddress"];
            string postRrecipient = Request.Form["PostRrecipient"];
            string posttel = Request.Form["PostTel"];
            string postzipcode = Request.Form["PostZipCode"];
            InvoiceDeliveryType DeliveryType = (InvoiceDeliveryType)int.Parse(Request.Form["DeliveryType"]);
            bool IsDefault = Request.Form["IsDefault"] != null;
            vInvoice entity = new vInvoicesRoll()[Request.QueryString["id"]];
            entity.IsPersonal = ispersonal;
            if (!ispersonal)
            {
                entity.TaxNumber = taxnumber;
                entity.RegAddress = regaddress;
                entity.Tel = tel;
            }
            else
            {
                entity.TaxNumber = string.Empty;
                entity.RegAddress = string.Empty;
                entity.Tel = string.Empty;
            }
            entity.Type = type;
            entity.Title = title;
            entity.BankName = bankname;
            entity.BankAccount = bankaAccount;
            entity.PostAddress = postaddress;
            entity.PostRecipient = postRrecipient;
            entity.PostTel = posttel;
            entity.PostZipCode = postzipcode;
            entity.DeliveryType = DeliveryType;
            entity.IsDefault = IsDefault;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}