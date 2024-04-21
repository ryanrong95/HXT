using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.WmsAdmins
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Admin, bool>> predicate = null;

            var query = Alls.Current.AdminsPfWms.Where(item => true);

            var s_name = Request.QueryString["s_name"];
            if (!string.IsNullOrWhiteSpace(s_name))
            {
                s_name = s_name.Trim();

                predicate = predicate.And(item => item.RealName.Contains(s_name) || item.RealName.Contains(s_name) || item.UserName.Contains(s_name));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return this.Paging(query.OrderBy(t => t.ID), t => new
            {
                t.ID,
                Status = t.Status,
                StatusName = t.Status.GetDescription(),
                t.RoleID,
                t.RealName,
                t.StaffID,
                t.SelCode,
                t.UserName,
                t.RoleName,
                IsSuper = t.Status == AdminStatus.Super,
                LastLoginDate = t.LastLoginDate.HasValue ? t.LastLoginDate.Value.ToString() : "",
            });
        }
    }
}