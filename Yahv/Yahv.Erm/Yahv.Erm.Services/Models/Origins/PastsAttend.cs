using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 考勤历史（统计）
    /// </summary>
    public class PastsAttend : IUnique
    {
        #region 属性

        private string id;
        /// <summary>
        /// ID(20190701AmStaff01105)
        /// </summary>
        public string ID
        {
            get
            {
                if (id == null)
                {
                    this.id = string.Concat(this.Date.ToString("yyyyMMdd") + this.AmOrPm + this.StaffID);
                }
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 上下午枚举：Am Or  Pm
        /// </summary>
        public string AmOrPm { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 实际班别ID
        /// </summary>
        public string SchedulingID { get; set; }

        /// <summary>
        /// 开始工作时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束下班时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 实际情况  枚举化： 正常、旷工、事假、病假、带薪假、加班、公休日、法定节假日
        /// </summary>
        public AttendInFactType InFact { get; set; }

        /// <summary>
        /// 是否迟到
        /// </summary>
        public bool? IsLater { get; set; }

        /// <summary>
        /// 是否早退
        /// </summary>
        public bool? IsEarly { get; set; }

        /// <summary>
        /// 上班补签
        /// </summary>
        public bool? OnWorkRemedy { get; set; }

        /// <summary>
        /// 下班补签
        /// </summary>
        public bool? OffWorkRemedy { get; set; }
        #endregion

        public PastsAttend()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.InFact = AttendInFactType.Normal;
            this.IsEarly = this.IsLater = false;
            this.OnWorkRemedy = this.OffWorkRemedy = false;
        }

        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (!repository.ReadTable<Pasts_Attend>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbErm.Pasts_Attend()
                    {
                        ID = this.ID,
                        Date = this.Date,
                        AmOrPm = this.AmOrPm,
                        StaffID = this.StaffID,
                        SchedulingID = this.SchedulingID,
                        StartTime = this.StartTime,
                        EndTime = this.EndTime,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        InFact = (int)this.InFact,
                        IsEarly = this.IsEarly,
                        IsLater = this.IsLater,
                        OnWorkRemedy = this.OnWorkRemedy,
                        OffWorkRemedy = this.OffWorkRemedy,
                    });
                }
                else
                {
                    repository.Update<Pasts_Attend>(new
                    {
                        Date = this.Date,
                        AmOrPm = this.AmOrPm,
                        StaffID = this.StaffID,
                        SchedulingID = this.SchedulingID,
                        StartTime = this.StartTime,
                        EndTime = this.EndTime,
                        ModifyDate = this.ModifyDate,
                        InFact = (int)this.InFact,
                        IsEarly = this.IsEarly,
                        IsLater = this.IsLater,
                        OnWorkRemedy = this.OnWorkRemedy,
                        OffWorkRemedy = this.OffWorkRemedy,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }

    /// <summary>
    /// 考勤最终结果
    /// </summary>
    public class PastsAttendFinalResult : IUnique
    {
        public string ID { get; set; }

        #region 属性

        public DateTime Date { get; set; }

        public string StaffID { get; set; }

        public string SchedulingID { get; set; }

        /// <summary>
        /// 考勤结果
        /// </summary>
        public IEnumerable<PastsAttend> PastsAttends { get; set; }

        /// <summary>
        /// 打卡详情
        /// </summary>
        public IEnumerable<Logs_Attend> Logs_Attends { get; set; }

        #endregion

        #region 扩展属性

        public Scheduling Scheduling { get; set; }

        public Staff Staff { get; set; }

        /// <summary>
        /// 打卡记录
        /// </summary>
        public string AttendTime
        {
            get
            {
                var Logs_Attend = this.Logs_Attends.Select(item => item.CreateDate.TimeOfDay);
                if (Logs_Attend.Count() == 0)
                {
                    return "--";
                }
                var min = Logs_Attend.Min();
                var max = Logs_Attend.Max();
                return min.ToString(@"hh\:mm\:ss") + " - " + max.ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// 工作时长
        /// </summary>
        public string WordHours
        {
            get
            {
                var Logs_Attend = this.Logs_Attends.Select(item => item.CreateDate.TimeOfDay);
                if (Logs_Attend.Count() == 0)
                {
                    return "--";
                }
                var diff = Logs_Attend.Max() - Logs_Attend.Min();
                return diff.ToString("hh") + "时 " + diff.ToString("mm") + "分";
            }
        }

        public string AttendResult
        {
            get
            {
                var Am = this.PastsAttends.SingleOrDefault(item => item.AmOrPm == AmOrPm.Am.ToString());
                var Pm = this.PastsAttends.SingleOrDefault(item => item.AmOrPm == AmOrPm.Pm.ToString());
                if (Am == null && Pm == null)
                {
                    return "--";
                }
                else
                {
                    List<string> result = new List<string>();
                    if (Am != null)
                    {
                        if (Am.InFact != AttendInFactType.Normal)
                        {
                            result.Add(Am.InFact.GetDescription() + "Am");
                        }
                        else
                        {
                            if (Am.IsEarly == true)
                            {
                                result.Add("早退Am");
                            }
                            else if (Am.IsLater == true)
                            {
                                result.Add("迟到Am");
                            }
                            else if (Am.OnWorkRemedy == true)
                            {
                                result.Add("上班补签Am");
                            }
                            else if (Am.OffWorkRemedy == true)
                            {
                                result.Add("下班补签Am");
                            }
                            else
                            {
                                result.Add("正常Am");
                            }
                        }
                    }
                    if (Pm != null)
                    {
                        if (Pm.InFact != AttendInFactType.Normal)
                        {
                            result.Add(Pm.InFact.GetDescription() + "Pm");
                        }
                        else
                        {
                            if (Pm.IsEarly == true)
                            {
                                result.Add("早退Pm");
                            }
                            else if (Pm.IsLater == true)
                            {
                                result.Add("迟到Pm");
                            }
                            else if (Pm.OnWorkRemedy == true)
                            {
                                result.Add("上班补签Pm");
                            }
                            else if (Pm.OffWorkRemedy == true)
                            {
                                result.Add("下班补签Pm");
                            }
                            else
                            {
                                result.Add("正常Pm");
                            }
                        }
                    }
                    return convertArrayToString(result.ToArray());
                }
            }
        }

        #endregion

        private string convertArrayToString(string[] strArr)
        {
            if (strArr == null || strArr.Count() == 0)
            {
                return "";
            }
            string res = "";
            for (int i = 0, len = strArr.Count(); i < len; i++)
            {
                res += strArr[i];
                if (i < len - 1)
                {
                    res += ",";
                }
            }
            return res;
        }

    }
}