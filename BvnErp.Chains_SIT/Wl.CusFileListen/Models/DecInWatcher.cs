using Needs.Ccs.Services.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wl.CusFileListen
{
    /// <summary>
    /// 监听导入回执文件夹-InBox
    /// </summary>
    public static class DecInWatcher
    {
        /// <summary>
        /// 监听文件夹
        /// </summary>
        public static string WatcherPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DecInBox"];
            }
        }

        /// <summary>
        /// 备份文件夹
        /// </summary>
        public static string BackupPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DecInBox_BK"];
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public static string FilterType
        {
            get
            {
                return "*.xml";
            }
        }

        /// <summary>
        /// 定义监听器
        /// </summary>
        private static FileSystemWatcher watcher;
        public static FileSystemWatcher Watcher
        {
            get
            {
                if (watcher == null)
                {
                    watcher = new FileSystemWatcher();
                }
                return watcher;
            }
            set
            {
                if (value != null)
                {
                    watcher = value;
                }
            }
        }

        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 初始化监听
        /// </summary>
        /// <param name="StrWarcherPath">需要监听的目录</param>
        /// <param name="FilterType">需要监听的文件类型(筛选器字符串)</param>
        /// <param name="IsEnableRaising">是否启用监听</param>
        /// <param name="IsInclude">是否监听子目录</param>
        public static void WatcherStrat()
        {
            //初始化监听
            Watcher.BeginInit();
            //设置监听文件类型
            Watcher.Filter = FilterType;
            //设置是否监听子目录
            Watcher.IncludeSubdirectories = false;
            //设置是否启用监听?
            Watcher.EnableRaisingEvents = false;
            //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
            Watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            //设置监听的路径
            Watcher.Path = WatcherPath;
            //注册创建文件或目录时的监听事件
            Watcher.Created += new FileSystemEventHandler(watch_created);
            //注册重命名文件或目录的监听事件
            Watcher.Renamed += new RenamedEventHandler(watch_renamed);
            //结束初始化
            Watcher.EndInit();
        }

        /// <summary>
        /// 创建文件或者目录时的监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void watch_created(object sender, FileSystemEventArgs e)
        {
            //事件内容
            //线程中处理
            Thread th = new Thread(() => HandleDecHeadResponse(e.Name, e.FullPath));
            th.Start();
        }

        /// <summary>
        /// 重命名文件的监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void watch_renamed(object sender, RenamedEventArgs e)
        {
            //事件内容
            //线程中处理
            Thread th = new Thread(() => HandleDecHeadResponse(e.Name, e.FullPath));
            th.Start();
        }

        /// <summary>
        /// 处理回执
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FullPath"></param>
        private static void HandleDecHeadResponse(string FileName, string FullPath)
        {
            try
            {
                //业务回执
                if (FileName.ToLower().LastIndexOf("receipt") >= 0)
                {
                    var receipt = new DecData();
                    receipt.DecResult = XmlHelper.GetDecReceipt(FullPath);
                    Logger.Trace("-------------------");
                    Logger.Trace("获取报关单导入业务回执:" + receipt.DecResult.SEQ_NO);
                    //content
                    Logger.Trace("报关单业务回执内容:" + receipt.DecResult.SEQ_NO + " CHANNEL: " + receipt.DecResult.CHANNEL + " NOTE: " + receipt.DecResult.NOTE);

                    receipt.FileName = FileName;
                    receipt.FilePath = FullPath;
                    receipt.BackupUrl = BackupPath + FileName;
                    receipt.SetHead();
                    receipt.DecHead.EnterError += Declare_EnterError;
                    receipt.DecHead.EnterSuccess += Declare_EnterSuccess;
                    receipt.SaveAs();

                }
                else
                {

                    var fileName = FileName.Substring(0, FileName.LastIndexOf("."));
                    var responsTime = fileName.Substring(fileName.LastIndexOf("_") + 1, 14);
                    var ResponseTime = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    var BackupUrl = BackupPath + fileName + ".xml";

                    if (FileName.Length == 51 && FileName.ToLower().LastIndexOf("failed") >= 0)
                    {
                        //客户端配置异常
                        var root = XmlHelper.GetDecSysResponse(FullPath);
                        Logger.Trace("-------------------");
                        root.ClientSeqNo = FileName.Split('_')[1];
                        Logger.Trace("客户端异常: " + root.ClientSeqNo + " Flag：" + root.resultFlag + " Code：" + root.failCode + " Info：" + root.failInfo + " Data：" + root.retData);

                        root.ResponseTime = ResponseTime;
                        root.FileName = FileName;
                        root.FilePath = FullPath;
                        root.BackupUrl = BackupUrl;
                        root.SetHead();
                        root.DecHead.EnterError += Declare_EnterError;
                        root.DecHead.EnterSuccess += Declare_EnterSuccess;
                        root.SaveAs();
                    }
                    else
                    {
                        //暂存回执
                        var response = XmlHelper.GetDecResponse(FullPath);
                        Logger.Trace("-------------------");
                        Logger.Trace("获取报关单导入响应回执:" + response.ClientSeqNo);
                        //content
                        Logger.Trace("报关单响应回执内容:" + response.ClientSeqNo + " Code: " + response.ResponseCode + " Message: " + response.ErrorMessage);

                        response.ResponseTime = ResponseTime;
                        response.FileName = FileName;
                        response.FilePath = FullPath;
                        response.BackupUrl = BackupUrl;
                        response.SetHead();
                        response.DecHead.EnterError += Declare_EnterError;
                        response.DecHead.EnterSuccess += Declare_EnterSuccess;
                        response.SaveAs();
                    }

                }

                Logger.Trace("-------------------");

            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单导入回执失败：" + ex.Message);
            }
        }

        private static void Declare_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("报关单回执更新报关单失败");
        }

        private static void Declare_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("报关单回执更新报关单成功");
        }

        //private static void DecTrace_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        //{
        //    Logger.Trace("报关单回执新增轨迹失败");
        //}

        //private static void DecTrace_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        //{
        //    Logger.Trace("报关单回执新增轨迹成功");
        //    var decTrace = sender as DecTrace;
        //    File.Move(decTrace.FilePath, decTrace.BackupUrl);
        //}
    }
}

