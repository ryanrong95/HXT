using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Recruit
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        protected void LoadComboBoxData()
        {
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
        }

        protected void LoadData()
        {
            //申请信息
            string ApplicationID = Request.QueryString["ID"];
            this.Model.ApplicationData = null;
            if (!string.IsNullOrEmpty(ApplicationID))
            {
                var application = Erp.Current.Erm.Applications.Single(item => item.ID == ApplicationID);
                this.Model.ApplicationData = new
                {
                    ApplicantID = application.ApplicantID,
                    DepartmentCode = application.RecruitContext.DepartmentCode,
                    PostionName = application.RecruitContext.PostionName,
                    WorkAddress = application.RecruitContext.WorkAddress,
                    NumberOfNeeds = application.RecruitContext.NumberOfNeeds,
                    NumberOfPositions = application.RecruitContext.NumberOfPositions,
                    NumberOfNow = application.RecruitContext.NumberOfNow,
                    NumberOfRecruiters = application.RecruitContext.NumberOfRecruiters,
                    PeriodSalary = application.RecruitContext.PeriodSalary,
                    NormalSalary = application.RecruitContext.NormalSalary,
                    ExpectedArrivalTime = application.RecruitContext.ExpectedArrivalTime.ToString("yyyy-MM-dd"),
                    BussnessTripRequirement = application.RecruitContext.BussnessTripRequirement,
                    EmergentRequirement = application.RecruitContext.EmergentRequirement,
                    GenderRequirement = application.RecruitContext.GenderRequirement,
                    EducationRequirements = application.RecruitContext.EducationRequirement,
                    AgeRequirement = application.RecruitContext.AgeRequirement,
                    MajorRequirement = application.RecruitContext.MajorRequirement,
                    ExperienceRequirement = application.RecruitContext.ExperienceRequirement,
                    OtherRequirement = application.RecruitContext.OtherRequirement,
                    PositionDescription = application.RecruitContext.PositionDescription,

                    SocialRecruitment = application.RecruitContext.RecruitmentRoute.SocialRecruitment,
                    CampusRecruitment = application.RecruitContext.RecruitmentRoute.CampusRecruitment,
                    InternalTransfer = application.RecruitContext.RecruitmentRoute.InternalTransfer,
                    OtherWay = application.RecruitContext.RecruitmentRoute.OtherWay,

                    LeaveSupplement = application.RecruitContext.RecruitmentReason.LeaveSupplement,
                    CoordinateSupplement = application.RecruitContext.RecruitmentReason.CoordinateSupplement,
                    PostAddition = application.RecruitContext.RecruitmentReason.PostAddition,
                    PostExpansion = application.RecruitContext.RecruitmentReason.PostExpansion,
                };
            }
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            var id = Request.QueryString["ID"];
            var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.ApplicationID == id);

            var data = logs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.Admin.RealName,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                item.VoteStepName,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }
    }
}