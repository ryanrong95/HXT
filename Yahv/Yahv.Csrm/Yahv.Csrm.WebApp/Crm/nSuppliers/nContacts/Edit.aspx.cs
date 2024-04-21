using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string clientid = Request.QueryString["clientid"];
                string supplierid = Request.QueryString["supplierid"];
                string id = Request.QueryString["contactid"];
                var supplier = Erp.Current.Whs.WsClients[clientid].nSuppliers[supplierid];
                this.Model.Entity = supplier.nContacts[id];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string clientid = Request.QueryString["clientid"];
            string supplierid = Request.QueryString["supplierid"];
            string contactid = Request.QueryString["contactid"];
            var client = Erp.Current.Whs.WsClients[clientid];
            if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.AutoAlert("请先规范客户名称", Web.Controls.Easyui.AutoSign.Warning);
                return;
            }
            var supplier = client.nSuppliers[supplierid];
            var contacts = supplier.nContacts;
            var contact = contacts[contactid] ?? new nContact();
            if (string.IsNullOrWhiteSpace(contactid))
            {
                contact.nSupplierID = supplierid;
                contact.EnterpriseID = clientid;
                contact.Creator = Erp.Current.ID;
                contact.RealID = supplier.RealID;
            }
            contact.Name = Request["Name"].Trim();
            contact.Tel = Request["Tel"].Trim();
            contact.Mobile = Request["Mobile"].Trim();
            contact.Email = Request["Email"].Trim();
            contact.QQ = Request["QQ"].Trim();
            contact.Fax = Request["Fax"].Trim();
            contact.EnterSuccess += Contact_EnterSuccess; ;
            contact.Enter();
        }

        private void Contact_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}