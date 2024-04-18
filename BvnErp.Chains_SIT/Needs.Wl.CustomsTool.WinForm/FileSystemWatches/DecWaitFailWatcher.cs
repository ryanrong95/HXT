using Needs.Ccs.Services;
using Needs.Ccs.Services.Models.BalanceQueueRedis;
using Needs.Utils.Redis;
using Needs.Utils.Serializers;
using Needs.Wl.CustomsTool.WinForm.Models;
using Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class DecWaitFailWatcher : FileSystemWatcher
    {
        private Logger failXmlLogger = LogManager.GetLogger("Dec_Msg_Fail_Logger");

        public DecWaitFailWatcher()
        {
            this.BeginInit();
            //设置监听文件类型
            this.Filter = "*.xml";
            //设置是否启用监听?
            this.EnableRaisingEvents = true;
            //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
            this.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            //设置监听的路径
            this.Path = System.IO.Path.Combine(Tool.Current.Folder.DecMainFolder, ConstConfig.WaitFail);
            //注册创建文件或目录时的监听事件
            this.Created += new FileSystemEventHandler(DecMessageWatcher_Created);
            //注册重命名文件或目录的监听事件
            this.Renamed += new RenamedEventHandler(DecMessageWatcher_Created);
            //结束初始化
            this.EndInit();
        }

        private void DecMessageWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Task task = new Task(() => MoveFile(sender, e));
            task.Start();
        }

        /// <summary>
        /// 移动回执文件到临时文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveFile(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));

            string newFailXmlFullPath = System.IO.Path.Combine(Tool.Current.Folder.DecMainFolder, ConstConfig.InBox_BK + @"\" + e.Name);

            //File.Delete(newFailXmlFullPath);
            File.Move(e.FullPath, newFailXmlFullPath);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            try
            {

                ExceptionXmlPreHandle(newFailXmlFullPath);
            }
            catch (Exception ex)
            {
                this.failXmlLogger.Error("DecWaitFailWatcher -> MoveFile (failed) 中异常：" + ex.Message
                    + "调用堆栈上的桢的字符串表现形式:" + ex.StackTrace
                    + "\r\n引发当前异常的函数为:" + ex.TargetSite.Name
                    + "\r\n导致错误的应用程序或对象的名称为:" + ex.Source);
            }
        }

        #region 异常处理相关

        /// <summary>
        /// 异常Xml预处理
        /// </summary>
        /// <param name="filePath"></param>
        private void ExceptionXmlPreHandle(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(fileContent))
            {
                return;
            }

            fileContent = fileContent.Trim();

            fileContent = System.Text.RegularExpressions.Regex.Replace(fileContent,
                @"(xmlns:?[^=]*=[""][^""]*[""])", "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                System.Text.RegularExpressions.RegexOptions.Multiline);

            //获取 Brief
            string brief = string.Empty;
            if (fileContent.ToLower().Contains("resultflag"))
            {
                ExceptionXml.Root root = fileContent.XmlTo<ExceptionXml.Root>();
                brief = root.failInfo;
            }
            else if (fileContent.ToLower().Contains("decimportresponse"))
            {
                ExceptionXml.DecImportResponse decImportResponse = fileContent.XmlTo<ExceptionXml.DecImportResponse>();
                brief = decImportResponse.ErrorMessage;
            }

            //获取 BusinessID
            string businessID = string.Empty;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            businessID = GetBusinessID(fileName);
            if (string.IsNullOrEmpty(businessID))
            {
                businessID = GetBusinessID(fileContent);
            }
            if (string.IsNullOrEmpty(businessID))
            {
                return;
            }

            //EnterQueue
            Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue()
            {
                Info = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo()
                {
                    MacAddr = MacService.GetMacAddress(),
                    ProcessName = ConstConfig.InBox,
                    BusinessType = Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
                    BusinessID = businessID,
                    FilePath = filePath,
                    Brief = brief,
                },
            };

            RedisHelper redis = new RedisHelper();
            RedisKey redisKey = new RedisKey(balanceQueue.Info.BusinessType.ToString());

            balanceQueue.EnterQueue(redis, redisKey);

            string responsTime = filePath.Substring(filePath.LastIndexOf("_") + 1, 14);
            DateTime noticeDate = DateTime.ParseExact(responsTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

            TraceFail(
                declarationID: businessID,
                message: brief,
                noticeDate: noticeDate,
                fileName: System.IO.Path.GetFileName(filePath),
                backupUrl: filePath);
        }

        /// <summary>
        /// 从字符串中获取 BusinessID
        /// </summary>
        /// <param name="originStr"></param>
        /// <returns></returns>
        private string GetBusinessID(string originStr)
        {
            string prefix = string.Empty;
            if (originStr.Contains("HYCDO"))
            {
                prefix = "HYCDO";
            }
            else if (originStr.Contains("XDTCDO"))
            {
                prefix = "XDTCDO";
            }

            if (string.IsNullOrEmpty(prefix))
            {
                return string.Empty;
            }

            return originStr.Substring(originStr.IndexOf(prefix), prefix.Length + 15);
        }

        private void TraceFail(
            string declarationID,
            string message,
            DateTime noticeDate,
            string fileName,
            string backupUrl)
        {
            try
            {
                ErrDecTrace trace = new ErrDecTrace()
                {
                    DeclarationID = declarationID,
                    Channel = MultiEnumUtils.ToCode<Ccs.Services.Enums.CusDecStatus>(Ccs.Services.Enums.CusDecStatus.F1),
                    Message = message,
                    NoticeDate = noticeDate,
                    FileName = fileName,
                    BackupUrl = backupUrl,
                };
                trace.Enter();

                ErrDecHead head = new ErrDecHead()
                {
                    ID = declarationID,
                    CusDecStatus = MultiEnumUtils.ToCode<Ccs.Services.Enums.CusDecStatus>(Ccs.Services.Enums.CusDecStatus.F1),
                };
                head.UpdateCusDecStatus();
            }
            catch (Exception ex)
            {
                this.failXmlLogger.Error("DecWaitFailWatcher -> TraceFail (failed) 中异常：" + ex.Message
                        + "调用堆栈上的桢的字符串表现形式:" + ex.StackTrace
                        + "\r\n引发当前异常的函数为:" + ex.TargetSite.Name
                        + "\r\n导致错误的应用程序或对象的名称为:" + ex.Source);
            }
        }

        #endregion

    }
}
