using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.SiteUsers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["wsclientid"]];
                if (wsclient != null)
                {
                    var user = wsclient.SiteUsers[Request.QueryString["userid"]];
                    this.Model.Entity = user;
                    this.Model.Phone = wsclient.Contact?.Mobile;
                }

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var userid = Request.QueryString["userid"];
            var wsclientid = Request.QueryString["wsclientid"];

            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["wsclientid"]];
            if (wsclient.ServiceManager == null)
            {
                Easyui.Dialog.Close("请先分配业务员!", Web.Controls.Easyui.AutoSign.Error);
            }
            if (wsclient.SiteUsers.Count() > 5)
            {
                Easyui.Dialog.Close("会员账号数量已达到上限!", Web.Controls.Easyui.AutoSign.Error);
                //Easyui.Alert("提示", "会员账号数量已达到上限", Yahv.Web.Controls.Easyui.Sign.Warning, true, Web.Controls.Easyui.Method.Dialog);
            }
            string username = Request["UserName"].Trim();

            if (wsclient != null)
            {
                var siteuser = wsclient.SiteUsers[userid] ?? new SiteUserXdt();
                siteuser.UserName = Request["UserName"].Trim();
                //siteuser.RealName = Request["RealName"].Trim();
                siteuser.Mobile = Request["Mobile"];
                siteuser.Email = Request["Email"];
                siteuser.Summary = Request["Summary"];
                siteuser.QQ = Request["QQ"].Trim();
                siteuser.Wx = Request["Wx"].Trim();
                siteuser.IsMain = Request["IsMain"] == null ? false : true;

                if (string.IsNullOrWhiteSpace(userid))
                {
                    siteuser.Password = Request["Password"].StrToMD5();
                    siteuser.EnterpriseID = wsclientid;
                }
                siteuser.UserNameRepeat += Siteuser_UserNameRepeat;
                siteuser.EnterSuccess += Siteuser_EnterSuccess;
                siteuser.Enter();
            }
        }

        private void Siteuser_UserNameRepeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Reload("提示", "用户名已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Siteuser_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            var data = sender as SiteUserXdt;
            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["wsclientid"]];
            if (!string.IsNullOrWhiteSpace(apiurl) && wsclient.ServiceManager != null)
            {
                Unify(apiurl, wsclient, data);
            }
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        object Unify(string api, WsClient client, SiteUserXdt data)
        {
            var json = new
            {
                Enterprise = client.Enterprise,
                UserName = data.UserName,
                Password = data.Password,
                Mobile = data.Mobile,
                Email = data.Email,
                IsMain = data.IsMain,
                RealName = data.RealName,
                Summary = data.Summary,
                Creator = client.ServiceManager?.RealName
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"] + "/clients/account", json);
            return response;

        }
    }
}