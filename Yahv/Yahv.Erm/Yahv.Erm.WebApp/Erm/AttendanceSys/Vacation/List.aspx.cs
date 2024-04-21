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
using Yahv.Erm.Services;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.WebApp.Erm.AttendanceSys.Vacation
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.VacationType = ExtendsEnum.ToArray<VacationType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }

        protected object data()
        {
            var query = Erp.Current.Erm.Vacations.Where(Predicate());

            //return this.Paging(query.OrderBy(t => t.Date), t => new
            //{
            //    t.ID,
            //    t.Date,
            //    Type = t.Type.GetDescription(),
            //    t.Salary,
            //    t.Summary,
            //});
            return null;
        }

        Expression<Func<Services.Models.Origins.Vacation, bool>> Predicate()
        {
            Expression<Func<Services.Models.Origins.Vacation, bool>> predicate = item => true;

            //查询参数
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            //快速筛选参数
            var Type = Request.QueryString["Type"];

            if (!string.IsNullOrWhiteSpace(Type))
            {
                var type = int.Parse(Type.Trim());
                predicate = predicate.And(item => (int)item.Type == type);
            }
            //if (!string.IsNullOrWhiteSpace(StartDate))
            //{
            //    var start = Convert.ToDateTime(StartDate);
            //    predicate = predicate.And(item => item.Date >= start);
            //}
            //if (!string.IsNullOrWhiteSpace(EndDate))
            //{
            //    var end = Convert.ToDateTime(EndDate).AddDays(1);
            //    predicate = predicate.And(item => item.Date < end);
            //}
            return predicate;
        }

        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}