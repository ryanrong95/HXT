using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;
using Yahv.Underly.Enums;

namespace Yahv.AttendanceData.Import.Services
{
    /// <summary>
    /// 员工假期数据维护
    /// </summary>
    public class VacationService : IDataService
    {
        #region 从数据库读取的数据

        Layers.Data.Sqls.PvbErm.Applications[] overtimeApplys;
        Layers.Data.Sqls.PvbErm.SchedulesPrivate[] schedulesPrivate;

        #endregion

        #region 需要保存的数据

        List<Layers.Data.Sqls.PvbErm.Vacations> vacations;

        #endregion

        /// <summary>
        /// 数据读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IDataService Read(string path = null)
        {
            using (var reponsitory = new PvbErmReponsitory())
            {
                //2020年开始时间
                DateTime date = DateTime.Parse("2020-01-01");

                //2020年的加班申请
                overtimeApplys = (from entity in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Applications>()
                                  join voteFlow in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>() on entity.VoteFlowID equals voteFlow.ID
                                  where entity.CreateDate >= date && voteFlow.Type == (int)ApplicationType.Overtime
                                  select entity).ToArray();

                //2020年的员工日程安排
                schedulesPrivate = (from entity in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.SchedulesPrivate>()
                                    join application in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Applications>() on entity.ApplicationID equals application.ID
                                    where application.CreateDate >= date
                                    select entity).ToArray();
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public IDataService Encapsule()
        {
            using (var reponsitory = new PvbErmReponsitory())
            {
                var names = DataManager.Current.XdtStaffs.Select(item => item.Name).ToArray();
                var staffs = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>().Where(item => names.Contains(item.Name))
                    .Select(item => new { item.ID, item.Name, item.DyjCode }).ToArray();

                vacations = new List<Layers.Data.Sqls.PvbErm.Vacations>();
                foreach (var staff in staffs)
                {
                    var adminID = DataManager.Current.XdtStaffs.Single(item => item.Name == staff.Name).AdminID;
                    var annualLeaveDays = schedulesPrivate.Count(item => item.Type == (int)SchedulePrivateType.AnnualLeave && item.StaffID == staff.ID) * 0.5m;
                    var leaveInLieuDays = schedulesPrivate.Count(item => item.Type == (int)SchedulePrivateType.LeaveInLieu && item.StaffID == staff.ID) * 0.5m;
                    var overtimeDays = overtimeApplys.Count(item => item.ApplicantID == adminID);

                    //年假:
                    //vacations.Add(new Layers.Data.Sqls.PvbErm.Vacations()
                    //{
                    //    StaffID = staff.ID,
                    //    Type = (int)VacationType.YearsDay,
                    //    Lefts = 5m - annualLeaveDays
                    //});
                    //调休:
                    vacations.Add(new Layers.Data.Sqls.PvbErm.Vacations()
                    {
                        StaffID = staff.ID,
                        Type = (int)VacationType.OffDay,
                        Lefts = overtimeDays - leaveInLieuDays
                    });
                }
            }

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            using (var reponsitory = new PvbErmReponsitory())
            {
                foreach (var vacation in vacations)
                {
                    Expression<Func<Layers.Data.Sqls.PvbErm.Vacations, bool>> predication = item => item.StaffID == vacation.StaffID && item.Type == vacation.Type;
                    if (reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Vacations>().Any(predication))
                    {
                        reponsitory.Update(new { Lefts = vacation.Lefts }, predication);
                    }
                    else
                    {
                        vacation.ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.Vacation);
                        reponsitory.Insert(vacation);
                    }
                }
            }
        }
    }
}
