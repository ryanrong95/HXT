using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services.Common;

namespace WinApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //捕获主流程异常（线程异常）
            Application.ThreadException += Application_ThreadException;

            ////指定如何响应未经处理的异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            //捕获主流程异常（应用程序域异常）
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Gecko.Xpcom.Initialize("Firefox");

            ////闪退测试(应用程序域抛异常)
            //new Thread(() =>
            //{
            //    throw new Exception("1");
            //}).Start();


            //var startor = new Main();

            ////一种
            //startor.WindowState = FormWindowState.Minimized;
            //startor.ShowInTaskbar = false;

            ////二种
            //startor.Visible = false;

            ////三种
            //startor.Hide(); 

            Application.Run(new HomePage());
            //if (ConfigurationManager.AppSettings["Startor"] == "chenhan1")
            //{

            //}
            //else
            //{
            //Application.Run(new TestForm());
            //}

        }

        /// <summary>
        /// 应用程序域抛异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (ex == null)
            {
                Application.Exit();
            }

            //记录错误日志
            Tools.Log(ex.InnerException ?? ex);

            if (e.IsTerminating)
            {
                Application.Exit();
            }

            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            //var fileInfo = new FileInfo(path);
            //if (!fileInfo.Directory.Exists)
            //{
            //    fileInfo.Directory.Create();
            //}

            //using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            //using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            //{
            //    ex = ex.InnerException ?? ex;
            //    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //    writer.Write('：');
            //    writer.Write(ex.Message);
            //    writer.WriteLine();
            //    writer.Write("StackTrace：");
            //    writer.WriteLine(ex.StackTrace);
            //    writer.WriteLine();
            //    writer.Close();
            //}


        }

        /// <summary>
        /// 捕获主流程异常事件（线程异常）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //记录错误日志
            Tools.Log(e.Exception.InnerException ?? e.Exception);

            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            //var fileInfo = new FileInfo(path);
            //if (!fileInfo.Directory.Exists)
            //{
            //    fileInfo.Directory.Create();
            //}

            //using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            //using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            //{
            //    Exception ex = e.Exception.InnerException ?? e.Exception;
            //    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //    writer.Write('：');
            //    writer.Write(ex.Message);
            //    writer.WriteLine();
            //    writer.Write("StackTrace：");
            //    writer.WriteLine(ex.StackTrace);
            //    writer.WriteLine();
            //    writer.Close();
            //}
        }
    }
}

/*
  //         string errorMsg = "An application error occurred. Please contact the adminstrator " +
                //"with the following information:\n\n";
                //         if (!EventLog.SourceExists("ThreadException"))
                //         {
                //             EventLog.CreateEventSource("ThreadException", "Application");
                //         }

                //         //记录到windows事件日志
                //         EventLog myLog = new EventLog();
                //         myLog.Source = "ThreadException";
                //         myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
     */
