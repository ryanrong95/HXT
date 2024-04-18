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
    /// Step 2
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class SearchPair<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : ContextParam
        where TNode : NodeParam, new()
    {
        public SearchPair()
        {
            
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.RedisKey = context.Param.RedisKey;
            param.Redis = context.Param.Redis;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.SearchPairParam.MinProcessIDModelInCircle = context.Param.MinProcessIDModelInCircle;

            base.EventBuilder = new EventBuilder<TNode>();
            base.EventBuilder.Append(GetFromAlonePairSet);
            var infoInAlonePairSet = (Models.BalanceQueueRedis.BalanceQueueInfo)base.EventBuilder.Execute(param);
            if (infoInAlonePairSet != null)
            {
                //配对的另一方已经执行完了
                context.Node = new CirclePush<TParam, TNode>();
                context.Execute(new TNode());
            }
            else
            {
                base.EventBuilder = new EventBuilder<TNode>();
                base.EventBuilder.Append(GetPairModelInCircleAndQueue);
                var pairModelInCircleAndQueue = (Models.BalanceQueueRedis.BalanceQueueInfo)base.EventBuilder.Execute(param);

                if (pairModelInCircleAndQueue != null)
                {
                    context.Param.IsSuccessPaired = true;
                    context.Param.PairModelInCircleAndQueue = pairModelInCircleAndQueue;
                    context.Node = new PairPop<TParam, TNode>();
                    context.Execute(new TNode());
                }
                else
                {
                    context.Node = new PopOrRepush<TParam, TNode>();
                    context.Execute(new TNode());
                }
            }

            return null;
        }

        private object GetFromAlonePairSet(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.SearchPairParam.MinProcessIDModelInCircle;

            BalanceQueueRedis.BalanceQueueInfo infoInAlonePairSet = null;
            if (entity.Redis.HashExists(entity.RedisKey.AlonePairSet, minProcessIDModelInCircle.BusinessID))
            {
                infoInAlonePairSet = entity.Redis.HashGet<BalanceQueueRedis.BalanceQueueInfo>(entity.RedisKey.AlonePairSet, minProcessIDModelInCircle.BusinessID);
                entity.Redis.HashDelete(entity.RedisKey.AlonePairSet, minProcessIDModelInCircle.BusinessID);
            }

            return infoInAlonePairSet;
        }

        private object GetPairModelInCircleAndQueue(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.SearchPairParam.MinProcessIDModelInCircle;

            BalanceQueueRedis.BalanceQueueInfo pairModelInCircleAndQueue = null;
            if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".xml"))
            {
                if (entity.Redis.HashExists(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID))
                {
                    pairModelInCircleAndQueue = entity.Redis.HashGet<BalanceQueueInfo>(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID);
                }
            }
            else if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".zip"))
            {
                if (entity.Redis.HashExists(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID))
                {
                    pairModelInCircleAndQueue = entity.Redis.HashGet<BalanceQueueInfo>(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID);
                }
            }

            return pairModelInCircleAndQueue;
        }
    }
}
