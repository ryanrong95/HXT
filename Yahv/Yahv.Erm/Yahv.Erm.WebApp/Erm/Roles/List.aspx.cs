using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services.Extends;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Roles
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
            var query = Alls.Current.Roles;
            Expression<Func<Services.Models.Origins.Role, bool>> predicate = item => true;

            string name = Request.QueryString["s_name"];
            string type = Request.QueryString["s_type"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(type))
            {
                var roleType = (RoleType)int.Parse(type);
                predicate = predicate.And(item => item.Type == roleType);
            }

            return Paging(query.Where(predicate).OrderBy(t => t.ID).ToArray().Select(t => new
            {
                t.ID,
                StatusName = t.Status.GetDescription(),
                Status = t.Status,
                t.Name,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                IsSuper = t.Status == RoleStatus.Super,
                TypeName = t.Type.GetDescription(),
                Type = t.Type,
            }));
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        protected void delete()
        {
            var array = Request.Form["ids"].Split(',');
            Alls.Current.Roles.Where(t => array.Contains(t.ID)).Detele();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "权限管理",
                    $"删除", array.Json());
        }

        protected object GetType()
        {
            return ExtendsEnum.ToDictionary<RoleType>().Select(item => new { text = item.Value, value = item.Key });
        }
    }
}