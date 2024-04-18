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

namespace WebApp.Client.Consignee
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            //参数
            string id = Request.QueryString["ID"];
            this.Model.ID = id;

            string clientConsigneeID = Request.QueryString["ConsigneeID"];
            this.Model.ClientConsigneeID = clientConsigneeID ?? "";

            string count = Request.QueryString["Count"];
            this.Model.Count = count ?? "";
            if (!string.IsNullOrEmpty(id) && count == "0")
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                this.Model.ClientInfoData = new
                {
                    CompanyName = client.Company.Name,
                    Address = client.Company.Address,
                    ContactName = client.Company.Contact.Name,
                    Mobile = client.Company.Contact.Mobile,
                    IsDefault = true
                }.Json();
            }
            else
            {
                this.Model.ClientInfoData = null;
            }
            if (!string.IsNullOrEmpty(clientConsigneeID))
            {
                var consignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees[clientConsigneeID];
                this.Model.ClientConsigneeData = new
                {
                    ID = consignee.ID,
                    Name = consignee.Name,
                    Address = consignee.Address,
                    ContactName = consignee.Contact.Name,
                    Mobile = consignee.Contact.Mobile,
                    Email = consignee.Contact.Email,
                    IsDefault = consignee.IsDefault,
                    Summary = consignee.Summary
                }.Json();
            }
            else
            {
                this.Model.ClientConsigneeData = null;
            }
        }

        /// <summary>
        /// 保存会员收件地址信息
        /// </summary>
        protected void SaveClientConsignee()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string consigneeID = model.ClientConsigneeID;
            var consignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees[consigneeID] ?? new Needs.Ccs.Services.Models.ClientConsignee();

            consignee.Contact = new Needs.Ccs.Services.Models.Contact();
            consignee.Name = model.Name;
            consignee.Contact.Name = model.ContactName;
            consignee.Contact.Mobile = model.Mobile;
            consignee.Contact.Tel = model.Mobile;
            consignee.Contact.Email = model.Email;
            consignee.Address = model.Address;
            consignee.ClientID = model.ClientID;
            consignee.IsDefault = model.IsDefault;
            consignee.Summary = model.Summary;

            //默认地址
            if (consignee.IsDefault)
            {
                var consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees.Where(t => t.ClientID == consignee.ClientID);
                foreach (var con in consignees)
                {
                    con.IsDefault = false;
                    con.Enter();
                }
            }

            consignee.EnterError += Consignee_EnterError;
            consignee.EnterSuccess += Consignee_EnterSuccess;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                consignee.Enter();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/ConsigneeEnter";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址

                    var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[consignee.ClientID];
                    var addressdata = new ApiAddressHelp().GetReceiver(consignee.Address);
                    var entity = new ApiModel.ClientConsigin()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = client.Company.Corporate,
                            Name = client.Company.Name,
                            RegAddress = client.Company.Address,
                            Uscc = client.Company.Code,
                            Status = 200
                        },
                        Title = consignee.Name,
                        Place = "CHN",
                        DyjCode = "",
                        District = 1,
                        Province = addressdata.ProvinceName,
                        City = addressdata.CityName,
                        Land = addressdata.ExpAreaName,
                        Name = consignee.Contact.Name,
                        Mobile = consignee.Contact.Mobile,
                        Address = consignee.Address,
                        Tel = consignee.Contact.Mobile,
                        Email = consignee.Contact.Email,
                        IsDefault = consignee.IsDefault,
                        Status = 200,
                        Postzip = "",
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

                    consignee.IsDefault = consignee.IsDefault ? true : false;
                    consignee.Enter();
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
        private void Consignee_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consignee_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}