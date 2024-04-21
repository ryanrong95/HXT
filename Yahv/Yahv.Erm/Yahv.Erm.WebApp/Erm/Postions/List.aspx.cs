using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Postions
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
            Expression<Func<Postion, bool>> predicate = null;

            var query = Alls.Current.Postions.Where(item => true);

            var s_name = Request.QueryString["s_name"];
            if (!string.IsNullOrWhiteSpace(s_name))
            {
                s_name = s_name.Trim();
                predicate = predicate.And(item => item.Name.Contains(s_name));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return new
            {
                rows = query.OrderBy(t => t.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Name,
                    Status = item.Status.GetDescription(),
                    item.AdminID,
                    item.AdminName,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                })
            };
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        protected void delete()
        {
            var array = Request.Form["ids"].Split(',');
            Alls.Current.Postions.Where(t => array.Contains(t.ID)).Detele();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                nameof(Yahv.Systematic.Erm),
                "岗位管理",
                $"删除岗位({array.Length}个)",
                array.Json());
        }
    }
}