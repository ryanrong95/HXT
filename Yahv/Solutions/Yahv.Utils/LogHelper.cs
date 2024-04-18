using System;
using System.IO;
using System.Text;

namespace Yahv.Utils
{
    /// <summary>
    /// 日志文件写入
    /// </summary>
    public class LogHelper
    {
        //记录错误日志
        static object lockHelperLog = new object();

        public static void Log(string msg, string logname = "log")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            //lock (lockHelperLog)
            //{
            //    string txt = string.Format("{0}\r\n{1}\r\n\r\n", DateTime.Now, msg);
            //    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + logname + ".txt ", txt, Encoding.UTF8);
            //}



            //张晓辉2020年5月12日修改，解决文件被另一进程占用的问题
            var filepath = AppDomain.CurrentDomain.BaseDirectory + logname + ".txt ";
            string txt = string.Format("{0}\r\n{1}\r\n\r\n", DateTime.Now, msg);

            using (var appender = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(txt);
                appender.Write(bytes, 0, bytes.Length);
                appender.Flush();
            }
        }

        public static void Log(Exception ex, string logname = "log")
        {
            if (ex == null)
            {
                return;
            }

            //lock (lockHelperLog)
            //{
            //    string txt = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n", ex.Message, DateTime.Now, ex.StackTrace);
            //    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + logname + ".txt ", txt, Encoding.UTF8);
            //}


            //张晓辉2020年5月12日修改，解决文件被另一进程占用的问题
            var filepath = AppDomain.CurrentDomain.BaseDirectory + logname + ".txt ";
            string txt = string.Format("{0}\r\n{1}\r\n{2}\r\n\r\n", ex.Message, DateTime.Now, ex.StackTrace);

            using (var appender = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(txt);
                appender.Write(bytes, 0, bytes.Length);
                appender.Flush();
            }
        }

       
    }
}
