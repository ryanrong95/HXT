using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 个人日程安排
    /// </summary>
    public class SchedulePrivate : Schedule
    {
        #region 属性

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        public SchedulePrivateType SchedulePrivateType { get; set; }

        /// <summary>
        /// 上下午枚举：Am Or  Pm
        /// </summary>
        public string AmOrPm { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 上班补签
        /// </summary>
        public bool? OnWorkRemedy { get; set; }

        /// <summary>
        /// 下班补签
        /// </summary>
        public bool? OffWorkRemedy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Sched);

                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Schedules()
                    {
                        ID = this.ID,
                        Date = this.Date,
                        Type = (int)this.Type,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        ModifyDate = this.ModifyDate,
                        ModifyID = this.ModifyID,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPrivate()
                    {
                        ID = this.ID,
                        ApplicationID = this.ApplicationID,
                        Type = (int)this.SchedulePrivateType,
                        AmOrPm = this.AmOrPm.ToString(),
                        StaffID = this.StaffID,
                        CreateDate = DateTime.Now,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Schedules>(new
                    {
                        Date = this.Date,
                        Type = (int)this.Type,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        ModifyDate = this.ModifyDate,
                        ModifyID = this.ModifyID,
                    }, item => item.ID == this.ID);

                    reponsitory.Update<Layers.Data.Sqls.PvbErm.SchedulesPrivate>(new
                    {
                        ApplicationID = this.ApplicationID,
                        Type = (int)this.SchedulePrivateType,
                        AmOrPm = this.AmOrPm.ToString(),
                        StaffID = this.StaffID,
                        CreateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }
            }
        }

        #endregion
    }
}