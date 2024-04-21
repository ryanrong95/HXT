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

namespace Yahv.Csrm.WebApp.Srm.Admins
{
    /// <summary>
    /// 销售人员
    /// </summary>
    public partial class Selector_Admins : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var purchasers = Yahv.Erp.Choice[FixedRole.PurchasingManager, FixedRole.Purchaser].AsQueryable<Models.ErpAdmin>().Where(Predicate());
            return new
            {
                rows = purchasers.OrderBy(item => item.RealName).ToArray().Select(item => new
                {
                    item.ID,
                    item.RealName,
                    item.UserName,
                    RoleName = item.Role.Name
                })
            };
        }
        Expression<Func<Models.ErpAdmin, bool>> Predicate()
        {
            Expression<Func<Models.ErpAdmin, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.ID == name || item.RealName.Contains(name) || item.UserName.Contains(name));
            }
            return predicate;
        }
    }
}