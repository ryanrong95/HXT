using Needs.Utils.Flow.Event;
using Needs.Utils.Flow.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep
{
    /// <summary>
    /// Step 5
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class CirclePush<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : ContextParam
        where TNode : NodeParam, new()
    {
        public CirclePush()
        {
            base.EventBuilder.Append(CirclePushFromQueuePop);
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.RedisKey = context.Param.RedisKey;
            param.Redis = context.Param.Redis;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.MaxCountInCircle = context.Param.MaxCountInCircle;

            base.EventBuilder.Execute(param);

            //匹配后或不需要匹配处理方式
            context.Param.OnPairedOrNotNeedPair();

            return null;
        }

        private object CirclePushFromQueuePop(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;

            int currentCountInCircle = (int)entity.Redis.ListLength(entity.RedisKey.CircleList);

            if (currentCountInCircle < entity.MaxCountInCircle)
            {
                for (int i = 0; i < entity.MaxCountInCircle - currentCountInCircle; i++)
                {
                    BalanceQueueInfo minProcessIDModelInQueue = entity.Redis.ListLeftPop<BalanceQueueInfo>(entity.RedisKey.QueueList);

                    if (minProcessIDModelInQueue == null)
                    {
                        break;
                    }

                    entity.Redis.ListRightPush<BalanceQueueRecord>(entity.RedisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
                    {
                        RecordID = Guid.NewGuid().ToString("N"),
                        InfoID = minProcessIDModelInQueue.InfoID,
                        OldProcessName = minProcessIDModelInQueue.ProcessName,
                        NewProcessName = minProcessIDModelInQueue.ProcessName,
                        OldProcessStatus = minProcessIDModelInQueue.ProcessStatus,
                        NewProcessStatus = Enums.BalanceQueueProcessStatus.InCircle,
                        OldProcessID = 0,
                        NewProcessID = 0,
                        Status = Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });

                    minProcessIDModelInQueue.ProcessStatus = Enums.BalanceQueueProcessStatus.InCircle;
                    minProcessIDModelInQueue.ProcessID = 0;
                    minProcessIDModelInQueue.UpdateDate = DateTime.Now;
                    entity.Redis.ListRightPush<BalanceQueueInfo>(entity.RedisKey.CircleList, minProcessIDModelInQueue);
                }
            }

            return null;
        }
    }
}
