using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nConsignors
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
                var nsupplier = wsclient.nSuppliers[Request.QueryString["supplierid"]];
                var nConsignor = nsupplier.nConsignors[Request.QueryString["id"]];
                this.Model.Entity = nConsignor;
                //this.Model.Place = nsupplier.RealEnterprise.Place ==null ? "未知" : Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == nsupplier.RealEnterprise.Place).GetOrigin().ChineseName;
            }
        }
        bool IsChineseAddress = false;//用于判断是不是中国地址
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string clientid = Request.QueryString["clientid"];
            var id = Request.QueryString["id"];
            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
            if (wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.AutoAlert("请先规范客户名称", Web.Controls.Easyui.AutoSign.Warning);
                return;
            }
            var nsupplier = wsclient.nSuppliers[Request.QueryString["supplierid"]];
            var entity = nsupplier.nConsignors[id] ?? new nConsignor();
            entity.nSupplierID = nsupplier.ID;
            entity.EnterpriseID = wsclient.Enterprise.ID;
            //entity.Title = Request["Title"].Trim();
            //entity.RealID = nsupplier.ID;
            entity.Contact = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.IsDefault = Request["IsDefault"] == null ? false : true; ;
            string[] area = Request["muliarea"].Split(',');
            #region 地址是不是中国

            switch (area[0])
            {
                case "中国":
                    if (area[1] == "香港")
                    {
                        IsChineseAddress = true;
                    }
                    else if (area[1] == "台湾")
                    {
                        IsChineseAddress = true;
                    }
                    else
                    {
                        IsChineseAddress = true;
                    }
                    break;
                default:
                    IsChineseAddress = false;
                    break;
            }
            #endregion


            if (string.IsNullOrWhiteSpace(id))
            {
                entity.Creator = Yahv.Erp.Current.ID;
                string A = string.Join(" ", area);
                entity.Address = string.Join(" ", A, Request["Address"]).Replace("中国 ", "");

            }
            entity.Place = Request["Origin"];
            string postzip = Request.Form["Postzip"].Trim();
            entity.Postzip = postzip;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //数据同步，调用接口
            string api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            var entity = sender as nConsignor;
            if (!string.IsNullOrWhiteSpace(api) && IsChineseAddress)
            {
                var client = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
                string suppliername = client.nSuppliers[Request.QueryString["supplierid"]].RealEnterprise.Name;
                Unify(api, client.Enterprise, suppliername, entity);
            }

            Easyui.Ttop.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        //调用接口
        object Unify(string api, Enterprise client, string suppliername, nConsignor data)
        {
            var json = new
            {
                Enterprise = client,
                SupplierName = suppliername,
                Address = data.Address.Replace("中国 ", ""),
                Name = data.Contact,
                Tel = data.Tel,
                Mobile = data.Mobile,
                Place = data.Place,
                IsDefault = data.IsDefault
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(api + "/suppliers/address", json);
            return response;
        }
    }
}