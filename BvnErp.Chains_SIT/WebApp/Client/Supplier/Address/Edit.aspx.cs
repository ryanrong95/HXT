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
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //参数
            string clientSupplierID = Request.QueryString["SupplierID"];
            this.Model.ClientSupplierID = clientSupplierID;
            string addressID = Request.QueryString["SupplierAddressID"];
            this.Model.AddressID = addressID ?? "";
            //国家、地区
            this.Model.Places = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(x => new { x.Code, x.Name }).Json();
            if (!string.IsNullOrEmpty(addressID))
            {
                var address = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses[addressID];
                this.Model.SupplierAddressData = new
                {
                    ID = address.ID,
                    ContactName = address.Contact.Name,
                    Mobile = address.Contact.Mobile,
                    Address = address.Address.Replace("'", "#39;"),
                    IsDefault = address.IsDefault,
                    Summary = address?.Summary,
                    address.Place
                }.Json();
            }
            else
            {
                this.Model.SupplierAddressData = null;
            }
        }

        /// <summary>
        /// 保存供应商提货地址信息
        /// </summary>
        protected void SaveClientSupplierAddress()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'").Replace("&amp;", "&");
            dynamic model = Model.JsonTo<dynamic>();
            string addressID = model.AddressID;
            string place = model.PlaceID;
            var supplieraddress = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses[addressID] ?? new Needs.Ccs.Services.Models.ClientSupplierAddress();

            string address = model.Address;
            supplieraddress.Contact = supplieraddress.Contact ?? new Needs.Ccs.Services.Models.Contact();
            supplieraddress.Contact.Name = model.ContactName;
            supplieraddress.Contact.Mobile = model.Mobile;

            supplieraddress.ClientSupplierID = model.ClientSupplierID;
            supplieraddress.Address = address.ToString().Replace("#39;", "'");
            supplieraddress.IsDefault = model.IsDefault == null ? false : model.IsDefault;
            supplieraddress.Place = place;
            supplieraddress.Summary = model.Summary.ToString().Replace("#39;", "\'");
            supplieraddress.EnterError += SupplierAddress_EnterError;
            supplieraddress.EnterSuccess += SupplierAddress_EnterSuccess;
            //默认地址
            if (supplieraddress.IsDefault)
            {
                var clientaddress = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierAddresses.Where(t => t.ClientSupplierID == supplieraddress.ClientSupplierID);
                foreach (var add in clientaddress)
                {
                    add.IsDefault = false;
                    add.Enter();
                }
            }
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                supplieraddress.Enter();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/ConsignorEnter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers[supplieraddress.ClientSupplierID];
                    var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[supplier.ClientID];

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
                            Status = 200,
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
                        Name = supplieraddress.Contact.Name,
                        Address = supplieraddress.Address,
                        Mobile = supplieraddress.Contact.Mobile,
                        Email = supplieraddress.Contact.Email,
                        IsDefault = supplieraddress.IsDefault,
                        Status = 200,
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                        Postzip = "",
                        Place= place
                    };

                    string apiSupplier = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }

                    supplieraddress.Enter();
                }
                catch (Exception)
                {

                    throw;
                }

                #endregion
            }

        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierAddress_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierAddress_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}