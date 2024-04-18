using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Consignees
{
    /// <summary>
    /// 收发货地址编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        #region 页面初始化
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.CustomerCompanyData = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.plot).OrderBy(item => item.Name).Json();
                string ClientID = Request.QueryString["ClientID"];
                this.Model.Contact = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts.Where(item => item.ClientID == ClientID).Select(item => new { item.ID, item.Name }).Json();
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var invoice = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees[id];
            if (invoice != null)
            {
                this.Model.Consignee = new
                {
                    CompanyID = invoice.CompanyID,
                    ContactID = invoice.Contact.ID,
                    ContactPhone = invoice.Contact.Mobile,
                    Address = invoice.Address,
                    Zipcode = invoice.Zipcode,
                }.Json();
            }
            else
            {
                this.Model.Consignee = invoice.Json();
            }
        }
        #endregion

        #region 保存
        /// <summary>
        /// 页面数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            string ContactID = Request.Form["ContactID"];

            if (!string.IsNullOrWhiteSpace(ContactID))
            {
                //联系人是选的，则不会修改联系人，新增或修改地址簿信息
                var Invoice = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees[id] as NtErp.Crm.Services.Models.Consignee ?? new NtErp.Crm.Services.Models.Consignee();

                Invoice.Contact = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Contact>.Create(ContactID);
                Invoice.CompanyID = Request.Form["CompanyID"];
                Invoice.ClientID = ClientID;
                Invoice.Address = Request.Form["Address"];
                Invoice.Zipcode = Request.Form["ZipCode"];
                Invoice.EnterSuccess += Invoice_EnterSuccess;
                Invoice.Enter();
            }
            else
            {
                //联系人是新增的，地址簿新增或修改在 Contact.EnterSuccess 里                 
                var Contact = new NtErp.Crm.Services.Models.Contact();
                Contact.Clients = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(ClientID);
                Contact.Mobile = Request.Form["ContactPhone"];
                Contact.Name = Request.Form["ContactName"];
                Contact.ClientID = ClientID;
                Contact.Types = ConsigneeType.Invoice;
                Contact.CompanyID = "";
                Contact.Email = "";
                Contact.Tel = "";
                Contact.Position = "";
                Contact.EnterSuccess += Contact_EnterSuccess;
                Contact.InvoiceEnter();
            }


        }

        /// <summary>
        /// 联系人保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            string id = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            string ContactID = e.Object;
            var Invoice = Needs.Erp.ErpPlot.Current.ClientSolutions.Consignees[id] as NtErp.Crm.Services.Models.Consignee ?? new NtErp.Crm.Services.Models.Consignee();

            Invoice.CompanyID = Request.Form["CompanyID"];
            Invoice.Contact = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Contact>.Create(ContactID);

            Invoice.ClientID = ClientID;
            Invoice.Address = Request.Form["Address"];
            Invoice.Zipcode = Request.Form["Zipcode"];
            Invoice.Status = NtErp.Crm.Services.Enums.Status.Normal;
            Invoice.Enter();

            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 发票信息保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Invoice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
        #endregion


        #region Ajax方法调用
        /// <summary>
        /// 根据选择联系人,带出联系方式
        /// </summary>
        /// <returns></returns>
        protected string getPhoneAddress()
        {
            string value = "noResult";
            string id = Request.Form["ID"];
            var Contacts = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts[id];
            if (Contacts != null)
            {
                value = Contacts.Mobile;
            }
            return value;
        }
        #endregion
    }
}