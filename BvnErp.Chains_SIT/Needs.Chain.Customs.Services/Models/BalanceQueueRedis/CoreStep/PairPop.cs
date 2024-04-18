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
    /// Step 3
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class PairPop<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : BalanceQueueRedis.CoreStep.ContextParam
        where TNode : BalanceQueueRedis.CoreStep.NodeParam, new()
    {
        public PairPop()
        {
            base.EventBuilder.Append(DeleteFromXmlSet);
            base.EventBuilder.Append(DeleteFromFailBoxSet);
            base.EventBuilder.Append(SetInAlonePairSet);
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.RedisKey = context.Param.RedisKey;
            param.Redis = context.Param.Redis;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;

            param.PairPopParam.PairCode = Guid.NewGuid().ToString("N");
            param.PairPopParam.MinProcessIDModelInCircle = context.Param.MinProcessIDModelInCircle;
            param.PairPopParam.PairModelInCircleAndQueue = context.Param.PairModelInCircleAndQueue;

            base.EventBuilder.Execute(param);

            context.Node = new CirclePush<TParam, TNode>();
            context.Execute(new TNode());

            return null;
        }

        private object DeleteFromXmlSet(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PairPopParam.MinProcessIDModelInCircle;

            entity.Redis.HashDelete(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID);

            return null;
        }

        private object DeleteFromFailBoxSet(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PairPopParam.MinProcessIDModelInCircle;

            entity.Redis.HashDelete(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID);

            return null;
        }

        private object SetInAlonePairSet(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PairPopParam.MinProcessIDModelInCircle;

            entity.Redis.HashSet<BalanceQueueInfo>(entity.RedisKey.AlonePairSet, minProcessIDModelInCircle.BusinessID, minProcessIDModelInCircle);

            return null;
        }
    }
}
