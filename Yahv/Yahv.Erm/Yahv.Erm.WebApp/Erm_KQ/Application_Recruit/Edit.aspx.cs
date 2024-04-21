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
    public partial class Edit : ErpParticlePage
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

            //部门负责人
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);

            var staff = staffs.SingleOrDefault(item => item.ID == Erp.Current.StaffID);
            if (staff != null)
            {
                if (staff.PostionCode == ((int)PostType.Manager).ToString() || staff.PostionCode == ((int)PostType.President).ToString())
                {
                    this.Model.ManageData = new
                    {
                        Department = staff.DepartmentCode,
                    };
                }
            }
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

        /// <summary>
        /// 提交
        /// </summary>
        protected void Submit()
        {
            try
            {
                #region 界面数据
                string ID = Request.Form["ID"];
                string Department = Request.Form["Department"];
                string PostionName = Request.Form["PostionName"];
                string WorkAddress = Request.Form["WorkAddress"];
                string NumberOfNeeds = Request.Form["NumberOfNeeds"];
                string NumberOfPositions = Request.Form["NumberOfPositions"];
                string NumberOfNow = Request.Form["NumberOfNow"];
                string NumberOfRecruiters = Request.Form["NumberOfRecruiters"];

                string PeriodSalary = Request.Form["PeriodSalary"];
                string NormalSalary = Request.Form["NormalSalary"];
                DateTime ExpectedArrivalTime = Convert.ToDateTime(Request.Form["ExpectedArrivalTime"]);
                //招聘途经
                string SocialRecruitment = Request.Form["SocialRecruitment"];
                string CampusRecruitment = Request.Form["CampusRecruitment"];
                string InternalTransfer = Request.Form["InternalTransfer"];
                string OtherWay = Request.Form["OtherWay"];
                //岗位需求
                string LeaveSupplement = Request.Form["LeaveSupplement"];
                string CoordinateSupplement = Request.Form["CoordinateSupplement"];
                string PostAddition = Request.Form["PostAddition"];
                string PostExpansion = Request.Form["PostExpansion"];

                string BussnessTripRequirement = Request.Form["BussnessTripRequirement"];
                string EmergentRequirement = Request.Form["EmergentRequirement"];
                string GenderRequirement = Request.Form["GenderRequirement"];
                string EducationRequirement = Request.Form["EducationRequirement"];

                string AgeRequirement = Request.Form["AgeRequirement"];
                string MajorRequirement = Request.Form["MajorRequirement"];
                string ExperienceRequirement = Request.Form["ExperienceRequirement"];
                string OtherRequirement = Request.Form["OtherRequirement"];
                string PositionDescription = Request.Form["PositionDescription"];

                #endregion

                Application application = Erp.Current.Erm.Applications[ID] ?? new Application();
                application.Title = "招聘申请";
                application.Context = new RecruitContext()
                {
                    DepartmentCode = Department,
                    PostionName = PostionName,
                    WorkAddress = WorkAddress,
                    NumberOfNeeds = NumberOfNeeds,
                    NumberOfPositions = NumberOfPositions,
                    NumberOfNow = NumberOfNow,
                    NumberOfRecruiters = NumberOfRecruiters,
                    PeriodSalary = PeriodSalary,
                    NormalSalary = NormalSalary,
                    ExpectedArrivalTime = ExpectedArrivalTime,
                    RecruitmentRoute = new RecruitmentRoute()
                    {
                        SocialRecruitment = SocialRecruitment == "true" ? true : false,
                        CampusRecruitment = CampusRecruitment == "true" ? true : false,
                        InternalTransfer = InternalTransfer == "true" ? true : false,
                        OtherWay = OtherWay == "true" ? true : false,
                    },
                    RecruitmentReason = new RecruitmentReason()
                    {
                        LeaveSupplement = LeaveSupplement == "true" ? true : false,
                        CoordinateSupplement = CoordinateSupplement == "true" ? true : false,
                        PostAddition = PostAddition == "true" ? true : false,
                        PostExpansion = PostExpansion == "true" ? true : false,
                    },
                    BussnessTripRequirement = BussnessTripRequirement,
                    EmergentRequirement = EmergentRequirement,
                    GenderRequirement = GenderRequirement,
                    EducationRequirement = EducationRequirement,
                    AgeRequirement = AgeRequirement,
                    MajorRequirement = MajorRequirement,
                    ExperienceRequirement = ExperienceRequirement,
                    OtherRequirement = OtherRequirement,
                    PositionDescription = PositionDescription,
                }.Json();
                application.ApplicantID = Erp.Current.ID;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.UnderApproval;
                application.ApplicationType = Services.ApplicationType.Recruit;

                application.Enter();

                //如果是总经理发起招聘，则直接到行政招聘步骤
                var Staff = Alls.Current.Staffs.SingleOrDefault(item => item.ID == Erp.Current.StaffID);
                if (Staff?.PostionCode == ((int)Services.Common.PostType.President).ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[application.ID];
                    apply.CurrentVoteStep.Summary = "同意";
                    apply.Approval(true);
                    apply.Approval(true);
                }
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 保存草稿
        /// </summary>
        protected void SaveDraft()
        {
            try
            {
                #region 界面数据
                string Department = Request.Form["Department"];
                string PostionName = Request.Form["PostionName"];
                string WorkAddress = Request.Form["WorkAddress"];
                string NumberOfNeeds = Request.Form["NumberOfNeeds"];
                string NumberOfPositions = Request.Form["NumberOfPositions"];
                string NumberOfNow = Request.Form["NumberOfNow"];
                string NumberOfRecruiters = Request.Form["NumberOfRecruiters"];

                string PeriodSalary = Request.Form["PeriodSalary"];
                string NormalSalary = Request.Form["NormalSalary"];
                DateTime ExpectedArrivalTime = Convert.ToDateTime(Request.Form["ExpectedArrivalTime"]);
                //招聘途经
                string SocialRecruitment = Request.Form["SocialRecruitment"];
                string CampusRecruitment = Request.Form["CampusRecruitment"];
                string InternalTransfer = Request.Form["InternalTransfer"];
                string OtherWay = Request.Form["OtherWay"];
                //岗位需求
                string LeaveSupplement = Request.Form["LeaveSupplement"];
                string CoordinateSupplement = Request.Form["CoordinateSupplement"];
                string PostAddition = Request.Form["PostAddition"];
                string PostExpansion = Request.Form["PostExpansion"];

                string BussnessTripRequirement = Request.Form["BussnessTripRequirement"];
                string EmergentRequirement = Request.Form["EmergentRequirement"];
                string GenderRequirement = Request.Form["GenderRequirement"];
                string EducationRequirement = Request.Form["EducationRequirement"];

                string AgeRequirement = Request.Form["AgeRequirement"];
                string MajorRequirement = Request.Form["MajorRequirement"];
                string ExperienceRequirement = Request.Form["ExperienceRequirement"];
                string OtherRequirement = Request.Form["OtherRequirement"];
                string PositionDescription = Request.Form["PositionDescription"];

                #endregion

                Application application = new Application();
                application.Title = "招聘申请";
                application.Context = new RecruitContext()
                {
                    DepartmentCode = Department,
                    PostionName = PostionName,
                    WorkAddress = WorkAddress,
                    NumberOfNeeds = NumberOfNeeds,
                    NumberOfPositions = NumberOfPositions,
                    NumberOfNow = NumberOfNow,
                    NumberOfRecruiters = NumberOfRecruiters,
                    PeriodSalary = PeriodSalary,
                    NormalSalary = NormalSalary,
                    ExpectedArrivalTime = ExpectedArrivalTime,
                    RecruitmentRoute = new RecruitmentRoute()
                    {
                        SocialRecruitment = SocialRecruitment == "true" ? true : false,
                        CampusRecruitment = CampusRecruitment == "true" ? true : false,
                        InternalTransfer = InternalTransfer == "true" ? true : false,
                        OtherWay = OtherWay == "true" ? true : false,
                    },
                    RecruitmentReason = new RecruitmentReason()
                    {
                        LeaveSupplement = LeaveSupplement == "true" ? true : false,
                        CoordinateSupplement = CoordinateSupplement == "true" ? true : false,
                        PostAddition = PostAddition == "true" ? true : false,
                        PostExpansion = PostExpansion == "true" ? true : false,
                    },
                    BussnessTripRequirement = BussnessTripRequirement,
                    EmergentRequirement = EmergentRequirement,
                    GenderRequirement = GenderRequirement,
                    EducationRequirement = EducationRequirement,
                    AgeRequirement = AgeRequirement,
                    MajorRequirement = MajorRequirement,
                    ExperienceRequirement = ExperienceRequirement,
                    OtherRequirement = OtherRequirement,
                    PositionDescription = PositionDescription,
                }.Json();
                application.ApplicantID = Erp.Current.ID;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.Draft;
                application.ApplicationType = Services.ApplicationType.Recruit;

                application.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}