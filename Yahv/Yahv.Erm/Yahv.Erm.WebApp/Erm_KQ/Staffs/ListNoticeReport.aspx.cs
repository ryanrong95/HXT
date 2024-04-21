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

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class ListNoticeReport : ErpParticlePage
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
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<StaffNoticeReportStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Logs_StaffApproval, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var staffs = Erp.Current.Erm.Logs_StaffApprovalAll
                .Where(item => item.ApprovalStep == StaffApprovalStep.Notice).Where(expression);

            var data = staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                ID = item.StaffID,
                Name = item.Staff.Name,
                Code = item.Staff.Code,
                Gender = item.Staff.Gender.GetDescription(),
                Age = item.Staff.Personal.Age,
                Education = item.Staff.Personal.Education,
                GraduatInstitutions = item.Staff.Personal.GraduatInstitutions,
                Mobile = item.Staff.Personal.Mobile,
                Email = item.Staff.Personal.Email,
                Status = item.NoticeReportStatus,
                StatusDec = item.NoticeReportStatus.GetDescription(),
                CreateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                ReportDate = item.Logs_StaffApprovalContext.ReportDate,
                AdminName = item.Admin?.RealName,
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }
        Expression<Func<Logs_StaffApproval, bool>> Predicate()
        {
            Expression<Func<Logs_StaffApproval, bool>> predicate = item => true;

            //查询参数
            var Name = Request.QueryString["Name"];
            var ApprovalStatus = Request.QueryString["ApprovalStatus"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Staff.Name.Contains(Name) || item.Staff.Code.Contains(Name));
            }
            if (!string.IsNullOrWhiteSpace(ApprovalStatus))
            {
                var status = (StaffNoticeReportStatus)Enum.Parse(typeof(StaffNoticeReportStatus), ApprovalStatus);
                predicate = predicate.And(item => item.NoticeReportStatus == status);
            }
            return predicate;
        }
    }
}