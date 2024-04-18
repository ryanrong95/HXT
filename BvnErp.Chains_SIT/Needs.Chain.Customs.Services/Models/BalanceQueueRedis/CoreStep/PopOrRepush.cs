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
    /// Step 4
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class PopOrRepush<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : ContextParam
        where TNode : NodeParam, new()
    {
        public PopOrRepush()
        {

        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.RedisKey = context.Param.RedisKey;
            param.Redis = context.Param.Redis;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.PopOrRepushParam.MinProcessIDModelInCircle = context.Param.MinProcessIDModelInCircle;

            var minProcessIDModelInCircle = (Models.BalanceQueueRedis.BalanceQueueInfo)context.Param.MinProcessIDModelInCircle;
            if (DateTime.Now - minProcessIDModelInCircle.CreateDate > context.Param.MaxWaitPairTimeSpan)
            {
                base.EventBuilder.Append(PopMinProcessIDModelInCircle);
            }
            else
            {
                base.EventBuilder.Append(RepushMinProcessIDModelInCircle);
            }

            base.EventBuilder.Execute(param);

            context.Node = new CirclePush<TParam, TNode>();
            context.Execute(new TNode());

            return null;
        }

        private object PopMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PopOrRepushParam.MinProcessIDModelInCircle;

            entity.Redis.ListRightPush<BalanceQueueRecord>(entity.RedisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = minProcessIDModelInCircle.InfoID,
                OldProcessName = minProcessIDModelInCircle.ProcessName,
                NewProcessName = minProcessIDModelInCircle.ProcessName,
                OldProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                NewProcessStatus = Enums.BalanceQueueProcessStatus.ExceptionOut,
                OldProcessID = minProcessIDModelInCircle.ProcessID,
                NewProcessID = minProcessIDModelInCircle.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });

            minProcessIDModelInCircle.ProcessStatus = Enums.BalanceQueueProcessStatus.ExceptionOut;
            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            entity.Redis.ListRightPush<BalanceQueueInfo>(entity.RedisKey.SyncDBBalanceInfoList, minProcessIDModelInCircle);

            if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".xml"))
            {
                entity.Redis.HashDelete(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID);
            }
            else if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".zip"))
            {
                entity.Redis.HashDelete(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID);
            }

            return null;
        }

        private object RepushMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PopOrRepushParam.MinProcessIDModelInCircle;

            entity.Redis.ListRightPush<BalanceQueueRecord>(entity.RedisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = minProcessIDModelInCircle.InfoID,
                OldProcessName = minProcessIDModelInCircle.ProcessName,
                NewProcessName = minProcessIDModelInCircle.ProcessName,
                OldProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                NewProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                OldProcessID = 0,
                NewProcessID = 0,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });

            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            entity.Redis.ListRightPush<BalanceQueueInfo>(entity.RedisKey.CircleList, minProcessIDModelInCircle);

            return null;
        }
    }
}
