//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Web.Erp;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Crm.WsConsignors
//{
//    public partial class List : ErpParticlePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                this.Model.ClientID = Request.QueryString["clientid"];
//                this.Model.SupplierID = Request.QueryString["SupplierID"];
//                //Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "0", "全部" } };
//                //statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
//                //statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
//                //statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
//                //statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
//                ////状态
//                //this.Model.Status = statuslist.Select(item => new
//                //{
//                //    value = item.Key,
//                //    text = item.Value
//                //});
//            }
//        }
//        protected object data()
//        {
//            string id = Request.QueryString["id"];
//            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//            var wssupplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//            var consignor = wssupplier.Consignors.Where(Predicate());
//            return new
//            {
//                rows = consignor.ToArray().Select(item => new
//                {
//                    item.ID,
//                    item.Address,
//                    item.Postzip,
//                    item.DyjCode,
//                    ContactName = item.Name,
//                    item.Tel,
//                    item.Mobile,
//                    item.Email,
//                    item.Status,
//                    StatusName = item.Status.GetDescription(),
//                    EnterpriseID = item.EnterpriseID,
//                    Admin = item.Admin == null ? null : item.Admin.RealName,
//                    item.Title,
//                    IsDefault = item.IsDefault ? "是" : "否"
//                })
//            };
//        }
//        Expression<Func<Consignor, bool>> Predicate()
//        {
//            Expression<Func<Consignor, bool>> predicate = item => true;
//            var address = Request["address"];
//            var contactname = Request["contactname"];
//            var tel = Request["tel"];
//            var status = Request["status"];
//            if (!string.IsNullOrWhiteSpace(address))
//            {
//                predicate = predicate.And(item => item.Address.Contains(address));
//            }
//            if (!string.IsNullOrWhiteSpace(contactname))
//            {
//                predicate = predicate.And(item => item.Name.Contains(contactname));
//            }
//            if (!string.IsNullOrWhiteSpace(tel))
//            {
//                predicate = predicate.And(item => item.Tel.Contains(tel) || item.Mobile.Contains(tel));
//            }
//            //ApprovalStatus approvalstatus;
//            //if (Enum.TryParse(status, out approvalstatus) && approvalstatus != 0)
//            //{
//            //    predicate = predicate.And(item => item.Status == approvalstatus);
//            //}
//            return predicate;
//        }
//        protected void del()
//        {
//            var wsclient = Erp.Current.Whs.WsClients[Request["clientid"]];
//            var wssupllier = wsclient.WsSuppliers[Request["supplierid"]];
//            var entity = wssupllier.Consignors[Request["id"]];
//            entity.Abandon();
//            //调用接口
//            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(api))
//            {
//                Unify(api, wsclient.Enterprise.Name, wssupllier.Enterprise.Name, entity.Address);
//            }
//        }

//        object Unify(string api, string clientname, string suppliername, string address)
//        {
//            var response = HttpClientHelp.CommonHttpRequest(api + "/suppliers/address?name=" + clientname + "&supplierName=" + suppliername + "&address=" + address, "DELETE");
//            return response;
//        }
//    }
//}