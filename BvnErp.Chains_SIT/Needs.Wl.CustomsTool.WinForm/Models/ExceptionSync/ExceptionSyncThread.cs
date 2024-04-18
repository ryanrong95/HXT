using Needs.Utils.Redis;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Needs.Wl.CustomsTool.WinForm.Models.ExceptionSync
{
    public class ExceptionSyncThread
    {
        System.Timers.Timer syncTimer = new System.Timers.Timer();
        private bool IsSyncTimerRun = false;

        private bool IsSyncTimerBusy = false;

        //初始化 ExceptionSyncThread Begin
        private RedisHelper _redis;
        private Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey _redisKey;
        private string _macAddr = string.Empty;
        private string _processName = string.Empty;
        private Needs.Ccs.Services.Enums.BalanceQueueBusinessType _businessType;
        private Logger _logger;

        public ExceptionSyncThread(string macAddr, string processName, Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType, Logger logger)
        {
            this._redis = new RedisHelper();
            this._redisKey = new Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey(businessType.ToString());
            this._macAddr = macAddr;
            this._processName = processName;
            this._businessType = businessType;
            this._logger = logger;
        }
        //初始化 ExceptionSyncThread End

        /// <summary>
        /// 启动 SyncThread
        /// </summary>
        public void StartSyncThread()
        {
            this.IsSyncTimerRun = true;

            var handlerThread = new Thread(() =>
            {
                syncTimer.Enabled = true;
                syncTimer.Interval = 5000;
                syncTimer.Elapsed += new System.Timers.ElapsedEventHandler(FunInSyncTimer);
            });
            handlerThread.IsBackground = true;
            handlerThread.Start();
        }

        /// <summary>
        /// 启动 SyncThread
        /// </summary>
        public void StartSyncTimer()
        {
            this.IsSyncTimerRun = true;
            syncTimer.Start();
        }

        /// <summary>
        /// 停止 SyncTimer
        /// </summary>
        public void StopSyncTimer()
        {
            this.IsSyncTimerRun = false;
        }

        public bool ShowIsHandlerTimerBusy()
        {
            return this.IsSyncTimerBusy;
        }

        private void FunInSyncTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                if (!this.IsSyncTimerRun)
                {
                    //停止 HandlerThread
                    syncTimer.Enabled = false;
                    return;
                }

                if (this.IsSyncTimerBusy)
                {
                    return;
                }

                this.IsSyncTimerBusy = true;

                int doneCount = 0;

                for (int i = 0; i < 5; i++)
                {
                    var balanceQueueInfo = this._redis.ListLeftPop<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.SyncDBBalanceInfoList);
                    if (balanceQueueInfo == null)
                    {
                        break;
                    }

                    balanceQueueInfo.NoDuplicateOperation();
                    doneCount++;
                }

                for (int i = 0; i < 10 - doneCount; i++)
                {
                    var balanceQueueRecord = this._redis.ListLeftPop<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueRecord>(this._redisKey.SyncDBBalanceRecordList);
                    if (balanceQueueRecord == null)
                    {
                        break;
                    }

                    balanceQueueRecord.NoDuplicateOperation();
                }

                for (int i = 0; i < 5; i++)
                {
                    var remindBalanceQueueInfo = this._redis.ListLeftPop<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.RemindHimList);
                    if (remindBalanceQueueInfo == null)
                    {
                        break;
                    }

                    remindBalanceQueueInfo.NoDuplicateOperation();
                }

                this.IsSyncTimerBusy = false;
            }
            catch (Exception ex)
            {
                _logger.Error("ExceptionSyncThread -> FunInSyncTimer 中异常：" + ex.Message
                    + "调用堆栈上的桢的字符串表现形式:" + ex.StackTrace
                    + "\r\n引发当前异常的函数为:" + ex.TargetSite.Name
                    + "\r\n导致错误的应用程序或对象的名称为:" + ex.Source);
            }
        }
    }
}
