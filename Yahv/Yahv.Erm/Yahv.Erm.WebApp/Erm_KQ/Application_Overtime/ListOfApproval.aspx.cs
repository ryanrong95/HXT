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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Overtime
{
    public partial class ListOfApproval : ErpParticlePage
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
            //状态
            this.Model.Status = ExtendsEnum.ToDictionary<Services.ApplicationStatus>().Select(item => new { Value = item.Key, Text = item.Value });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //员工
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Application, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var applications = Erp.Current.Erm.ApplicationsRoll
                 .Where(item => item.ApplicationType == Services.ApplicationType.Overtime)
                 .Where(item => item.ApplicationStatus == Services.ApplicationStatus.UnderApproval)
                 .Where(expression);
            if (!Erp.Current.IsSuper)
            {
                applications = applications.Where(item => item.CurrentVoteStep.AdminID == Erp.Current.ID);
            }

            var data = applications.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.ApplicantID,
                ApplicantName = item.Applicant.RealName,
                DepartmentType = item.OverTimeContext.DepartmentName,
                Manager = item.OverTimeContext.ApproveName,
                Date = item.OverTimeContext.Date.ToString("yyyy-MM-dd"),
                Reason = item.OverTimeContext.Reason,
                Status = item.ApplicationStatus,
                StatusDec = item.ApplicationStatus.GetDescription(),
                Name = item.CurrentVoteStep.VoteStep.Name,
                AdminName = item.CurrentVoteStep.Admin.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }

        Expression<Func<Application, bool>> Predicate()
        {
            Expression<Func<Application, bool>> predicate = item => true;

            //查询参数
            var Status = Request.QueryString["Status"];
            var Staff = Request.QueryString["Staff"];
            var Department = Request.QueryString["Department"];
            var Date = Request.QueryString["Date"];

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var status = ((Services.ApplicationStatus)Enum.Parse(typeof(Services.ApplicationStatus), Status));
                predicate = predicate.And(item => item.ApplicationStatus == status);
            }
            if (!string.IsNullOrWhiteSpace(Staff))
            {
                predicate = predicate.And(item => item.ApplicantID == Staff);
            }
            if (!string.IsNullOrWhiteSpace(Department))
            {
                var department = (DepartmentType)Enum.Parse(typeof(DepartmentType), Department);
                predicate = predicate.And(item => item.Context.Contains(department.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(Date))
            {
                predicate = predicate.And(item => item.Context.Contains(Date));
            }
            return predicate;
        }

    }
}