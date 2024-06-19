using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Underly;
using Yahv.Linq.Extends;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using Yahv.Erm.Services.Models.Origins;
using Layers.Data;
using Yahv.Utils;
using System.Data;
using Yahv.Utils.Serializers;
using System.Linq.Expressions;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Views.Rolls;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class ListVacation : ErpParticlePage
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
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<StaffVacation_Show, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);
            string years = Request.QueryString["Years"];
            int Years = string.IsNullOrEmpty(years) ? DateTime.Now.Year : int.Parse(years);

            var staffs = new StaffVacations_Xdt(Years).Where(expression);

            var data = staffs.Select(item => new
            {
                item.ID,
                item.Code,
                item.SelCode,
                item.Name,
                PostionName = item.Postion.Name,
                DepartmentCode = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                item.TotalYearDay,
                item.UsedYearDays,
                RemainYearDays = item.TotalYearDay - item.UsedYearDays,
                item.TotalOffDay,
                item.UsedOffDays,
                item.TotalSickDay,
                item.UsedSickDays,
                item.CasualLeaveDays,
                item.OfficialBusinessDays,
                item.BusinessTripDays,
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }

        Expression<Func<StaffVacation_Show, bool>> Predicate()
        {
            Expression<Func<StaffVacation_Show, bool>> predicate = item => true;
            //查询参数
            var Name = Request.QueryString["Name"];
            var DepartmentType = Request.QueryString["DepartmentType"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Name.Contains(Name) || item.Code.Contains(Name) || item.SelCode.Contains(Name));
            }
            if (!string.IsNullOrWhiteSpace(DepartmentType))
            {
                predicate = predicate.And(item => item.DepartmentCode == DepartmentType);
            }
            return predicate;
        }

        /// <summary>
        /// 初始化员工假期
        /// </summary>
        protected void InitVacation()
        {
            try
            {
                var staffs = new Services.Views.StaffAlls()
                    .Where(item => item.Labour.EnterpriseID == ErmConfig.LabourEnterpriseID)
                    .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray();
                foreach (var staff in staffs)
                {
                    staff.InitVacation();
                }

                Response.Write((new { success = true, message = "初始化成功" }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "初始化失败:" + ex.Message }).Json());
            }
        }
    }
}