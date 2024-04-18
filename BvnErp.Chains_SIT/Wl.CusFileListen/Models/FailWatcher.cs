using Needs.Ccs.Services.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.CusFileListen
{
    /// <summary>
    /// 监听校验失败数据的文件夹-FailBox
    /// </summary>
    public static class FailWatcher
    {
        /// <summary>
        /// 监听文件夹
        /// </summary>
        public static string WatcherPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FailBox"];
            }
        }

        /// <summary>
        /// 备份文件夹
        /// </summary>
        public static string BackupPath
        {
            get
            {
                return ConfigurationManager.AppSettings["FailBox_BK"];
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
            try {
                var response = XmlHelper.GetFailBox(e.FullPath);
                Logger.Trace("-------------------");
                Logger.Trace("获取报关单导入fail回执:" + response.ClientSeqNo);
                //content
                Logger.Trace("报关单fail回执内容:" + response.ClientSeqNo + " Code: " + response.ResponseCode + " ErrorMessage: " + response.ErrorMessage);

                var fileName = e.Name.Substring(0, e.Name.LastIndexOf("."));
                var responsTime = fileName.Substring(fileName.LastIndexOf("_") + 1, 14);
                response.ResponseTime = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                response.FileName = e.Name;
                response.BackupUrl = BackupPath + fileName + ".xml";

                //更新DB
                var Declare = new Needs.Ccs.Services.Views.DecHeadsView()[response.ClientSeqNo];
                Declare.EnterError += Declare_EnterError;
                Declare.EnterSuccess += Declare_EnterSuccess;
                //Declare.DeclareRecepit(response.ResponseCode);

                //Declare.Trace(response.ErrorMessage, response.ResponseTime.Value, response.FileName, response.BackupUrl);

                //写入回执轨迹表
                var decTrace = new DecTrace
                {
                    DeclarationID = response.ClientSeqNo,
                    Channel = response.ResponseCode,
                    Message = response.ErrorMessage,
                    NoticeDate = response.ResponseTime.Value,
                    FileName = response.FileName,
                    FilePath = e.FullPath,
                    BackupUrl = response.BackupUrl
                };
                decTrace.EnterError += DecTrace_EnterError;
                decTrace.EnterSuccess += DecTrace_EnterSuccess;
                decTrace.Enter();
                
                Logger.Trace("-------------------");

            }
            catch (Exception ex) {
                Logger.Error("获取报关单导入fail回执失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建文件或者目录时的监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void watch_renamed(object sender, FileSystemEventArgs e)
        {
            //事件内容
            try
            {
                var response = XmlHelper.GetFailBox(e.FullPath);
                Logger.Trace("-------------------");
                Logger.Trace("获取报关单导入fail回执:" + response.ClientSeqNo);
                //content
                Logger.Trace("报关单fail回执内容:" + response.ClientSeqNo + " Code: " + response.ResponseCode + " ErrorMessage: " + response.ErrorMessage);

                var fileName = e.Name.Substring(0, e.Name.LastIndexOf("."));
                var responsTime = fileName.Substring(fileName.LastIndexOf("_") + 1, 14);
                response.ResponseTime = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                response.FileName = e.Name;
                response.BackupUrl = BackupPath + fileName + ".xml";

                //更新DB
                var Declare = new Needs.Ccs.Services.Views.DecHeadsView()[response.ClientSeqNo];
                Declare.EnterError += Declare_EnterError;
                Declare.EnterSuccess += Declare_EnterSuccess;
                //Declare.DeclareRecepit(response.ResponseCode);

                //Declare.Trace(response.ErrorMessage, response.ResponseTime.Value, response.FileName, response.BackupUrl);

                //写入回执轨迹表
                var decTrace = new DecTrace
                {
                    DeclarationID = response.ClientSeqNo,
                    Channel = response.ResponseCode,
                    Message = response.ErrorMessage,
                    NoticeDate = response.ResponseTime.Value,
                    FileName = response.FileName,
                    FilePath = e.FullPath,
                    BackupUrl = response.BackupUrl
                };
                decTrace.EnterError += DecTrace_EnterError;
                decTrace.EnterSuccess += DecTrace_EnterSuccess;
                decTrace.Enter();

                Logger.Trace("-------------------");

            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单导入fail回执失败：" + ex.Message);
            }
        }


        private static void Declare_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("报关单fail回执更新报关单失败");
        }

        private static void Declare_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("报关单fail回执更新报关单成功");
        }

        private static void DecTrace_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("报关单fail回执新增轨迹失败");
        }

        private static void DecTrace_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("报关单fail回执新增轨迹成功");
            var decTrace = sender as DecTrace;
            File.Move(decTrace.FilePath, decTrace.BackupUrl);
        }
    }
}
