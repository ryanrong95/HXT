using Needs.Ccs.Services.Models.BalanceQueueRedis;
using Needs.Utils.Redis;
using Needs.Utils.Serializers;
using Needs.Wl.CustomsTool.WinForm.Models;
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
    public class ManifestMessageWatcher : FileSystemWatcher
    {
        public ManifestMessageWatcher()
        {
            this.BeginInit();
            //设置监听文件类型
            this.Filter = "*.xml";
            //设置是否启用监听?
            this.EnableRaisingEvents = true;
            //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
            this.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            //设置监听的路径
            this.Path = System.IO.Path.Combine(Tool.Current.Folder.RmftMainFolder, ConstConfig.InBox);
            //注册创建文件或目录时的监听事件
            this.Created += new FileSystemEventHandler(ManifestMessageWatcher_Created);
            //注册重命名文件或目录的监听事件
            this.Renamed += new RenamedEventHandler(ManifestMessageWatcher_Created);
            //结束初始化
            this.EndInit();
        }

        private void ManifestMessageWatcher_Created(object sender, FileSystemEventArgs e)
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
            if (e.Name.ToLower().LastIndexOf("failed") >= 0)
            {
                string newFailXmlFullPath = System.IO.Path.Combine(Tool.Current.Folder.RmftMainFolder, ConstConfig.WaitFail + @"\" + e.Name);
                File.Move(e.FullPath, newFailXmlFullPath);
            }
            else
            {
                var path = Tool.Current.Folder.RmftMainFolder + @"\" + ConstConfig.WaitReceipt + @"\" + e.Name;
                File.Move(e.FullPath, path);
                ManiReceiptQueue queue = new ManiReceiptQueue();
                queue.EnterQueue(path);
            }
        }

    }
}
