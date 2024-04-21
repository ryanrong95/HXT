using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Srm.WsSuppliers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
       {
//            if (!IsPostBack)
//            {
//                init();
//            }
        }
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
//            var query = Erp.Current.Srm.MyWsSuppliers.Where(Predicate());

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
//                Admin = item.Admin == null ? null : item.Admin.RealName,
//                item.Enterprise.Uscc,
//                item.Enterprise.Corporation,
//                item.Enterprise.RegAddress,
//                item.CreateDate,
//                item.UpdateDate
//            });
//        }
//        Expression<Func<WsSupplier, bool>> Predicate()
//        {
//            Expression<Func<WsSupplier, bool>> predicate = item => true;
//            var status = Request["selStatus"];
//            var name = Request["s_name"];
//            ApprovalStatus approvalstatus;
//            if (Enum.TryParse(status, out approvalstatus) && status != "-100")
//            {
//                predicate = predicate.And(item => item.WsSupplierStatus == approvalstatus);
//            }
//            if (!string.IsNullOrWhiteSpace(name))
//            {
//                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
//            }

//            return predicate;
//        }
//        protected void del()
//        {
//            var arry = Request.Form["items"].Split(',');
//            var entity = Erp.Current.Srm.MyWsSuppliers.Where(t => arry.Contains(t.ID));
//            entity.Delete();

//        }
    }
}