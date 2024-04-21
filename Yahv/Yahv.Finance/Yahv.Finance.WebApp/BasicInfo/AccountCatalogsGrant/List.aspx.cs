using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.AccountCatalogsGrant
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

            var query = Erp.Current.Finance.Admins.Where(item => true);

            var s_name = Request.QueryString["s_name"];
            if (!string.IsNullOrWhiteSpace(s_name))
            {
                s_name = s_name.Trim();
                predicate = predicate.And(item => item.UserName.Contains(s_name) || item.RealName.Contains(s_name));
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
                //t.DyjCode,
                IsSuper = t.Status == AdminStatus.Super,
                //CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                LastLoginDate = t.LastLoginDate.HasValue ? t.LastLoginDate.Value.ToString() : "",
                //t.StaffStatus,
            });
        }
    }
}