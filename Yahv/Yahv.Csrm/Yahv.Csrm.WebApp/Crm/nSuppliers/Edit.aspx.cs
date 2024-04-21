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

namespace Yahv.Csrm.WebApp.Crm.nSuppliers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Where(item => item == SupplierGrade.First || item == SupplierGrade.Second || item == SupplierGrade.Third).Select(item => new
                {
                    value = (int)item,
                    text = "<span class='level" + (int)item + "'></span>"
                });
                this.Model.Entity = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]].nSuppliers[Request.QueryString["id"]];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            string clientid = Request.QueryString["clientid"];
            var wsclient = Erp.Current.Whs.WsClients[clientid];
            if (wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.AutoAlert("请先规范客户名称", Web.Controls.Easyui.AutoSign.Warning);
                return;
            }
            var entity = wsclient.nSuppliers[id] ?? new nSupplier();

            string admincode = "";// Request["AdminCode"].Trim();
            string corporation = Request["Corporation"].Trim();
            string regAddress = Request["RegAddress"].Trim().Trim();
            string uscc = Request["Uscc"].Trim();
            string summary = Request["Summary"];
            string chinesename = Request.Form["Name"];//以中文名称作为供应商的企业名称
            string englishName = Request["EnglishName"].Trim();
            entity.EnterpriseID = clientid;
            entity.ChineseName = chinesename;
            entity.EnglishName = englishName;
            entity.RealEnterprise = new Enterprise
            {
                Name = englishName,
                AdminCode = admincode,
                Corporation = corporation,
                RegAddress = regAddress,
                Uscc = uscc,
                Place = Request["Place"]
            };
            entity.Grade = (SupplierGrade)int.Parse(Request["Grade"]);
            entity.CHNabbreviation = Request["Abbreviation"].Trim();
            entity.Summary = summary;
            if (string.IsNullOrEmpty(id))
            {
                //录入人
                entity.Creator = Yahv.Erp.Current.ID;
                entity.Repeat += Entity_Repeat;
            }
            entity.EnterSuccess += suppliers_EnterSuccess;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Ttop.Close("供应商已存在", Yahv.Web.Controls.Easyui.AutoSign.Warning);
        }

        private void suppliers_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //调用接口
            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(api))
            {
                var entity = sender as nSupplier;
                Unify(api, entity);
            }
            Easyui.Ttop.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }


        object Unify(string api, nSupplier data)
        {
            var client = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll()[data.EnterpriseID];
            var json = new
            {
                Enterprise = client,
                EnglishName = data.EnglishName,
                ChineseName = data.ChineseName,
                Summary = data.Summary,
                Grade = (int)data.Grade,
                Place = data.RealEnterprise.Place
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(api + "/clients/suppliers", json);
            return response;

        }
    }
}