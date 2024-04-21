using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Shifts
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
            var query = Erp.Current.Erm.ShiftStaffs.Where(GetExpression()).OrderBy(item => item.NextSchedulingID).ToArray();
            return from entity in query
                   select new
                   {
                       entity.ID,
                       StaffName = entity.Staff.Name,
                       SelCode = entity.Staff.SelCode,
                       DepartmentCode = string.IsNullOrEmpty(entity.Staff.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), entity.Staff.DepartmentCode)).GetDescription(),
                       Current = entity.CurrentScheduling.Name,
                       Next = entity.NextScheduling.Name,
                       CreateDate = entity.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       UpdateDate = entity.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       Creator = entity.Creator.RealName,
                       Modify = entity.Modify?.RealName,
                   };
        }
        #endregion

        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];
                var shift = Erp.Current.Erm.ShiftStaffs.Single(item => item.ID == ID);
                shift.Abandon();
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
        private Expression<Func<ShiftStaff, bool>> GetExpression()
        {
            Expression<Func<ShiftStaff, bool>> predicate = item => true;
            string name = Request.QueryString["name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Staff.Name.Contains(name) || item.Staff.SelCode.Contains(name));
            }
            return predicate;
        }
        #endregion
    }
}