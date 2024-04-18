using Needs.Ccs.Services.Models.BalanceQueueRedis;
using Needs.Utils.Redis;
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
    public class DecFailBoxWatcher : FileSystemWatcher
    {
        private Logger _logger = LogManager.GetLogger("Dec_FailBox_Logger");

        public DecFailBoxWatcher(string path)
        {
            this.BeginInit();
            //设置监听文件类型
            this.Filter = "*.zip";
            //设置是否启用监听?
            this.EnableRaisingEvents = true;
            //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
            this.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            //设置监听的路径
            //this.Path = System.IO.Path.Combine(Tool.Current.Folder.DecMainFolder, ConstConfig.FailBox + @"\2019-08-01");
            this.Path = path;
            //注册创建文件或目录时的监听事件
            this.Created += new FileSystemEventHandler(DecFailBoxWatcher_Created);
            //注册重命名文件或目录的监听事件
            //decWatcher.Renamed += new RenamedEventHandler(decWatcher_renamed);
            //结束初始化
            this.EndInit();
        }

        private void DecFailBoxWatcher_Created(object sender, FileSystemEventArgs e)
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

            string newFilePath = System.IO.Path.Combine(Tool.Current.Folder.DecMainFolder, ConstConfig.FailBox_BK + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + e.Name);

            string dirPath = System.IO.Path.GetDirectoryName(newFilePath);
            if (Directory.Exists(dirPath))
            {
                //File.Delete(newFilePath);
            }
            else
            {
                Directory.CreateDirectory(dirPath);
            }

            File.Move(e.FullPath, newFilePath);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            try
            {
                

                FileBoxFilePreHandle(newFilePath);
            }
            catch (Exception ex)
            {
                this._logger.Error("DecFailBoxWatcher -> MoveFile 中异常：" + ex.Message
                    + "调用堆栈上的桢的字符串表现形式:" + ex.StackTrace
                    + "\r\n引发当前异常的函数为:" + ex.TargetSite.Name
                    + "\r\n导致错误的应用程序或对象的名称为:" + ex.Source);
            }
        }

        /// <summary>
        /// FileBox File 预处理
        /// </summary>
        /// <param name="filePath"></param>
        private void FileBoxFilePreHandle(string filePath)
        {
            //获取 BusinessID
            string businessID = string.Empty;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            businessID = GetBusinessID(fileName);

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
                    ProcessName = ConstConfig.FailBox,
                    BusinessType = Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
                    BusinessID = businessID,
                    FilePath = filePath,
                    Brief = string.Empty,
                },
            };

            RedisHelper redis = new RedisHelper();
            RedisKey redisKey = new RedisKey(balanceQueue.Info.BusinessType.ToString());

            balanceQueue.EnterQueue(redis, redisKey);
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
    }
}
