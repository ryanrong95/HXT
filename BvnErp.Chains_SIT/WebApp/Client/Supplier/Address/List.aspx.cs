using Needs.Ccs.Services.Models;
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
using WebApp.App_Utils;

namespace WebApp.Client.Supplier.Address
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["SupplierID"];
            this.Model.ID = id;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["SupplierID"];
            string ContactName = Request.QueryString["ContactName"];
            string Mobile = Request.QueryString["Mobile"];
            string Address = Request.QueryString["Address"];

            var addresses = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses.AsQueryable();
            addresses = addresses.Where(t => t.ClientSupplierID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);

            Func<Needs.Ccs.Services.Models.ClientSupplierAddress, object> convert = address => new
            {
                address.ID,
                ContactName = address.Contact.Name,
                Mobile = address.Contact.Mobile,
                Address = address.Address,
                CreateDate = address.CreateDate.ToString(),
                Status = address.Status.GetDescription(),
                IsDefault = address.IsDefault ? "是" : "否",
                Summary = address.Summary,
                Place=address.Place
            };
            this.Paging(addresses, convert);
        }

        /// <summary>
        /// 删除提货地址
        /// </summary>
        protected void DeleteClientSupplierAddress()
        {
            //string ids = Request.Form["ID"];
            //ids.Split(',').ToList().ForEach(t =>
            //{
            //    var address = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses[t];
            //    address.Abandon();
            //});
            string id = Request.Form["ID"];
            var address = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses[id];

            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                address.Abandon();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/DelConsignor";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers[address.ClientSupplierID];

                    var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Clients[supplier.ClientID];

                    var entity = new ApiModel.SupplierAddress()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = "",
                            Name = supplier.Name,
                            RegAddress = "",
                            Uscc = "",
                            Status = 200
                        },
                        WsClient = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = apiclient.Company.Corporate,
                            Name = apiclient.Company.Name,
                            RegAddress = apiclient.Company.Address,
                            Uscc = apiclient.Company.Code,
                            Status = 200
                        },
                        Title = apiclient.Company.Name,
                        DyjCode = "",
                        District = 1,
                        Province = "",
                        City = "",
                        Land = "",
                        Name = address.Contact.Name,
                        Address = address.Address,
                        Mobile = address.Contact.Mobile,
                        Email = address.Contact.Email,
                        IsDefault = address.IsDefault,
                        Status = 200,
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        Postzip = "",
                        Place= address.Place
                    };
                    string SuppliersAddress = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, SuppliersAddress);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    address.Abandon();
                }
                catch (Exception)
                {

                    throw;
                }
                #endregion
            }
        }
    }
}