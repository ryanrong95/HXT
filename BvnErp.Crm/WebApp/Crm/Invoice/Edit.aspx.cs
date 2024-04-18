using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Invoice
{
    /// <summary>
    /// 开票信息编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.InvoiceType = EnumUtils.ToDictionary<InvoiceType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                string ClientID = Request.QueryString["ClientID"];
                var client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
                this.Model.ClientName = client.Name.Json();
                this.Model.Contact = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts.Where(item => item.ClientID == ClientID).Select(item => new { item.ID, item.Name }).Json();
                this.Model.Consignee = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees.Where(item => item.ClientID == ClientID).Select(item => new { ID = item.ID, text = item.Address + ";" + item.Contact.Name }).Json();
                LoadData();
            }
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var invoice = Needs.Erp.ErpPlot.Current.ClientSolutions.MyInvoices[id];
            if (invoice != null)
            {
                this.Model.Invoice = new
                {
                    InvoiceTypes = invoice.InvoiceTypes,
                    CompanyCode = invoice.CompanyCode,
                    Address = invoice.Address,
                    Phone = invoice.Phone,
                    BankName = invoice.BankName,
                    Account = invoice.Account,
                    ConsigneeID = invoice.Consignee.ID,
                    ZipCode = invoice.Consignee.Zipcode,
                    ContactName = invoice.Consignee.Contact.Name,
                    ContactPhone = invoice.Consignee.Contact.Mobile,

                }.Json();
            }
            else
            {
                this.Model.Invoice = "".Json();
            }
        }


        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            string CompanyID = Request.Form["CompanyID"];
            string ConsigneeID = Request.Form["Consignee"];

            var Invoice = Needs.Erp.ErpPlot.Current.ClientSolutions.MyInvoices[id] as NtErp.Crm.Services.Models.Invoice ?? new NtErp.Crm.Services.Models.Invoice();

            Invoice.Companys = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(CompanyID);
            Invoice.Consignee = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Consignee>.Create(ConsigneeID);

            Invoice.InvoiceTypes = (InvoiceType)Convert.ToInt16(Request.Form["InvoiceTypes"]);
            Invoice.ClientID = ClientID;
            Invoice.CompanyCode = Request.Form["CompanyCode"];
            Invoice.Address = Request.Form["Address"];
            Invoice.Phone = Request.Form["Phone"];
            Invoice.BankName = Request.Form["BankName"];
            Invoice.Account = Request.Form["Account"];
            Invoice.EnterSuccess += Invoice_EnterSuccess;
            Invoice.Enter();
        }


        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Invoice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 获取关联信息
        /// </summary>
        /// <returns></returns>
        protected string getPhoneAddress()
        {
            string value = "noResult";
            string id = Request.Form["ID"];
            var consignee = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees[id];
            if (consignee != null)
            {
                string name = consignee.Contact == null ? "" : consignee.Contact.Name;
                string moblie = consignee.Contact == null ? "" : consignee.Contact.Mobile;
                string zipcode = consignee.Zipcode;
                value = name + "," + moblie + "," + zipcode;
            }
            return value;
        }

    }
}