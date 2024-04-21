//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using YaHv.Csrm.Services.Models.Origins;

//namespace Yahv.Csrm.WebApp.Crm.WsSuppliers
//{
//    public partial class Selector : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {

//        }
//        protected object data()
//        {
//            var query = Erp.Current.Srm.WsSuppliers.Where(Predicate());

//            return this.Paging(query.ToArray().Select(item => new
//            {
//                item.ID,
//                Name = item.Enterprise.Name,
//                item.ChineseName,
//                item.EnglishName,
//                item.Enterprise.AdminCode,
//                item.Grade,
//                item.Enterprise.District,
//                Admin = item.Admin == null ? null : item.Admin.RealName,
//                item.Enterprise.Uscc,
//                item.Enterprise.Corporation,
//                item.Enterprise.RegAddress
//            }));
//        }
//        Expression<Func<WsSupplier, bool>> Predicate()
//        {
//            Expression<Func<WsSupplier, bool>> predicate = item => item.WsSupplierStatus == ApprovalStatus.Normal;
//            var name = Request["s_name"];
//            if (!string.IsNullOrWhiteSpace(name))
//            {
//                predicate = predicate.And(item => item.Enterprise.Name.Contains(name) || item.ChineseName.Contains(name) || item.EnglishName.Contains(name));
//            }

//            return predicate;
//        }
//    }
//}