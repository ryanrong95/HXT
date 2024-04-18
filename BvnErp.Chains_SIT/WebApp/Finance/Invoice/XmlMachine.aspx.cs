using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.HttpUtility;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class XmlMachine : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadCombobox();
            }
        }

        protected void loadCombobox()
        {
            this.Model.ExpressCompanyData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DyjExpressCompany>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void ExpressSelect()
        {
            //快递公司ID
            string id = Request.Form["ID"];
            //快递方式
            if (id == "0")
            {
                Response.Write(EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.SFType>().Select(item => new { Value = item.Key, Text = item.Value }).Json());
            }
            else
            {
                Response.Write(EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.EMSType>().Select(item => new { Value = item.Key, Text = item.Value }).Json());
            }                      
        }

        protected void data()
        {
            string CleanIDs = Request.QueryString["InvoiceNoticeIDs"];

            string[] CleanIDs_Array = { };
            if (!string.IsNullOrEmpty(CleanIDs))
            {
                CleanIDs_Array = CleanIDs.Split(',');
            }

            using (var query = new Needs.Ccs.Services.Views.InvoiceXmlSplitShowView())
            {
                var view = query;

                view = view.SearchByInvoiceNoticeIDs(CleanIDs_Array);
               
                Response.Write(view.ToMyPage(1, 500).Json());
            }
        }

        protected void PostInvoice()
        {
            string requestID = Request["IDs"];
            var IDs = Request["IDs"].Replace("&quot;", "").Trim().Split(',');
            List<string> InvoiceNotices = new List<string>();
            for (int i = 0; i < IDs.Length; i++)
            {
                if (!InvoiceNotices.Contains(IDs[i]))
                {
                    InvoiceNotices.Add(IDs[i]);
                }
            }
            int ExpressCompany = Convert.ToInt16(Request["ExpressCompany"]);
            int ExpressType = Convert.ToInt16(Request["ExpressType"]);
            try
            {
                foreach (var item in InvoiceNotices)
                {
                    if (item != "合计")
                    {
                        XmlGeneRequestModel requestModel = new XmlGeneRequestModel(item, ExpressCompany, ExpressType);
                        InvoiceXmlRequestModel invoiceXmlRequestModel = new InvoiceXmlRequestModel();
                        invoiceXmlRequestModel.request_service = InvoiceApiSetting.ServiceName;
                        invoiceXmlRequestModel.request_item = InvoiceApiSetting.XmlRequestItem;
                        invoiceXmlRequestModel.data = requestModel;

                        string URL = System.Configuration.ConfigurationManager.AppSettings[InvoiceApiSetting.ApiName];
                        string requestUrl = URL + InvoiceApiSetting.GenerateXmlUrl;
                        string apiclient = JsonConvert.SerializeObject(invoiceXmlRequestModel);

                        DeliveryNoticeApiLog apiLog = new DeliveryNoticeApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            OrderID = item,
                            TinyOrderID = item,
                            Url = requestUrl,
                            RequestContent = apiclient,
                            ResponseContent = "",
                            Status = Needs.Ccs.Services.Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                        };
                        apiLog.Enter();

                        InvoiceNoticeLog log = new InvoiceNoticeLog();
                        log.InvoiceNoticeID = item;
                        log.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID); 
                        log.Status = Needs.Ccs.Services.Enums.InvoiceNoticeStatus.Confirmed;
                        log.Summary = "财务[" + log.Admin.RealName + "]导入了开票机";
                        log.Enter();

                        HttpResponseMessage response = new HttpResponseMessage();
                        response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);

                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Response.Write((new { success = false, message = "请求开票失败" }).Json());
                        }

                        var invoiceNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(t => t.ID == item).FirstOrDefault();
                        invoiceNotice.UpdateAuditing();

                    }
                }

                Response.Write((new { success = true, message = "请求成功" }).Json());
            }
            catch(Exception ex)
            {
                ex.CcsLog("请求dyj开票出错"+ requestID);
                Response.Write((new { success = false, message = "请求开票失败"}).Json());
            }            
        }
    }
}