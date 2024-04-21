using System;
using System.IO;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.AttendService
{
    /// <summary>
    /// 执行时间
    /// </summary>
    public class ExecDto
    {
        /// <summary>
        /// 执行日期（每天执行一次）
        /// </summary>
        /// <remarks>用于更新前一天考勤结果、初始化当天考勤记录</remarks>
        public string ExecDate { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        /// <remarks>同步大赢家数据、更新考勤打卡时间、根据个人日程更新考勤结果</remarks>
        public string ExecTime { get; set; }

        /// <summary>
        /// 服务1 执行间隔时间(毫秒)
        /// </summary>
        public double ServiceInterval1 { get; set; }

        /// <summary>
        /// 服务2 执行间隔时间(毫秒)
        /// </summary>
        public double ServiceInterval2 { get; set; }

        /// <summary>
        /// 服务3 执行间隔时间(毫秒)
        /// </summary>
        public double ServiceInterval3 { get; set; }

        /// <summary>
        /// 服务4 执行间隔时间(毫秒)
        /// </summary>
        public double ServiceInterval4 { get; set; }

        /// <summary>
        /// 服务5执行间隔时间(毫秒)
        /// </summary>
        public double ServiceInterval5 { get; set; }

        /// <summary>
        /// 背景调查提醒日期
        /// </summary>
        public string BackgroundNoticeDate { get; set; }

        /// <summary>
        /// 倒班日期
        /// </summary>
        public string ShiftDate { get; set; }

        public string InitVocationYear { get; set; }
    }

    public static class ExecDtoHelper
    {
        /// <summary>
        /// 获取配置文件时间
        /// </summary>
        /// <returns></returns>
        public static ExecDto GetConfig()
        {
            using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json")))
            {
                string json = reader.ReadToEnd();
                return Utils.Serializers.JsonSerializerExtend.JsonTo<ExecDto>(json);
            }
        }

        /// <summary>
        /// 修改配置文件执行时间
        /// </summary>
        public static void SetConfig(ExecDto dto)
        {
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json"), dto.Json());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public static class LogHelper
    {
        public static void WriteLogs(LogType type, string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + "Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " >>> " + type.ToString() + " >>> " + content);
                    sw.Close();
                }
            }
        }
    }

    public enum LogType
    {
        TRACE = 1,
        DEBUG = 2,
        INFO = 3,
        WARN = 4,
        ERROR = 5,
        FATAL = 6,
    }
}