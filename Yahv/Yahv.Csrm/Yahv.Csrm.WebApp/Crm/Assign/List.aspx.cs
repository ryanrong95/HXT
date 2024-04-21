using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Assign
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["CompanyID"], Request.QueryString["id"]];
                this.Model.ServiceManagerID = wsclient.ServiceManager == null ? null : wsclient.ServiceManager.ID;
                this.Model.MerchandiserID = wsclient.Merchandiser == null ? null : wsclient.Merchandiser.ID;
                this.Model.ReferrerID = wsclient.Referrer == null ? null : wsclient.Referrer.ID;
                //业务员和业务负责人
                this.Model.ServiceManager = Erp.Current.Crm.Admins.Where(manager => manager.RoleID == FixedRole.ServiceManager.GetFixedID()
                || manager.RoleID == FixedRole.ServiceManagerLeader.GetFixedID() || manager.RoleName == "报关系统总经理").Select(item => new
                {
                    ID = item.ID,
                    RealName = item.RealName,
                }).ToArray();
                //跟单员
                this.Model.Merchandiser = Erp.Current.Crm.Admins.Where(manager => manager.RoleID == FixedRole.Merchandiser.GetFixedID()).Select(item => new
                {
                    ID = item.ID,
                    RealName = item.RealName
                }).ToArray();
                this.Model.isAssignMerchandiser = wsclient.Contract == null;
                this.Model.CompanyID = Request.QueryString["ComapnyID"];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string companyid = Request.QueryString["CompanyID"];
            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["CompanyID"], Request.QueryString["ID"]];
            if (wsclient == null)
            {
                return;
            }

            var saleid = Request["ServiceManager"];
            var Merchandiser = Request["Merchandiser"];
            var reffererid = Request["Referrer"];
            if (!string.IsNullOrWhiteSpace(saleid))
            {
                wsclient.Assin(saleid, MapsType.ServiceManager);
                if (!string.IsNullOrWhiteSpace(Merchandiser))
                {
                    wsclient.Assin(Merchandiser, MapsType.Merchandiser);
                    if (wsclient.WsClientStatus == ApprovalStatus.UnComplete)
                    {
                        Erp.Current.Whs.WsClients[wsclient.ID].Complete();
                    }
                }
                if (!string.IsNullOrWhiteSpace(reffererid))
                {
                    wsclient.Assin(reffererid, MapsType.Referrer);
                }
                //贯通接口
                var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                //芯达通客户时同步
                if (!string.IsNullOrWhiteSpace(api) && companyid == "DBAEAB43B47EB4299DD1D62F764E6B6A")
                {
                    var admins = Erp.Current.Crm.Admins;
                    var merchandiser = string.IsNullOrWhiteSpace(Merchandiser) ? "" : admins[Merchandiser]?.RealName;//跟单员不一定分配
                    var refferer = string.IsNullOrWhiteSpace(reffererid) ? "" : admins[reffererid]?.RealName;
                    Unify(wsclient.Enterprise, admins[saleid].RealName, merchandiser, refferer);
                }

                Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }

        //调用接口
        object Unify(Enterprise client, string ServiceManager, string Merchandiser, string Referrer)
        {
            var json = new
            {
                Enterprise = client,
                ServiceManager = ServiceManager,
                Merchandiser = Merchandiser,
                Referrer = Referrer,
                Summary = ""
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"] + "/clients/Assign", json);
            return response;
        }
    }
}