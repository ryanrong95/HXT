using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Admins
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

            var xdtStaffIDs = Erp.Current.Erm.XdtStaffs.Select(item => item.ID).ToArray();

            var query = Alls.Current.Admins.Where(item => xdtStaffIDs.Contains(item.StaffID));

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
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                LastLoginDate = t.LastLoginDate.HasValue ? t.LastLoginDate.Value.ToString() : "",
                t.StaffStatus,
            });
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void disable()
        {
            var array = Request.Form["items"].Split(',');
            Alls.Current.Admins.Where(t => array.Contains(t.ID)).Disable();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"停用管理员({array.Length}个)", array.Json());
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void enable()
        {
            var array = Request.Form["items"].Split(',');
            Alls.Current.Admins.Where(t => array.Contains(t.ID)).Enable();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"启用管理员({array.Length}个)", array.Json());
        }

        /// <summary>
        /// 初始化密码
        /// </summary>
        protected void initpassword()
        {
            var array = Request.Form["items"].Split(',');
            Alls.Current.Admins.Where(t => array.Contains(t.ID)).InitPassword();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "管理员管理", $"初始化密码({array.Length}个)", array.Json());
        }
    }
}