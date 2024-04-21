using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Schedulings
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region 加载数据
        /// <summary>
        /// 加载列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = Erp.Current.Erm.Schedulings.Where(GetExpression()).ToArray();
            return from entity in query
                   select new
                   {
                       entity.ID,
                       entity.Name,
                       entity.AmStartTime,
                       entity.AmEndTime,
                       entity.PmStartTime,
                       entity.PmEndTime,
                       entity.DomainValue,
                       entity.Summary,
                       entity.IsMain,
                   };
        }
        #endregion

        protected void Delete()
        {
            try
            {
                string id = Request.Form["ID"];
                var SchedulesPublic = Erp.Current.Erm.SchedulesPublic.Where(item => item.ShiftID == id || item.SchedulingID == id);
                var staffs = Alls.Current.Staffs.Where(item => item.SchedulingID == id);
                if (SchedulesPublic.Count() > 0)
                {
                    throw new Exception("班别已经在日期管理中使用。");
                }
                if (staffs.Count() > 0)
                {
                    throw new Exception("班别已经在员工信息中使用。");
                }
                //删除班别
                var del = Erp.Current.Erm.Schedulings[id];
                if (del != null)
                {
                    del.Abandon();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "班别信息",
                        $"删除", del.Json());
                }
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败:" + ex.Message }).Json());
            }
        }

        #region 自定义函数
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Scheduling, bool>> GetExpression()
        {
            Expression<Func<Scheduling, bool>> predicate = item => true;

            string name = Request.QueryString["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            return predicate;
        }
        #endregion
    }
}