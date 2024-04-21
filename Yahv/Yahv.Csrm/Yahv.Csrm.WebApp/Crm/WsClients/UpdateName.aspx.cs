using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Csrm.WebApp.Crm.WsClients
{
    public partial class UpdateName : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                string companyid = Request.QueryString["CompanyID"];
                string Name = Request.QueryString["Name"].Replace("reg-", "");
                this.Model.Entity = new { ClientID = clientid, Name = Name, CompanyID = companyid };
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string clientid = Request.QueryString["id"];
            string CompanyID = Request.QueryString["CompanyID"];
            string newname = Request["NewName"].Trim();
            bool result = false;
            var client = Erp.Current.Whs.WsClients[CompanyID, clientid];
            if (newname.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Reload("提示", "客户名称不规范", Web.Controls.Easyui.Sign.Warning);
            }
            else
            {
                if (new YaHv.Csrm.Services.Views.Rolls.WsClientsRoll().Any(item => item.Enterprise.Name == newname))
                {
                    Easyui.Reload("提示", "客户已存在", Web.Controls.Easyui.Sign.Warning);
                }
                else if (new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll().Any(item => item.Name == newname))
                {
                    result = client.UpdateEnterpriseID(null, newname);
                }
                else
                {
                    result = client.Enterprise.UpdateName(newname);
                }
            }
            if (result)
            {
                //同步
                string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                if (!string.IsNullOrWhiteSpace(apiurl))
                {
                    var ss = Yahv.Utils.Http.ApiHelper.Current.JPost($"{apiurl}/clients/ModifyCompanyName?OldName={client.Enterprise.Name}&NewName={newname}");
                }
                Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }
        //protected object data()
        //{
        //    string id = Request.QueryString["id"];
        //    var siteusers = Erp.Current.Whs.WsClients[id].SiteUsers;
        //    return new
        //    {
        //        rows = siteusers.OrderBy(item => item.CreateDate).ToArray().Select(item => new
        //        {
        //            item.ID,
        //            item.UserName,
        //            item.RealName,
        //            item.Mobile,
        //            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
        //        })
        //    };
        //}
    }
}