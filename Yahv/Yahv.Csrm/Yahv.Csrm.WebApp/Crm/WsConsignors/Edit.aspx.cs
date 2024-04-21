//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Crm.WsConsignors
//{
//    public partial class Edit : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//                var wssupplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//                this.Model.Entity = wssupplier.Consignors[Request.QueryString["id"]];
//            }
//        }
//        protected void btnSubmit_Click(object sender, EventArgs e)
//        {
//            //try
//            //{
//            string clientid = Request.QueryString["clientid"];
//            var id = Request.QueryString["id"];
//            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//            var wssupplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//            var entity = wssupplier.Consignors[id] ?? new WsConsignor();
//            entity.WsClient = wsclient.Enterprise;
//            entity.Title = Request["Title"].Trim();
//            entity.EnterpriseID = Request.QueryString["supplierid"];
//            entity.Enterprise = wssupplier.Enterprise;
//            entity.Name = Request.Form["Name"].Trim();
//            entity.Tel = Request.Form["Tel"].Trim();
//            entity.Mobile = Request.Form["Mobile"].Trim();
//            entity.Email = Request.Form["Email"].Trim();
//            entity.IsDefault = Request["IsDefault"] == null ? false : true; ;
//            if (string.IsNullOrWhiteSpace(id))
//            {
//                entity.CreatorID = Yahv.Erp.Current.ID;

//                string address = string.Join(" ", "香港", Request.Form["HKArea"], Request.Form["Address"].Trim());
//                entity.Address = address;
//                string postzip = Request.Form["Postzip"].Trim();
//                entity.Postzip = string.IsNullOrWhiteSpace(postzip) ? "" : postzip;
//                string dyjcode = Request.Form["DyjCode"].Trim();
//                entity.DyjCode = dyjcode == null ? "" : dyjcode;
//            }
//            entity.EnterSuccess += Entity_EnterSuccess;
//            entity.Enter();

//        }

//        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            //数据同步，调用接口
//            string api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(api))
//            {
//                var entity = sender as Consignor;
//                var client = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//                Unify(api, client.Enterprise, entity);
//            }

//            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
//        }
//        //调用接口
//        object Unify(string api, Enterprise client, Consignor data)
//        {
//            var json = new
//            {
//                Enterprise = client,
//                SupplierName = data.Enterprise.Name,
//                IsDefault = data.IsDefault,
//                Address = data.Address,
//                Name = data.Name,
//                Tel = data.Tel,
//                Mobile = data.Mobile,
//                PostZip = data.Postzip
//            }.Json();
//            var response = HttpClientHelp.HttpPostRaw(api + "/suppliers/address", json);
//            return response;
//        }
//    }
//}