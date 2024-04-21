using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.SiteUsers
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wscient = Erp.Current.Whs.WsClients[Request["id"]];
                this.Model.WsClient = wscient;
                this.Model.Count = wscient.SiteUsers.Count();
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var siteusers = Erp.Current.Whs.WsClients[id].SiteUsers;
            return new
            {
                rows = siteusers.OrderBy(item => item.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.UserName,
                    item.RealName,
                    item.Mobile,
                    item.Email,
                    item.QQ,
                    item.Wx,
                    Status = item.Status.GetDescription(),
                    item.Summary,
                    IsMain = item.IsMain ? "是" : "否",
                    CreateDate = item.CreateDate
                })
            };
        }

        protected void del()
        {
            var userid = Request.Form["userid"];
            var wsclient = Erp.Current.Whs.WsClients[Request.Form["clientid"]];
            if (wsclient != null)
            {
                var User = wsclient.SiteUsers[userid];
                string username = User.UserName;
                User.Abandon();
                string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                if (!string.IsNullOrWhiteSpace(apiurl))
                {
                    Unify_DelUser(apiurl, wsclient.Enterprise, username);
                }
            }
        }



        protected void resetPwd()
        {
            var userid = Request.Form["userid"];
            var wsclient = Erp.Current.Whs.WsClients[Request.Form["enterpriseid"]];
            if (wsclient != null)
            {
                wsclient.SiteUsers[userid].ResetPwd();
                string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                if (!string.IsNullOrWhiteSpace(apiurl))
                {
                    Unify_RestPwd(apiurl, wsclient.Enterprise, wsclient.SiteUsers[userid]);
                }
            }
        }
        //调用接口，重置密码
        object Unify_RestPwd(string apiurl, Enterprise client, SiteUserXdt data)
        {
            var response = Utils.Http.ApiHelper.Current.Post(System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"] + "/account/reset?name=" + client.Name + "&userName=" + data.UserName);
            return response;
        }
        //调用接口，删除用户
        object Unify_DelUser(string api, Enterprise client, string UserName)
        {
            var response = HttpClientHelp.CommonHttpRequest(api + "/clients/account?name=" + client.Name + "&userName=" + UserName, "DELETE");
            return response;
        }
    }
}
