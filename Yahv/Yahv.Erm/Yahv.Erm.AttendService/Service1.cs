using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Erm.Services;

namespace Yahv.Erm.AttendService
{
    partial class Service1 : ServiceBase
    {
        static object locker = new object();
        static System.Timers.Timer timer1;
        static System.Timers.Timer timer2;
        static System.Timers.Timer timer3;
        static System.Timers.Timer timer4;
        static System.Timers.Timer timer5;

        public Service1()
        {
            InitializeComponent();

            var config = ExecDtoHelper.GetConfig();

            timer1 = new System.Timers.Timer();
            timer1.Interval = config.ServiceInterval1;
            timer1.Elapsed += Timer1_Elapsed;

            timer2 = new System.Timers.Timer();
            timer2.Interval = config.ServiceInterval2;
            timer2.Elapsed += Timer2_Elapsed;

            timer3 = new System.Timers.Timer();
            timer3.Interval = config.ServiceInterval3;
            timer3.Elapsed += Timer3_Elapsed;

            timer4 = new System.Timers.Timer();
            timer4.Interval = config.ServiceInterval4;
            timer4.Elapsed += Timer4_Elapsed;

            timer5 = new System.Timers.Timer();
            timer5.Interval = config.ServiceInterval5;
            timer5.Elapsed += Timer5_Elapsed;
        }

        /// <summary>
        /// 每天凌晨执行一次
        /// </summary>
        /// <remarks>更新前一天考勤结果、初始化当天考勤记录</remarks>
        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    var task = Yahv.Erm.Services.AttendanceCalc.Current;
                    var dto = ExecDtoHelper.GetConfig();
                    if (Convert.ToDateTime(dto.ExecDate).Date == DateTime.Now.Date)
                    {
                        //根据打卡时间计算昨天的考勤结果
                        LogHelper.WriteLogs(LogType.TRACE, $"根据打卡时间计算[{DateTime.Now.AddDays(-1)}]的考勤结果");
                        task.Calculate(DateTime.Now.AddDays(-1), AttendCalcStep.ModifyPastsStatus);

                        //根据员工 初始化当天的考勤结果
                        LogHelper.WriteLogs(LogType.TRACE, $"根据员工 初始化[{DateTime.Now}]的考勤结果");
                        task.Calculate(DateTime.Now, AttendCalcStep.InitPasts);

                        //更新配置文件
                        dto.ExecDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

                        ExecDtoHelper.SetConfig(dto);
                    }

                    //如果配置文件日期，小于当前时间，直接执行考勤
                    if (Convert.ToDateTime(dto.ExecDate).Date < DateTime.Now.Date)
                    {
                        DateTime date = Convert.ToDateTime(dto.ExecDate);

                        //根据打卡时间计算昨天的考勤结果
                        LogHelper.WriteLogs(LogType.TRACE, $"根据打卡时间计算[{date.AddDays(-1)}]的考勤结果");
                        task.Calculate(date.AddDays(-1), AttendCalcStep.ModifyPastsStatus);

                        //根据员工 初始化当天的考勤结果
                        LogHelper.WriteLogs(LogType.TRACE, $"根据员工 初始化[{date.Date}]的考勤结果");
                        task.Calculate(date.Date, AttendCalcStep.InitPasts);

                        //同步数据，更新Pasts数据
                        LogHelper.WriteLogs(LogType.TRACE, $"同步大赢家数据，更新考勤结果");
                        task.Calculate(date, AttendCalcStep.SyncLogs | AttendCalcStep.ModifyPastsTime);

                        //根据日程安排 更新Pasts状态
                        LogHelper.WriteLogs(LogType.TRACE, $"根据个人日程安排 更新考勤结果");
                        task.Calculate(DateTime.Parse(dto.ExecTime), AttendCalcStep.ModifyPastsStatusBySched);

                        //更新配置文件
                        dto.ExecDate = date.AddDays(1).ToString("yyyy-MM-dd");

                        ExecDtoHelper.SetConfig(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                    "空间名：" + ex.Source + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace + '\n' +
                    "错误提示:" + ex.ToString();
                LogHelper.WriteLogs(LogType.ERROR, $"更新前天考勤结果、初始化当天考勤记录异常：" + message);
            }
        }

