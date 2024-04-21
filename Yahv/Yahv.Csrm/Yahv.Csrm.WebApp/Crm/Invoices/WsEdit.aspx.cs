using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Invoices
{
    public partial class WsEdit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //下拉绑定
                //1.发票类型
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>(InvoiceType.Unkonwn, InvoiceType.Normal, InvoiceType.None).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.DeliveryType = ExtendsEnum.ToArray<InvoiceDeliveryType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
                this.Model.Entity = wsclient.Invoice;
                this.Model.NotShowBtnSave = Erp.Current.Role.ID == FixedRole.ServiceManager.GetFixedID() && wsclient.WsClientStatus == ApprovalStatus.Normal;
                this.Model.Nonstandard = wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase);
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string clientid = Request.QueryString["id"];
            var wsclient = Erp.Current.Whs.WsClients[clientid];
            if (wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Alert("提示!", "请先规范客户名称", Web.Controls.Easyui.Sign.None);
            }
            var entity = new WsInvoice();

            entity.CompanyTel = Request.Form["CompanyTel"];
            entity.Bank = Request.Form["Bank"].Trim();
            entity.BankAddress = Request.Form["BankAddress"].Trim();
            entity.Account = Request.Form["Account"].Trim();
            entity.Type = (InvoiceType)int.Parse(Request["Type"]);
            entity.TaxperNumber = Request.Form["TaxperNumber"];

            entity.Address = Request.Form["Address"].Trim();
            entity.District = (District)int.Parse(Request["District"]);
            entity.Postzip = Request.Form["Postzip"].Trim();
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.DeliveryType = (InvoiceDeliveryType)int.Parse(Request["DeliveryType"]);
            entity.EnterpriseID = clientid;
            entity.Enterprise = wsclient.Enterprise;
            entity.IsDefault = true;
            entity.InvoiceAddress = Request["InvoiceAddress"].Trim();
            if (wsclient.Invoice == null)
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            else
            {
                entity.CreatorID = wsclient.Invoice.Creator == null ? "" : wsclient.Invoice.Creator.ID;
            }
            entity.EnterSuccess += Invoice_EnterSuccess;

            //try
            //{
            entity.Enter();
            //}
            //catch (Exception exc)
            //{
            //    throw exc;
            //}
            //}
            //catch (Exception ex)
            //{
            //    Easyui.Reload("提示", ex.Message, Yahv.Web.Controls.Easyui.Sign.Warning);
            //}
        }

        private void Invoice_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (apiurl != null)
            {
                var data = sender as WsInvoice;
                var client = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
                Unify(apiurl, client.Enterprise, data);
            }
            Easyui.Reload("提示!", "保存成功", Web.Controls.Easyui.Sign.None);
        }

        object Unify(string api, Enterprise client, WsInvoice data)
        {
            var json = new
            {
                Enterprise = client,
                Bank = data.Bank,
                CompanyTel = data.CompanyTel,
                BankAddress = data.BankAddress,//发票地址
                TaxperNumber = data.TaxperNumber,
                Account = data.Account,
                DeliveryType = data.DeliveryType,
                ConsigneeAddress = data.Address,
                InvoiceAddress = data.InvoiceAddress,
                Name = data.Name,
                Tel = data.Tel,
                Email = data.Email,
                Mobile = data.Mobile,
                Postzip = data.Postzip,
                Summary = ""
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(api + "/clients/invoice", json);
            return response;
        }


    }
}