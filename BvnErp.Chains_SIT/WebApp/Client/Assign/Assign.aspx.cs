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

namespace WebApp.Client.Assign
{
    public partial class Assign : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        protected void LoadData()
        {
            //参数
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").Select(item => item.Admin.ID).ToArray();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => ServiceIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
            var handerIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "跟单员").Select(item => item.Admin.ID).ToArray();
            this.Model.Merchandiser = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => handerIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();
            var admins = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAdmins.Where(t => t.ClientID == id && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();

            var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];
            this.Model.ClientReferrerData = Needs.Wl.Admin.Plat.AdminPlat.Referrers.Where(x => x.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Key = item.Name, Value = item.Name }).ToArray().Json();
            this.Model.ClientAssignData = admins.Json();
            this.Model.ClientReferrer = client.Referrer.Json();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SaveClientAssign()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[(string)model.ID];
            //client.SetServiceManager(Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)model.ServiceManagerID), (string)model.Summary);
            //client.SetMerchandiser(Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)model.MerchandiserID), (string)model.Summary);
            client.ServiceManager = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)model.ServiceManagerID);
            client.Merchandiser = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)model.MerchandiserID);
            string referrer = model.Referrers;
            client.Referrer = referrer;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                client.EnterSuccess += ClientAssign_EnterSuccess;
                client.EnterError += ClientAssign_EnterError;
                client.Confirm((string)model.Summary);
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    //领导的奇怪要求 显示张庆永操作
                    var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金")
                    {
                        adminid = "Admin0000000282";//张庆永的ID
                    }

                    string requestUrl = URL + "/CrmUnify/Assign";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var entity = new ApiModel.ClientAssign()
                    {
                        Client = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = client.Company.Corporate,
                            Name = client.Company.Name,
                            RegAddress = client.Company.Address,
                            Uscc = client.Company.Code,
                            Status = 200
                        },
                        ServiceManager = client.ServiceManager.RealName,
                        Merchandiser = client.Merchandiser.RealName,
                        Referrer = string.Empty,
                        IsDeclaretion = client.IsValid.HasValue ? client.IsValid.Value : false,
                        IsStorageService = client.IsStorageValid.HasValue ? client.IsStorageValid.Value : false,
                    };
                    string apiSupplier = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    client.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(adminid);
                    client.EnterSuccess += ClientAssign_EnterSuccess;
                    client.EnterError += ClientAssign_EnterError;
                    //if (!string.IsNullOrEmpty(referrersID))
                    //{
                    //    client.Confirm((string)model.Summary);
                    //}
                    //else
                    //{
                    client.Confirm((string)model.Summary);
                    // }
                }
                catch (Exception ex)
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
        private void ClientAssign_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAssign_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}
