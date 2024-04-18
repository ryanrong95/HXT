using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Needs.Utils.Redis;
using Needs.Wl.CustomsTool.WinForm.Business;
using Needs.Wl.CustomsTool.WinForm.Models;
using NLog;

namespace Needs.Wl.CustomsTool
{
    public class DecSubQueue
    {
        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 操作队列
        /// </summary>
        public void ReadQueue()
        {
            while (true)
            {
                try
                {
                    RedisHelper redis = new RedisHelper();
                    var path = redis.ListLeftPop<string>("DecSubQueue");
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        DecSub sub = new DecSub(path);
                        sub.HandleDecSub();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("读取报关单订阅报文队列失败：" + ex.Message);
                }

            }
        }

        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="dec"></param>
        public void EnterQueue(string path)
        {
            RedisHelper redis = new RedisHelper();
            redis.ListRightPush<string>("DecSubQueue", path);
        }
    }
}
