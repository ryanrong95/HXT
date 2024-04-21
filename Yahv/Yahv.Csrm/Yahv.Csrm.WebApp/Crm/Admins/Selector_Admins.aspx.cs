using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Admins
{
    public partial class Selector_Admins : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            string salemanagerid = FixedRole.SaleManager.GetFixedID();
            string saleid = FixedRole.Sale.GetFixedID();
            //var sales = Erp.Current.Crm.Admins.Where(item => item.RoleID == salemanagerid
            //    || item.RoleID == saleid
            //    || item.RoleName == "销售兼盖章人").Where(query());
            var sales = new YaHv.Csrm.Services.Views.ConAdminsView().Search(FixedRole.SaleManager, FixedRole.Sale).Where(query());
            return new
            {
                rows = sales.OrderBy(item => item.RealName).ToArray().Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.UserName,
                    RoleName = item.RoleName
                })
            };
        }

        Expression<Func<Admin, bool>> query()
        {
            Expression<Func<Admin, bool>> predicate = item => item.StaffID != null;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.ID == name || item.RealName.Contains(name) || item.UserName.Contains(name));
            }
            return predicate;
        }

    }
}