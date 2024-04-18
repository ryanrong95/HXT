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

namespace WebApp.Client.User
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
            string id = Request.QueryString["ID"];
           // var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
            this.Model.ID = new
            {
                ID = id
            }.Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];

            //var useraccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.Users.Where(t => t.Client.ID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);
            var useraccounts = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];

            var accounts = useraccounts == null ? new Needs.Ccs.Services.Views.UsersView() : useraccounts.Users;

            Func<Needs.Ccs.Services.Models.User, object> convert = account => new
            {
                account.ID,
                Name = account.Name,
                RealName = account.RealName,
                Mobile = account.Mobile,
                Email = account.Email,
                Status = account.Status.GetDescription(),
                Summary = account.Summary,
                IsMain = account.IsMain ? "是" : "否",
                CreateDate = account.CreateDate.ToShortDateString()
            };

            this.Paging(accounts, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteUserAccount()
        {
            //string ids = Request.Form["ID"];
            //ids.Split(',').ToList().ForEach(userID =>
            //{
            //    var account = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.Users[userID];
            //    account.Abandon();
            //});
            string id= Request.Form["ID"];
            var user = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.Users[id];
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                user.Abandon();
                #endregion
            }
            else {
                #region 调用后
                try
            {
                string requestUrl = URL + "/CrmUnify/DelSiteUser";
                HttpResponseMessage response = new HttpResponseMessage();
                string requestClientUrl = requestUrl;//请求地址

                var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[user.ClientID];
                var entity = new ApiModel.ClientAccount()
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
                    UserName = user.Name,
                    RealName = user.RealName,
                    Password = user.Password,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    QQ = "",
                    Wx = "",
                    Summary = user.Summary,
                    IsMain = user.IsMain,
                    Status = 200,
                    CreateDate = DateTime.Now.ToString(),
                    UpdateDate = DateTime.Now.ToString(),
                };
                string apiclient = JsonConvert.SerializeObject(entity);
                response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                    return;
                }
                user.Abandon();
            }
            catch (Exception)
            {

                throw;
            }
            #endregion
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        protected void ResetPassword()
        {
            string id = Request.Form["ID"];
            string userid = Request.Form["UserID"];
            var user = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].Users[userid];

            #region   调用前
            user.ResetPassword();
            #endregion
            #region 调用后
            try
            {
                string requestUrl = URL + "/ResetPwd";
                HttpResponseMessage response = new HttpResponseMessage();
                string requestClientUrl = requestUrl;//请求地址

                var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[user.ClientID];
                var entity = new ApiModel.ClientAccount()
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
                    UserName = user.Name,
                    RealName = user.RealName,
                    Password = user.Password,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    QQ = "",
                    Wx = "",
                    Summary = user.Summary,
                    IsMain = user.IsMain,
                    Status = 200,
                    CreateDate = DateTime.Now.ToString(),
                    UpdateDate = DateTime.Now.ToString(),
                };
                string apiclient = JsonConvert.SerializeObject(entity);
                response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                    return;
                }
                user.ResetPassword();
            }
            catch (Exception)
            {

                throw;
            }
            #endregion

        }
    }
}