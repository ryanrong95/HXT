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
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.WsConsignees
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //地区
                this.Model.District = ExtendsEnum.ToArray<District>(District.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
                this.Model.Entity = wsclient.Consignees[Request.QueryString["id"]];
                this.Model.EnterpriseType = Request["enterprisetype"];
            }
        }
        bool IsChineseAddress = false;//用于判断是不是中国地址
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string clientid = Request.QueryString["clientid"];
            var id = Request.QueryString["id"];
            var client = Erp.Current.Whs.WsClients[clientid];
            if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.Reload("提示!", "请先规范客户名称", Web.Controls.Easyui.Sign.None);
            }
            WsConsignee entity = client.Consignees[id] ?? new WsConsignee();

            entity.Title = Request["Title"].Trim();
            entity.EnterpriseID = clientid;
            entity.District = District.CN;
            string postzip = Request.Form["Postzip"].Trim();
            entity.Postzip = string.IsNullOrWhiteSpace(postzip) ? "" : postzip;
            string dyjcode = Request.Form["DyjCode"].Trim();
            entity.DyjCode = string.IsNullOrWhiteSpace(dyjcode) ? "" : dyjcode;
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.IsDefault = Request["IsDefault"] == null ? false : true;
            if (string.IsNullOrWhiteSpace(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
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
                string A = string.Join(" ", area);
                entity.Address = string.Join(" ", A, Request["Address"]).Replace("中国 ", "");

            }
            entity.Place = Request["Origin"];
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            string api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            var entity = sender as WsConsignee;
            //只有中国地址向芯达通同步
            if (!string.IsNullOrWhiteSpace(api) && IsChineseAddress)
            {
                var client = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
                Unify(api, client.Enterprise, entity);
            }

            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        object Unify(string api, Enterprise client, WsConsignee data)
        {
            var json = new
            {
                Enterprise = client,
                Receiver = data.Title,
                Name = data.Name,
                Mobile = data.Tel,
                Address = data.Address.Replace("中国 ", ""),
                IsDefault = data.IsDefault,
                Email = data.Email,
                Summary = "",
                Place = data.Place
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(api + "/clients/consignee", json);
            return response;
        }
    }
}