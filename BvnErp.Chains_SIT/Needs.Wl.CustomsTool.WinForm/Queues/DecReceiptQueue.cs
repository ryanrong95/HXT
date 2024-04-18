using Needs.Ccs.Services.Models;
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

    /// <summary>
    /// 报关单队列
    /// </summary>
    public class DecReceiptQueue
    {
        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DecReceiptQueue()
        {
        }

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
                    var path = redis.ListLeftPop<string>("DecReceptQueue");
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        var fileName = Path.GetFileName(path);
                        if (fileName.ToLower().LastIndexOf("receipt") >= 0)
                        {
                            DecReceipt dec = new DecReceipt(path);
                            dec.HandleDecHeadResponse();
                        }
                        else
                        {
                            DecSuccess dec = new DecSuccess(path);
                            dec.HandleDecHeadResponse();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("操作报关单队列失败：" + ex.Message);
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
            redis.ListRightPush<string>("DecReceptQueue", path);
        }

    }
}