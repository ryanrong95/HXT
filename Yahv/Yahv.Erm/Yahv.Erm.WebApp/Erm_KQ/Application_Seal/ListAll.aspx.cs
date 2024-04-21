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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Seal
{
    public partial class ListAll : ErpParticlePage
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
            //审批状态
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<Services.ApprovalStatus>()
                .Where(item => item.Value != Services.ApprovalStatus.Waiting.GetDescription())
                .Select(item => new { Value = item.Key, Text = item.Value });
            //员工
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
            this.Model.CurrentAdmin = new
            {
                ID = Erp.Current.IsSuper ? "" : Erp.Current.ID,
            };
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
                .Where(item => item.ApplicationType == Services.ApplicationType.SealBorrow)
                .Where(expression);
            if (!Erp.Current.IsSuper)
            {
                //判断是否是总经理,总经理能看到所有人的
                var staff = Alls.Current.Staffs[Erp.Current.StaffID];
                if (staff != null && staff.PostionCode != ((int)Services.Common.PostType.President).ToString())
                {
                    applications = applications.Where(item => item.ApplicantID == Erp.Current.ID ||
                    item.Steps.Any(t => t.AdminID == Erp.Current.ID));
                }
            }
            var total = applications.Count();
            var data = applications.OrderByDescending(item => item.CreateDate).Skip((page - 1) * rows).Take(rows).ToArray();
            var result = data.Select(item => new
            {
                item.ID,
                item.ApplicantID,
                ApplicantName = item.Applicant.RealName,
                DepartmentType = item.SealBorrowContext.DepartmentName,
                Manager = item.SealBorrowContext.ApproveName,
                SealType = item.SealBorrowContext.SealType,
                SealBorrowType = item.SealBorrowContext.SealBorrowType,
                BorrowDate = item.SealBorrowContext.BorrowDate.ToString("yyyy-MM-dd"),
                ReturnDate = item.SealBorrowContext.ReturnDate.ToString("yyyy-MM-dd"),
                Reason = item.SealBorrowContext.Reason,
                Status = item.ApplicationStatus,
                StatusDec = item.ApplicationStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                StepName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.VoteStep?.Name : "--",
                AdminName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.Admin?.RealName : "--",
                AdminID = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.Admin?.ID : "",
            });

            return new
            {
                rows = result,
                total = total,
            };
        }
        Expression<Func<Application, bool>> Predicate()
        {
            Expression<Func<Application, bool>> predicate = item => true;

            //查询参数
            var Status = Request.QueryString["Status"];
            var Staff = Request.QueryString["Staff"];
            var Department = Request.QueryString["Department"];
            var ApprovalStatus = Request.QueryString["ApprovalStatus"];

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
            if (!string.IsNullOrWhiteSpace(ApprovalStatus))
            {
                if (!Erp.Current.IsSuper)
                {
                    var status = (Services.ApprovalStatus)Enum.Parse(typeof(Services.ApprovalStatus), ApprovalStatus);
                    var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.Status == status);
                    logs = logs.Where(item => item.AdminID == Erp.Current.ID);
                    var ids = logs.Select(item => item.ApplicationID).ToArray();
                    predicate = predicate.And(item => ids.Contains(item.ID));
                }
            }
            return predicate;
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Erp.Current.Erm.Applications[id];
            if (del != null)
            {
                del.Abandon();
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "加班申请",
                    $"删除", del.Json());
            }
        }    

    }
}