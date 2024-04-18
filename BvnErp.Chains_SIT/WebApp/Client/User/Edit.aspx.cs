using Needs.Utils.Serializers;
using System;
using Needs.Ccs.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using WebApp.App_Utils;
using System.Text.RegularExpressions;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Enums;

namespace WebApp.Client.User
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

            string userID = Request.QueryString["UserID"];
            this.Model.UserID = userID ?? "";
            if (!string.IsNullOrEmpty(id))
            {
                this.Model.Tel = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].Company.Contact.Mobile;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                var user = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.Users[userID];
                this.Model.UserData = new
                {
                    ID = user.ID,
                    RealName = user.RealName,
                    Name = user.Name,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    IsMain = user.IsMain,
                    Summary = user.Summary
                }.Json();
            }
            else
            {
                this.Model.UserData = null;
            }
        }

        /// <summary>
        /// 保存会员信息
        /// </summary>
        protected void SaveUserAccount()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var user = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.Users[(string)model.UserID] as Needs.Ccs.Services.Models.User ??
                new Needs.Ccs.Services.Models.User();

            user.Name = model.Name;
            user.Mobile = model.Mobile;
            user.Email = model.Email;
            user.Password = Needs.Utils.Converters.StringExtend.StrToMD5(string.IsNullOrEmpty((string)model.Password) ? user.Password:(string)model.Password);
            user.RealName = model.RealName;
            user.ClientID = model.ClientID;
            user.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            user.Summary = model.Summary;
            user.IsMain = model.IsMain;
            user.EnterError += UserAccount_EnterError;
            user.EnterSuccess += UserAccount_EnterSuccess;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                user.Enter();
                #endregion
            }
            else {
            #region 调用后
            try
            {
                string requestUrl = URL + "/CrmUnify/SiteUser";
                HttpResponseMessage response = new HttpResponseMessage();
                string requestClientUrl = requestUrl;//请求地址

                var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[user.ClientID];
                var entity = new ApiModel.ClientAccount()
                {
                    Enterprise = new EnterpriseObj
                    {
                        AdminCode = " ",
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
                user.Enter();
            }
            catch (Exception)
            {

                throw;
            }
            #endregion
            }
        }



        /// <summary>
        ///判断名称是否重复
        /// </summary>
        /// <returns></returns>
        protected bool IsExitName()
        {
            var result = false;
            var name = Request.Form["Name"];
            string id = Request.Form["ID"];
            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            name = regex.Replace(name, " ").Trim();
            string ClientID = Request.Form["ClientID"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                result = new UsersView().Where(x=>x.ID!=id &&x.Name == name && x.Status == Status.Normal).Count() < 1;
               
            }
            else
            {
                int count = new UsersView().Where(x=>x.Name == name && x.Status == Status.Normal).Count();
                result = count < 1;
            }
            return result;
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAccount_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAccount_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}