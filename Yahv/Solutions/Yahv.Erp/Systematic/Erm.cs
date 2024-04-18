using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// Erm领域 人事关系
    /// </summary>
    public class Erm : IAction
    {
        private IErpAdmin admin;
        public Erm(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region Views

        public IQueryable<Yahv.Erm.Services.Models.Origins.Staff> XdtStaffs
        {
            get
            {
                return new Yahv.Erm.Services.Views.StaffAlls()
                    .Where(item => item.Labour.EnterpriseID == Yahv.Erm.Services.Common.ErmConfig.LabourEnterpriseID
                    || item.Labour.EnterpriseID == Yahv.Erm.Services.Common.ErmConfig.LabourEnterpriseID2);
            }
        }

        public IQueryable<Yahv.Erm.Services.Models.Origins.Postion> XdtPostions
        {
            get
            {
                return new Yahv.Erm.Services.Views.PostionsAll()
                    .Where(item => Yahv.Erm.Services.Common.XdtInfoHelper.GetConfig().Positions.Contains(item.Name));
            }
        }

        /// <summary>
        /// 管理员视图
        /// </summary>
        public Yahv.Erm.Services.Views.Rolls.AdminsRoll Admins
        {
            get
            {
                return new Yahv.Erm.Services.Views.Rolls.AdminsRoll();
            }
        }

        /// <summary>
        /// 我的录入项
        /// </summary>
        public Yahv.Erm.Services.Views.MyInputItems MyInputItems
        {
            get { return new Yahv.Erm.Services.Views.MyInputItems(admin); }
        }

        /// <summary>
        /// 历史累计值
        /// </summary>
        public Yahv.Erm.Services.Views.PastsItemAll PastItems
        {
            get { return new Yahv.Erm.Services.Views.PastsItemAll(); }
        }

        /// <summary>
        /// 我的库房
        /// </summary>
        public Yahv.Erm.Services.Views.Rolls.MyWareHouseRoll MyWareHouses
        {
            get { return new Yahv.Erm.Services.Views.Rolls.MyWareHouseRoll(admin); }
        }

        /// <summary>
        /// 假期视图
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.VacationsOrigin Vacations
        {
            get
            {
                return new Yahv.Erm.Services.Views.Origins.VacationsOrigin();
            }
        }

        /// <summary>
        /// 自然日视图
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.CalendarsOrigin Calendars
        {
            get { return new Yahv.Erm.Services.Views.Origins.CalendarsOrigin(); }
        }

        /// <summary>
        /// 公共日程安排视图
        /// </summary>
        public Yahv.Erm.Services.Views.Rolls.SchedulesPublicRoll SchedulesPublic
        {
            get { return new Yahv.Erm.Services.Views.Rolls.SchedulesPublicRoll(); }
        }

        /// <summary>
        /// 私人日程安排视图
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.SchedulesPrivateOrigin SchedulesPrivate
        {
            get { return new Yahv.Erm.Services.Views.Origins.SchedulesPrivateOrigin(); }
        }

        /// <summary>
        /// 所属班别
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.SchedulingsOrigin Schedulings
        {
            get { return new Yahv.Erm.Services.Views.Origins.SchedulingsOrigin(); }
        }

        /// <summary>
        /// 区域
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.RegionsAcOrigin RegionsAc
        {
            get { return new Yahv.Erm.Services.Views.Origins.RegionsAcOrigin(); }
        }

        /// <summary>
        /// 申请
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.ApplicationsAll Applications
        {
            get { return new Yahv.Erm.Services.Views.Origins.ApplicationsAll(); }
        }

        /// <summary>
        /// 申请
        /// </summary>
        public Yahv.Erm.Services.Views.Roll.ApplicationsRoll ApplicationsRoll
        {
            get { return new Yahv.Erm.Services.Views.Roll.ApplicationsRoll(); }
        }

        /// <summary>
        /// 申请的审批日志
        /// </summary>
        public Yahv.Erm.Services.Views.Origins.Logs_ApplyVoteStepsAll Logs_ApplyVoteSteps
        {
            get { return new Yahv.Erm.Services.Views.Origins.Logs_ApplyVoteStepsAll(); }
        }

        /// <summary>
        /// 审批列表
        /// </summary>
        public Yahv.Erm.Services.Views.ApprovalsStatisticsView ApprovalsStatistics
        {
            get { return new Yahv.Erm.Services.Views.ApprovalsStatisticsView(); }
        }

        /// <summary>
        /// 考勤结果统计视图
        /// </summary>
        public Yahv.Erm.Services.Views.Rolls.PastsAttendRoll PastsAttendRoll
        {
            get { return new Yahv.Erm.Services.Views.Rolls.PastsAttendRoll(); }
        }

        /// <summary>
        /// 员工入职前的审批日志
        /// </summary>
        public Yahv.Erm.Services.Views.Alls.Logs_StaffApprovalAll Logs_StaffApprovalAll
        {
            get
            {
                return new Yahv.Erm.Services.Views.Alls.Logs_StaffApprovalAll();
            }
        }

        /// <summary>
        /// 印章证照
        /// </summary>
        public Yahv.Erm.Services.Views.Alls.SealCertificateAll SealCertificates
        {
            get
            {
                return new Yahv.Erm.Services.Views.Alls.SealCertificateAll();
            }
        }

        /// <summary>
        /// 轮班员工
        /// </summary>
        public Yahv.Erm.Services.Views.Alls.ShiftStaffsAll ShiftStaffs
        {
            get
            {
                return new Yahv.Erm.Services.Views.Alls.ShiftStaffsAll();
            }
        }

        #endregion

        #region Action
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="log"></param>
        public void Logs_Error(Logs_Error log)
        {
            using (var repository = new HvRFQReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.PvbErm.Logs_Errors>(new Layers.Data.Sqls.PvbErm.Logs_Errors
                {
                    AdminID = admin.ID,
                    Page = log.Page,
                    Message = log.Message,
                    Codes = log.Codes,
                    Source = log.Source,
                    Stack = log.Stack,
                    CreateDate = log.CreateDate
                });
            }
        }
        #endregion
    }
}
