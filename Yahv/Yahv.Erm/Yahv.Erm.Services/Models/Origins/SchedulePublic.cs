using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 公共日程安排
    /// </summary>
    public class SchedulePublic : Schedule
    {
        #region 属性

        /// <summary>
        /// 安排方式
        /// </summary>
        public ScheduleMethod Method { get; set; }

        /// <summary>
        /// 0 工作日、1 公休日、2 元旦、3 春节、4 清明、5 劳动节、6 端午、7 国庆节、8 中秋
        /// CalendarType枚举填充
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 日程安排来源
        /// </summary>
        public ScheduleFrom From { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public string RegionID { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 薪酬倍数
        /// </summary>
        public decimal SalaryMultiple { get; set; }

        /// <summary>
        /// 所属班别
        /// </summary>
        public string ShiftID { get; set; }

        /// <summary>
        /// 实际班别
        /// </summary>
        public string SchedulingID { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 所属班别 名称
        /// </summary>
        public string ShiftName { get; set; }

        /// <summary>
        /// 实际班别 名称
        /// </summary>
        public string SchedulingName { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string RegionName { get; set; }

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

                    reponsitory.Insert(new Layers.Data.Sqls.PvbErm.SchedulesPublic()
                    {
                        ID = this.ID,
                        From = (int)this.From,
                        Name = this.Name,
                        PostionID = this.PostionID,
                        RegionID = this.RegionID,
                        SalaryMultiple = this.SalaryMultiple,
                        SchedulingID = this.SchedulingID,
                        ShiftID = this.ShiftID,
                        Method = (int)this.Method
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

                    reponsitory.Update<Layers.Data.Sqls.PvbErm.SchedulesPublic>(new
                    {
                        From = (int)this.From,
                        Name = this.Name,
                        PostionID = this.PostionID,
                        RegionID = this.RegionID,
                        SalaryMultiple = this.SalaryMultiple,
                        SchedulingID = this.SchedulingID,
                        ShiftID = this.ShiftID,
                        Method = (int)this.Method,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}