using Needs.Ccs.Services;
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
    public static class MftInWatcher
    {
        /// <summary>
        /// 监听文件夹
        /// </summary>
        public static string WatcherPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MftInBox"];
            }
        }

        /// <summary>
        /// 备份文件夹
        /// </summary>
        public static string BackupPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MftInBox_BK"];
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
            Thread th = new Thread(() => HandleManifestResponse(e.Name, e.FullPath));
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
            Thread th = new Thread(() => HandleManifestResponse(e.Name, e.FullPath));
            th.Start();
        }

        public static void HandleManifestResponse(string FileName, string FullPath)
        {
            try
            {

                var receipt = XmlHelper.GetMftResponse(FullPath);
                Logger.Trace("-------------------");

                //导入失败回执，只可通过文件名取MessageID
                if (string.IsNullOrEmpty(receipt.Head.MessageID))
                {
                    receipt.Head.MessageID = FileName.Split('_')[2];
                }

                //舱单删除报文ID调整，以便正确读取
                receipt.Head.MessageID = receipt.Head.MessageID.Replace("DEL",string.Empty);

                Logger.Trace("获取舱单回执:" + receipt.Head.MessageID);
                //content

                receipt.FileName = FileName;
                receipt.FilePath = FullPath;
                receipt.BackupUrl = BackupPath + FileName;
                receipt.SetManifestConsignment();
                receipt.ManifestConsignment.EnterError += ManifestBill_EnterError;
                receipt.ManifestConsignment.EnterSuccess += ManifestBill_EnterSuccess;

                //addition为空：传输回执
                if (receipt.Response.AdditionalInformation == null && receipt.Response.Consignment == null)
                {
                    //发往海关成功
                    Logger.Trace("舱单回执内容:" + receipt.Head.MessageID + " FunctionCode: " + receipt.Head.FunctionCode
                        + " ResponseCode: " + receipt.Response.ResponseType.Code
                        + " Description: " + receipt.Response.ResponseType.Text);

                    receipt.SaveTransAs();
                }
                else if (receipt.Response.Consignment != null)
                {
                    //发往海关成功
                    Logger.Trace("舱单回执内容:" + receipt.Head.MessageID + " FunctionCode: " + receipt.Head.FunctionCode
                        + " ResponseCode: " + receipt.Response.Consignment[0].AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum()
                        + " Description: " + receipt.Response.Consignment[0].AdditionalInformation.StatementDescription);

                    receipt.ResponseSuccess();
                }
                else
                {
                    //调用成功
                    Logger.Trace("舱单回执内容:" + receipt.Head.MessageID + " FunctionCode: " + receipt.Head.FunctionCode
                        + " StatementCode: " + receipt.Response.AdditionalInformation.StatementCode.Value.GetXmlEnumAttributeValueFromEnum()
                        + " Description: " + receipt.Response.AdditionalInformation.StatementDescription);

                    receipt.ResponseNornmal();
                }

                Logger.Trace("-------------------");

            }
            catch (Exception ex)
            {
                Logger.Error("获取舱单导入回执失败：" + ex.Message);
            }
        }

        private static void ManifestBill_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("舱单回执更新报关单成功");
        }

        private static void ManifestBill_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("舱单回执更新报关单失败");
        }
    }
}

