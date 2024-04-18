using Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep;
using Needs.Utils.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis
{
    public class BalanceQueue
    {
        /// <summary>
        /// Circle 中的最大个数
        /// </summary>
        private int MaxCountInCircle;

        /// <summary>
        /// 最大等待匹配时间
        /// </summary>
        private TimeSpan MaxWaitPairTimeSpan;

        private event PairedOrNotNeedPairHandler PairedOrNotNeedPair;
        private event RemindNotifyHandler RemindNotify;
        private event CheckIsUnNeedPairHandler CheckIsUnNeedPairHandler;

        public BalanceQueue()
        {

        }

        public BalanceQueue(int maxCountInCircle, TimeSpan maxWaitPairTimeSpan,
            PairedOrNotNeedPairHandler pairedOrNotNeedPairHandler,
            RemindNotifyHandler remindNotify,
            CheckIsUnNeedPairHandler checkIsUnNeedPairHandler)
        {
            this.MaxCountInCircle = maxCountInCircle;
            this.MaxWaitPairTimeSpan = maxWaitPairTimeSpan;
            this.PairedOrNotNeedPair = pairedOrNotNeedPairHandler;
            this.RemindNotify = remindNotify;
            this.CheckIsUnNeedPairHandler = checkIsUnNeedPairHandler;
        }

        public BalanceQueueInfo Info { get; set; }

        /// <summary>
        /// 进入 Queue
        /// 外部需要填充的Info信息：
        /// MacAddr
        /// ProcessName
        /// BusinessType
        /// BusinessID
        /// FilePath
        /// Brief
        /// </summary>
        /// <param name="reponsitory"></param>
        public void EnterQueue(RedisHelper redis, RedisKey redisKey)
        {
            this.Info.InfoID = Guid.NewGuid().ToString("N");
            this.Info.ProcessStatus = Enums.BalanceQueueProcessStatus.InQueue;
            this.Info.ProcessID = 0;
            this.Info.Status = Enums.Status.Normal;
            this.Info.CreateDate = DateTime.Now;
            this.Info.UpdateDate = DateTime.Now;

            redis.ListRightPush<BalanceQueueInfo>(redisKey.QueueList, this.Info);
            if (!string.IsNullOrEmpty(this.Info.FilePath) && this.Info.FilePath.ToLower().EndsWith(".xml"))
            {
                redis.HashSet<BalanceQueueInfo>(redisKey.XmlSet, this.Info.BusinessID, this.Info);
            }
            else if (!string.IsNullOrEmpty(this.Info.FilePath) && this.Info.FilePath.ToLower().EndsWith(".zip"))
            {
                redis.HashSet<BalanceQueueInfo>(redisKey.FailBoxSet, this.Info.BusinessID, this.Info);
            }

            redis.ListRightPush<BalanceQueueRecord>(redisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = this.Info.InfoID,
                NewProcessName = this.Info.ProcessName,
                NewProcessStatus = this.Info.ProcessStatus,
                NewProcessID = this.Info.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });
        }

        public void CoreHandler(RedisHelper redis)
        {
            Needs.Utils.Flow.Process.Context<ContextParam, NodeParam> context = new Needs.Utils.Flow.Process.Context<ContextParam, NodeParam>(new ContextParam()
            {
                Redis = redis,
                MacAddr = this.Info.MacAddr,
                ProcessName = this.Info.ProcessName,
                BusinessType = this.Info.BusinessType,
                MaxCountInCircle = this.MaxCountInCircle,
                MaxWaitPairTimeSpan = this.MaxWaitPairTimeSpan,
            });

            context.Param.PairedOrNotNeedPair += this.PairedOrNotNeedPair;
            context.Param.RemindNotify += this.RemindNotify;
            context.Param.CheckIsUnNeedPair += this.CheckIsUnNeedPairHandler;

            context.Node = new CirclePop<ContextParam, NodeParam>();
            context.Execute(new NodeParam());

        }
    }
}