        /// <summary>
        /// 隔一段时间执行一次
        /// </summary>
        /// <remarks>同步大赢家数据、更新考勤结果的打卡时间、根据个人日程安排更新考勤结果</remarks>
        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    var task = Yahv.Erm.Services.AttendanceCalc.Current;
                    var dto = ExecDtoHelper.GetConfig();
                    var date = DateTime.Parse(dto.ExecTime).Date;
                    if (date < DateTime.Now.Date)
                    {
                        //更新配置时间与当前时间之间所有日期的考勤结果
                        LogHelper.WriteLogs(LogType.TRACE, $"[{date.ToString("yyyy-MM-dd")}] 同步大赢家数据、更新考勤打卡时间、根据个人日程安排更新考勤结果");
                        task.Calculate(DateTime.Parse(dto.ExecTime), AttendCalcStep.SyncLogs
                        | AttendCalcStep.ModifyPastsTime
                        | AttendCalcStep.ModifyPastsStatusBySched);

                        //日期+1
                        dto.ExecTime = date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (date == DateTime.Now.Date)
                    {
                        //执行当日更新
                        LogHelper.WriteLogs(LogType.TRACE, $"[{date.ToString("yyyy-MM-dd")}] 同步大赢家数据、更新考勤打卡时间、根据个人日程安排更新考勤结果");
                        task.Calculate(DateTime.Parse(dto.ExecTime), AttendCalcStep.SyncLogs
                        | AttendCalcStep.ModifyPastsTime
                        | AttendCalcStep.ModifyPastsStatusBySched);
                    }
                    if (date > DateTime.Now.Date)
                    {
                        //更新配置时间为当天，理论不会出现
                        dto.ExecTime = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    ExecDtoHelper.SetConfig(dto);
                }
            }
            catch (Exception ex)
            {
                string message =
                    "空间名：" + ex.Source + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace + '\n' +
                    "错误提示:" + ex.ToString();
                LogHelper.WriteLogs(LogType.ERROR, "同步大赢家数据、更新考勤结果的打卡时间、根据个人日程安排更新考勤结果异常：" + message);
            }
        }

        /// <summary>
        /// 背景调查邮件、短信提醒功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer3_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    var task = new WarningTask();
                    var dto = ExecDtoHelper.GetConfig();
                    if (Convert.ToDateTime(dto.BackgroundNoticeDate).Date == DateTime.Now.Date)
                    {
                        if (DateTime.Now.Hour >= 8)
                        {
                            LogHelper.WriteLogs(LogType.TRACE, "开始背景调查邮件、短信提醒服务");
                            task.Init();
                            task.PushWarning();
                            //更新配置文件
                            dto.BackgroundNoticeDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                            ExecDtoHelper.SetConfig(dto);
                            LogHelper.WriteLogs(LogType.TRACE, "提醒成功");
                        }
                    }
                    else
                    {
                        var date = Convert.ToDateTime(dto.BackgroundNoticeDate).Date;
                        if (date < DateTime.Now.Date)
                        {
                            dto.BackgroundNoticeDate = date.AddDays(1).ToString("yyyy-MM-dd");
                            ExecDtoHelper.SetConfig(dto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogs(LogType.ERROR, "提醒失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 初始化员工假期任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer4_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    var task = new InitStaffVocationTask();
                    var dto = ExecDtoHelper.GetConfig();
                    if (dto.InitVocationYear != DateTime.Now.Year.ToString())
                    {
                        LogHelper.WriteLogs(LogType.TRACE, "开始初始化员工假期");
                        task.Init();
                        //更新配置文件
                        dto.InitVocationYear = DateTime.Now.Year.ToString();
                        ExecDtoHelper.SetConfig(dto);
                        LogHelper.WriteLogs(LogType.TRACE, "初始化员工假期成功");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogs(LogType.FATAL, "初始化员工假期失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 每周周日执行一次
        /// </summary>
        /// <remarks>员工自动倒班</remarks>
        private void Timer5_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lock (locker)
                {
                    var dto = ExecDtoHelper.GetConfig();
                    var shiftData = Convert.ToDateTime(dto.ShiftDate);
                    //所有倒班的员工设置为A班
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour >= 21)
                    {
                        ShiftTask.SetScheduleA();
                    }
                    //每周一，下午六点之后开始更新轮班员工的班别
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour >= 21)
                    {
                        if (shiftData.Date == DateTime.Now.Date)
                        {
                            LogHelper.WriteLogs(LogType.TRACE, "员工自动换班任务开始");
                            ShiftTask.Shift();
                            //更新下次倒班时间
                            dto.ShiftDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                            ExecDtoHelper.SetConfig(dto);
                            LogHelper.WriteLogs(LogType.TRACE, "员工自动换班任务结束");
                        }
                        else
                        {
                            if (shiftData.Date < DateTime.Now.Date)
                            {
                                dto.ShiftDate = DateTime.Now.ToString("yyyy-MM-dd");
                                ExecDtoHelper.SetConfig(dto);
                            }
                            if (shiftData.Date > DateTime.Now.Date && shiftData.Date.DayOfWeek != DayOfWeek.Monday)
                            {
                                dto.ShiftDate = DateTime.Now.ToString("yyyy-MM-dd");
                                ExecDtoHelper.SetConfig(dto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message =
                    "空间名：" + ex.Source + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace + '\n' +
                    "错误提示:" + ex.ToString();
                LogHelper.WriteLogs(LogType.ERROR, $"员工自动倒班：" + message);
            }
        }

        protected override void OnStart(string[] args)
        {
            timer1.Start();
            timer2.Start();
            timer3.Start();
            timer4.Start();
            timer5.Start();
        }

        protected override void OnStop()
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
            timer5.Stop();
        }
    }
}
