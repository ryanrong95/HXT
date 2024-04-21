//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;

//namespace Yahv.Csrm.WebApp.Crm.WsSuppliers
//{
//    public partial class Edit : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var supplierid = Request.QueryString["id"];
//                string clientid = Request.QueryString["clientid"];
//                this.Model.Entity = Erp.Current.Whs.WsClients[clientid].WsSuppliers[supplierid];
//                init();
//            }
//        }
//        void init()
//        {
//            //级别
//            this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Select(item => new
//            {
//                value = (int)item,
//                text = item.GetDescription()
//            });
//        }
//        protected void btnSubmit_Click(object sender, EventArgs e)
//        {
//            var id = Request.QueryString["id"];
//            string clientid = Request.QueryString["clientid"];
//            var wsclient = Erp.Current.Whs.WsClients[clientid];
//            var entity = wsclient.WsSuppliers[id] ?? new XdtWsSupplier();

//            string admincode = Request["AdminCode"].Trim();
//            string corporation = Request["Corporation"].Trim();
//            string regAddress = Request["RegAddress"].Trim();
//            string uscc = Request["Uscc"].Trim();
//            string summary = Request["Summary"];
//            string chinesename = Request.Form["Name"];//以中文名称作为供应商的企业名称
//            string englishName = Request["EnglishName"];
//            entity.WsClient = wsclient.Enterprise;
//            entity.Summary = summary;
//            entity.ChineseName = chinesename;
//            entity.EnglishName = englishName;
//            entity.Enterprise = new Enterprise
//            {
//                Name = chinesename,
//                AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode,
//                Corporation = corporation,
//                RegAddress = regAddress,
//                Uscc = uscc
//            };
//            entity.Grade = (SupplierGrade)int.Parse(Request["Grade"]);

//            ///国家或地区
//            entity.Place = Request["Origin"];
//            //Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(item => item.GetOrigin().Code == Request["Origin"]);

//            if (string.IsNullOrEmpty(id))
//            {
//                //录入人
//                entity.CreatorID = Yahv.Erp.Current.ID;
//                entity.StatusUnnormal += Entity_StatusUnnormal;
//            }
//            entity.EnterSuccess += suppliers_EnterSuccess;
//            entity.Enter();
//        }

//        private void Entity_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
//        {
//            var entity = sender as XdtWsSupplier;
//            Easyui.Reload("提示", "代仓储供应商已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
//        }

//        private void suppliers_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            //调用接口
//            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(api))
//            {
//                var entity = sender as XdtWsSupplier;
//                Unify(api, entity);
//            }
//            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
//        }


//        object Unify(string api, XdtWsSupplier data)
//        {
//            var response = HttpClientHelp.HttpPostRaw(api + "/clients/suppliers", new
//            {
//                Enterprise = data.WsClient,
//                EnglishName = data.EnglishName,
//                ChineseName = data.ChineseName,
//                Summary = data.Summary
//            }.Json());
//            return response;

//        }
//    }
//}