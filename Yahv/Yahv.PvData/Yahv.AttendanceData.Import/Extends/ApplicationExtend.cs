using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Extends
{
    /// <summary>
    /// 申请扩展方法
    /// </summary>
    public static class ApplicationExtend
    {
        /// <summary>
        /// 申请
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbErm.Applications ToLinq(this Models.MyApplication myApply)
        {
            return new Layers.Data.Sqls.PvbErm.Applications()
            {
                ID = myApply.ID,
                VoteFlowID = myApply.VoteFlow.ID,
                Title = myApply.Title,
                Context = myApply.Context,
                ApplicantID = myApply.ApplicantID,
                CreatorID = myApply.CreatorID,
                CreateDate = myApply.CreateDate,
                Status = (int)myApply.Status
            };
        }

        /// <summary>
        /// 申请审批步骤
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static List<Layers.Data.Sqls.PvbErm.ApplyVoteSteps> ToApplyVoteSteps(this Models.MyApplication myApply)
        {
            var date = myApply.CreateDate.Date;
            var voteSteps = DataManager.Current.VoteFlows.Single(item => item.ID == myApply.VoteFlow.ID).Steps;
            var ids = Underly.PKeyType.ApplyVoteStep.Pick(voteSteps.Length);
            return voteSteps.Select((item, index) => new Layers.Data.Sqls.PvbErm.ApplyVoteSteps()
            {
                ID = ids[index],
                ApplicationID = myApply.ID,
                VoteStepID = item.ID,
                IsCurrent = false,
                AdminID = item.OrderIndex == 1 ? myApply.ManagerID : item.AdminID,
                Status = (int)ApprovalStatus.Agree,
                Summary = "同意申请",
                CreateDate = myApply.CreateDate,
                ModifyDate = Utils.DateUtil.GetAmStartTime(date.Year, date.Month, date.Day).AddHours(2 + index)
            }).ToList();
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static List<Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps> ToLogs_ApplyVoteSteps(this Models.MyApplication myApply, List<Layers.Data.Sqls.PvbErm.ApplyVoteSteps> steps)
        {
            var voteSteps = DataManager.Current.VoteFlows.Single(item => item.ID == myApply.VoteFlow.ID).Steps;
            var ids = Underly.PKeyType.ApplyVoteStepLog.Pick(voteSteps.Length);
            return voteSteps.Select((item, index) => new Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps()
            {
                ID = ids[index],
                ApplicationID = myApply.ID,
                VoteStepID = item.ID,
                AdminID = item.OrderIndex == 1 ? myApply.ManagerID : item.AdminID,
                Status = (int)ApprovalStatus.Agree,
                Summary = "同意申请",
                CreateDate = steps.Single(step => step.ApplicationID == myApply.ID && step.VoteStepID == item.ID).ModifyDate,
            }).ToList();
        }

        /// <summary>
        /// 受众
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbErm.MapsAppStaff ToMapsAppStaff(this Models.MyApplication myApply)
        {
            return new Layers.Data.Sqls.PvbErm.MapsAppStaff()
            {
                ApplicationID = myApply.ID,
                StaffID = myApply.StaffID
            };
        }

        /// <summary>
        /// 日程安排
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbErm.Schedules ToSchedule(this Models.MyApplication myApply)
        {
            return new Layers.Data.Sqls.PvbErm.Schedules()
            {
                ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.Sched),
                Date = myApply.AttendDate.Date,
                Type = (int)Underly.Enums.ScheduleType.Private,
                CreatorID = "NPC",
                CreateDate = myApply.UpdateDate,
                ModifyDate = myApply.UpdateDate
            };
        }

        /// <summary>
        /// 个人日程安排
        /// </summary>
        /// <param name="myApply">申请数据</param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbErm.SchedulesPrivate ToSchedulePrivate(this Models.MyApplication myApply, string scheduleID, string AmOrPm)
        {
            return new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
            {
                ID = scheduleID,
                ApplicationID = myApply.ID,
                Type = (int)myApply.PrivateType,
                AmOrPm = AmOrPm,
                StaffID = myApply.StaffID,
                CreateDate = myApply.UpdateDate
            };
        }
    }
}
