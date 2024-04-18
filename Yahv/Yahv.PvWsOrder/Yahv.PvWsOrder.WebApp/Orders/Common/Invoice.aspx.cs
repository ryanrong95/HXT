using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class Invoice : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            var orderId = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.OrderAlls().FirstOrDefault(o => o.ID == orderId);
            if (query.OrderClient.StorageType != WsIdentity.Personal)
            {
                this.Model.Invoice = new
                {
                    InvoiceType = query.Invoice?.Type.GetDescription(),
                    DeliveryType = query.Invoice?.DeliveryType.GetDescription(),
                    CompanyName = query.Invoice?.CompanyName,
                    CompanyTel = query.Invoice?.CompanyTel,
                    TaxperNumber = query.Invoice?.TaxperNumber,
                    RegAddress = query.Invoice?.RegAddress,
                    Bank = query.Invoice?.Bank,
                    Account = query.Invoice?.Account,
                    BankAddress = query.Invoice?.BankAddress,
                    Name = query.Invoice?.Name,
                    Tel = query.Invoice?.Tel,
                    Mobile = query.Invoice?.Mobile,
                    Email = query.Invoice?.Email,
                    Address = query.Invoice?.Address,
                };
            }
            else
            {
                this.Model.Invoice = new
                {
                    InvoiceType = query.vInvoice?.Type.GetDescription(),
                    DeliveryType = query.vInvoice?.DeliveryType.GetDescription(),
                    CompanyName = query.vInvoice?.Title,
                    CompanyTel = query.vInvoice?.Tel,
                    TaxperNumber = query.vInvoice?.TaxNumber,
                    RegAddress = query.vInvoice?.RegAddress,
                    Bank = query.vInvoice?.BankName,
                    Account = query.vInvoice?.BankAccount,
                    //BankAddress = query.vInvoice?.BankAddress,
                    Name = query.vInvoice?.PostRecipient,
                    Tel = query.vInvoice?.Tel,
                    //Mobile = query.vInvoice?.Mobile,
                    //Email = query.vInvoice?.Email,
                    Address = query.vInvoice?.PostAddress,
                };
            }
        }
    }
}