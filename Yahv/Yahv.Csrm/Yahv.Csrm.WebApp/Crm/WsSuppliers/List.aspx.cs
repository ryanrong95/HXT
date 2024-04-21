//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Web.Erp;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;

//namespace Yahv.Csrm.WebApp.Crm.WsSuppliers
//{
//    public partial class List : ErpParticlePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                //init();
//                this.Model.ClientID = Request.QueryString["id"];
//            }
//        }
//        void init()
//        {
//            Dictionary<string, string> statuslist = new Dictionary<string, string>() { { "-100", "全部" } };
//            statuslist.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
//            statuslist.Add(ApprovalStatus.Waitting.ToString(), ApprovalStatus.Waitting.GetDescription());
//            statuslist.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
//            statuslist.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
//            statuslist.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
//            //状态
//            this.Model.SupplierStatus = statuslist.Select(item => new
//            {
//                value = item.Key,
//                text = item.Value
//            });
//        }
//        protected object data()
//        {
//            string clientid = Request.QueryString["id"];
//            var query = Erp.Current.Whs.WsClients[clientid].WsSuppliers.Where(Predicate());

//            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
//            {
//                item.ID,
//                item.Enterprise.Name,
//                item.ChineseName,
//                item.EnglishName,
//                item.Enterprise.AdminCode,
//                item.Grade,
//                item.Enterprise.District,
//                item.WsSupplierStatus,
//                StatusName = item.WsSupplierStatus.GetDescription(),
//                Admin = item.Creator == null ? null : item.Creator.RealName,
//                item.Enterprise.Uscc,
//                item.Enterprise.Corporation,
//                item.Enterprise.RegAddress,
//                Origin = item.Place == null ? null : Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin().ChineseName,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//            });
//        }
//        Expression<Func<WsSupplier, bool>> Predicate()
//        {
//            Expression<Func<WsSupplier, bool>> predicate = item => true;
//            //var status = Request["selStatus"];
//            var name = Request["s_name"];
//            //ApprovalStatus approvalstatus;
//            //if (Enum.TryParse(status, out approvalstatus) && status != "-100")
//            //{
//            //    predicate = predicate.And(item => item.WsSupplierStatus == approvalstatus);
//            //}
//            if (!string.IsNullOrWhiteSpace(name))
//            {
//                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
//            }

//            return predicate;
//        }
//        protected object DelMaps()
//        {
//            var wsclient = Erp.Current.Whs.WsClients[Request["clientid"]];
//            if (wsclient == null)
//            {
//                return new { msg = "客户不存在", success = false };
//            }
//            else if (wsclient.WsSuppliers[Request["supplierid"]] == null)
//            {
//                return new { msg = "供应商不存在不存在", success = false };
//            }
//            else
//            {
//                var wssupplier = wsclient.WsSuppliers[Request["supplierid"]];
//                wssupplier.Abandon();
//                //调用接口
//                var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//                if (!string.IsNullOrWhiteSpace(api))
//                {
//                    Unify(api, wsclient.Enterprise.Name, wssupplier.Enterprise.Name);
//                }
//                return new { msg = "操作成功", success = true };
//            }
//        }
//        object Unify(string api, string clientname, string suppliername)
//        {
//            var response = HttpClientHelp.CommonHttpRequest(api + "/clients/suppliers?name=" + clientname + "&supplierName=" + suppliername, "DELETE");
//            return response;
//        }
//    }
//}