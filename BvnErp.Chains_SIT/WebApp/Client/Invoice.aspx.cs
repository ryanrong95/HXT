using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using WebApp.App_Utils;
using System.Net;

namespace WebApp.Client
{
    public partial class Invoice : Uc.PageBase
    { 
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadData();   
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var deliveryType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceDeliveryType>().Select(item => new { item.Key, item.Value });
            this.Model.DeliveryType = deliveryType.Json();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;

            if (!string.IsNullOrEmpty(id))
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                var address = client.Company.Address;

                this.Model.ClientInfoData = new
                {
                    ID = client.ID,
                    CompanyName = client.Company.Name,
                    CompanyCode = client.Company.Code,
                    Address = client.Company.Address,
                    ContactName = client.Company.Contact.Name,
                    Email = client.Company.Contact.Email,
                    Tel = client.Company.Contact.Tel,
                    Mobile = client.Company.Contact.Mobile,
                }.Json();

                var invoice = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientInvoices.Where(t => t.ClientID == id && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                var invoiceConsignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientInvoiceConsignees.Where(t => t.ClientID == id).FirstOrDefault() ?? new Needs.Ccs.Services.Models.ClientInvoiceConsignee();
                if (invoice != null)
                {
                    this.Model.ClientInvoiceData = new
                    {
                        ID = invoice.ID,
                        Name = invoice.Title,
                        Taxpayer = invoice.TaxCode,
                        Address = invoice.Address,
                        Tel =string.IsNullOrEmpty(invoice.Tel)!=true? invoice.Tel: string.Empty,
                        BankName = invoice.BankName,
                        BankAccount = invoice.BankAccount,
                        DeliveryType = (int)invoice.DeliveryType,

                        ConsigneeName = invoiceConsignee.Name,
                        ConsigneeAddress = invoiceConsignee.Address,
                        ConsigneeMobile = invoiceConsignee.Mobile,
                        ConsigneeTel = invoiceConsignee.Tel,
                        ConsigneeEmail = invoiceConsignee.Email,
                        Summary = invoice.Summary
                    }.Json();
                }
                else
                {
                    this.Model.ClientInvoiceData = null;
                }
            }
            else
            {
                this.Model.ClientInvoiceData = null;
                this.Model.ClientInfoData = null;
            }
        }

        /// <summary>
        /// 保存会员发票信息
        /// </summary>
        protected void SaveClientInvoice()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            string clientid = model.ClientID;

            var invoice = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[clientid].Invoice ?? new Needs.Ccs.Services.Models.ClientInvoice();
            var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[clientid];

            var oldInvoice = (Needs.Ccs.Services.Models.ClientInvoice)invoice.Copy();

            var invoiceConsignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[clientid].InvoiceConsignee ?? new Needs.Ccs.Services.Models.ClientInvoiceConsignee();

            //发票信息
            invoice.ClientID = model.ClientID;
            invoice.Title = model.Name;
            invoice.TaxCode = model.Taxpayer;
            invoice.Address = model.Address;
            invoice.Tel = model.Tel;
            invoice.BankName = model.BankName;
            invoice.BankAccount = model.BankAccount;
            invoice.DeliveryType = (Needs.Ccs.Services.Enums.InvoiceDeliveryType)model.DeliveryTypeID;
            invoice.Summary = model.Summary;

            invoice.EnterError += Invoice_EnterError;
            invoice.EnterSuccess += Invoice_EnterSuccess;
            //发票收件地址
            invoiceConsignee.ClientID = model.ClientID;
            invoiceConsignee.Name = model.ConsigneeName;
            invoiceConsignee.Mobile = model.ConsigneeMobile;
            invoiceConsignee.Tel = model.ConsigneeTel;
            invoiceConsignee.Email = model.ConsigneeEmail;
            invoiceConsignee.Address = model.ConsigneeAddress;
             
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                invoiceConsignee.Enter();
                invoice.Enter(oldInvoice, client);
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/InvoiceEnter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var addressdata = new ApiAddressHelp().GetReceiver(invoiceConsignee.Address);
                    var entity = new ApiModel.ClientInvoice()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = client.Company.Corporate,
                            Name = client.Company.Name,
                            RegAddress = invoice.Address??client.Company.Address,
                            Uscc = client.Company.Code,
                            Status = 200
                        },
                        CompanyTel = invoice.Tel,
                        Type = 0,
                        Bank = invoice.BankName,
                        Account = invoice.BankAccount,
                        BankAddress ="",
                        TaxperNumber = invoice.TaxCode,
                        Name = model.ConsigneeName,
                        Tel = model.ConsigneeTel,
                        Mobile = model.ConsigneeMobile,
                        Email = model.ConsigneeEmail,
                        District = 1,
                        Province = addressdata.ProvinceName,
                        City =addressdata.CityName,
                        Land = addressdata.ExpAreaName,
                        Address = model.ConsigneeAddress,
                        InvoiceAddrss = invoice.Address,
                        Postzip = "",
                        DeliveryType = (int)model.DeliveryTypeID,
                        Status = 200,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName
                    };
                    string apiclient = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    invoiceConsignee.Enter();
                    invoice.Enter(oldInvoice, client);
                }
                catch (Exception e)
                {

                    throw e;
                }

                #endregion
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Invoice_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Invoice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Needs.Ccs.Services.Models.ClientInvoice clientInvoice = (Needs.Ccs.Services.Models.ClientInvoice)e.Object;
            var oldInvoice = clientInvoice.OldClientInvoice;

            StringBuilder sbSummary = new StringBuilder();
            sbSummary.Append("操作人员[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]编辑了发票信息.");


            //纳税人识别号
            if (oldInvoice.TaxCode != clientInvoice.TaxCode)
            {
                sbSummary.Append("纳税人识别号:从[" + oldInvoice.TaxCode + "]改为[" + clientInvoice.TaxCode + "],    ");
            }

            //地址
            if (oldInvoice.Address != clientInvoice.Address)
            {
                sbSummary.Append("地址:从[" + oldInvoice.Address + "]改为[" + clientInvoice.Address + "],    ");
            }

            //电话
            if (oldInvoice.Tel != clientInvoice.Tel)
            {
                sbSummary.Append("电话:从[" + oldInvoice.Tel + "]改为[" + clientInvoice.Tel + "],    ");
            }

            //开户行
            if (oldInvoice.BankName != clientInvoice.BankName)
            {
                sbSummary.Append("开户行:从[" + oldInvoice.BankName + "]改为[" + clientInvoice.BankName + "],    ");
            }

            //账号
            if (oldInvoice.BankAccount != clientInvoice.BankAccount)
            {
                sbSummary.Append("账号:从[" + oldInvoice.BankAccount + "]改为[" + clientInvoice.BankAccount + "]    ");
            }
             var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            clientInvoice.LogClient.Admin = admin;
            clientInvoice.Log(clientInvoice.LogClient, sbSummary.ToString().Trim(','));
            Response.Write((new { success = true, message = "保存成功", ID = clientInvoice.ID }).Json());
        }

        //发票收件地址智能解析
        public string NewAddress(string  address)
        {
            //var addressdata = new ApiAddressHelp().GetReceiver(address);
            return address;
        }
    }
}