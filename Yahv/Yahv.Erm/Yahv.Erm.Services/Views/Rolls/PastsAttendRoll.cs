using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Linq.Generic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 考勤统计视图
    /// </summary>
    public class PastsAttendRoll : UniqueView<PastsAttend, PvbErmReponsitory>
    {
        public PastsAttendRoll()
        {

        }

        public PastsAttendRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PastsAttend> GetIQueryable()
        {
            return new PastsAttendOrigin(Reponsitory);
        }

        /// <summary>
        /// 获取考勤结果
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public IQueryable<PastsAttend> GetPastsAttend(Expression<Func<PastsAttend, bool>> expression, params LambdaExpression[] expressions)
        {
            //过滤考勤结果
            var PastsAttend = this.GetIQueryable();
            foreach (var predicate in expressions)
            {
                PastsAttend = PastsAttend.AsQueryable().Where(predicate as Expression<Func<PastsAttend, bool>>);
            }
            var pasts = PastsAttend.Where(expression);
            return pasts;
        }

        /// <summary>
        /// 获取考勤最终结果
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public IQueryable<PastsAttendFinalResult> GetPastsAttendFinalResult(Expression<Func<PastsAttend, bool>> expression, params LambdaExpression[] expressions)
        {
            //过滤考勤结果
            var PastsAttend = this.GetIQueryable();
            foreach (var predicate in expressions)
            {
                PastsAttend = PastsAttend.AsQueryable().Where(predicate as Expression<Func<PastsAttend, bool>>);
            }
            var pasts = PastsAttend.Where(expression);
            var groupPasts = (from past in pasts
                              group past by new { past.Date, past.StaffID, past.SchedulingID } into groupPast
                              select new
                              {
                                  Date = groupPast.Key.Date,
                                  StaffID = groupPast.Key.StaffID,
                                  SchedulingID = groupPast.Key.SchedulingID,
                              }).ToArray();
            //获取考勤结果
            var staffView = new StaffsOrigin(Reponsitory);
            var scheduleView = new SchedulingsOrigin(Reponsitory);
            var pastAttendView = new PastsAttendOrigin(Reponsitory);
            var logAttendView = new Logs_AttendOrigin(Reponsitory);

            var linq = from entity in groupPasts
                       join staff in staffView on entity.StaffID equals staff.ID
                       join schedule in scheduleView on entity.SchedulingID equals schedule.ID
                       join passAttend in pastAttendView on new { entity.Date, entity.StaffID, entity.SchedulingID } equals new { passAttend.Date, passAttend.StaffID, passAttend.SchedulingID } into passAttends
                       join logAttend in logAttendView on new { entity.Date, entity.StaffID } equals new { logAttend.Date, logAttend.StaffID } into logAttends
                       select new PastsAttendFinalResult
                       {
                           Date = entity.Date,
                           SchedulingID = entity.SchedulingID,
                           StaffID = entity.StaffID,
                           PastsAttends = passAttends,
                           Logs_Attends = logAttends,
                           Scheduling = schedule,
                           Staff = staff,
                       };
            return linq.AsQueryable();
        }
    }
}