using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Application = Yahv.Erm.Services.Models.Origins.Application;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class ListOfApproval : ErpParticlePage
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
            var query = Erp.Current.Erm.ApprovalsStatistics.Where(GetExpression()).ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                Entity = item.Context.JsonTo<Resignation>(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                item.Title,
                item.ApplicantID,
                item.CreatorID,
                item.Applicant,
                Status = $"{item.VoteStepName}({item.ApproveName})",
                StatusDec = item.Status.GetDescription(),
                item.VoteStepName,
                item.ApproveName,
                item.ApplicationID,
                item.Uri,
            });
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<ApprovalStatistic, bool>> GetExpression()
        {
            Expression<Func<ApprovalStatistic, bool>> predicate = item => item.Type == ApplicationType.Leave;

            if (!Erp.Current.IsSuper)
            {
                predicate = predicate.And(item => item.ApproveID == Erp.Current.ID);
            }

            //查询参数
            var Staff = Request.QueryString["Staff"];
            var Department = Request.QueryString["Department"];

            if (!string.IsNullOrWhiteSpace(Staff))
            {
                predicate = predicate.And(item => item.ApplicantID == Staff);
            }
            if (!string.IsNullOrWhiteSpace(Department))
            {
                var department = (DepartmentType)Enum.Parse(typeof(DepartmentType), Department);
                predicate = predicate.And(item => item.Context.Contains(department.ToString()));
            }
            return predicate;
        }
        #endregion
    }
}