using System;
using System.ServiceProcess;
using Yahv.Erm.Services;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.AttendService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            #region Test
            //var task = Yahv.Erm.Services.AttendanceCalc.Current;
            //var config = ExecDtoHelper.GetConfig();
            //var date = Convert.ToDateTime(config.ExecDate);

            ////根据打卡时间计算昨天的考勤结果
            ////根据打卡时间计算昨天的考勤结果
            //LogHelper.WriteLogs(LogType.TRACE, $"根据打卡时间计算[{date.AddDays(-1)}]的考勤结果");
            //task.Calculate(date.AddDays(-1), AttendCalcStep.ModifyPastsStatus);
            ////根据员工 初始化当天的考勤结果
            //task.Calculate(date.Date, AttendCalcStep.InitPasts);

            ////根据个人日程安排 更新考勤结果
            //var config2 = ExecDtoHelper.GetConfig();
            //task.Calculate(DateTime.Parse(config2.ExecTime), AttendCalcStep.SyncLogs
            //       | AttendCalcStep.ModifyPastsTime
            //       | AttendCalcStep.ModifyPastsStatusBySched);
            #endregion

            //var task = new InitStaffVocationTask();
            //var dto = ExecDtoHelper.GetConfig();
            //if (dto.InitVocationYear != DateTime.Now.Year.ToString())
            //{
            //    task.Init();
            //    //更新配置文件
            //    dto.InitVocationYear = DateTime.Now.Year.ToString();
            //    ExecDtoHelper.SetConfig(dto);
            //}

            //try
            //{
            //    var task = new WarningTask();
            //    var dto = ExecDtoHelper.GetConfig();
            //    if (Convert.ToDateTime(dto.BackgroundNoticeDate).Date == DateTime.Now.Date)
            //    {
            //        if (DateTime.Now.Hour >= 8)
            //        {

            //            LogHelper.WriteLogs(LogType.TRACE, "开始背景调查邮件、短信提醒服务");
            //            task.Init();
            //            //task.PushWarning();
            //            //更新配置文件
            //            dto.BackgroundNoticeDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            //            ExecDtoHelper.SetConfig(dto);
            //            LogHelper.WriteLogs(LogType.TRACE, "提醒成功");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriteLogs(LogType.ERROR, "提醒失败：" + ex.Message);
            //}
        }
    }
}