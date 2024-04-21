using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Application = Yahv.Erm.Services.Models.Origins.Application;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            //状态
            this.Model.Status = ExtendsEnum.ToDictionary<Services.ApplicationStatus>().Select(item => new { Value = item.Key, Text = item.Value });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //审批状态
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<Services.ApprovalStatus>()
                .Where(item => item.Value != Services.ApprovalStatus.Waiting.GetDescription())
                .Select(item => new { Value = item.Key, Text = item.Value });
            //历史员工
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Departure || item.Status == StaffStatus.Cancel || item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 列表数据
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var query = Erp.Current.Erm.ApplicationsRoll.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.Title,
                StatusName = item.ApplicationStatus.GetDescription(),
                item.ApplicationStatus,
                item.ApplicantID,
                item.CreatorID,
                item.ID,
                Entity = item.Context.JsonTo<Resignation>(),
                StepName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.VoteStep?.Name : "--",
                AdminName = item.ApplicationStatus == Services.ApplicationStatus.UnderApproval ? item.CurrentVoteStep?.Admin?.RealName : "--",
            });
        }
        #endregion

        #region 功能函数

        #region 功能函数
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
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "离职申请",
                    $"删除", del.Json());
            }
        }
        #endregion

        /// <summary>
        /// 下载交接表模板
        /// </summary>
        protected void btn_handover_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职交接表.docx";
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 下载申请表模板
        /// </summary>
        protected void btn_apply_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\") + "Template\\files\\员工离职申请表.xlsx";
            DownLoadFile(fileName);
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Application, bool>> GetExpression()
        {
            Expression<Func<Application, bool>> predicate = item => item.ApplicationType == ApplicationType.Leave;

            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item => item.ApplicantID == Erp.Current.ID || item.Steps.Any(t => t.AdminID == Erp.Current.ID));
            }

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
        #endregion
    }
}