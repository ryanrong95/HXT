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
    /// Step 1
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class CirclePop<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : ContextParam
        where TNode : NodeParam, new()
    {
        public CirclePop()
        {
            base.EventBuilder.Append(GetMinProcessIDModelInCircle);
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.RedisKey = context.Param.RedisKey;
            param.Redis = context.Param.Redis;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;

            var minProcessIDModelInCircle = (Models.BalanceQueueRedis.BalanceQueueInfo)base.EventBuilder.Execute(param);

            if (minProcessIDModelInCircle != null)
            {
                context.Param.MinProcessIDModelInCircle = minProcessIDModelInCircle;

                if (context.Param.OnCheckIsUnNeedPair())
                {
                    context.Param.IsUnNeedPair = true;

                    RemoveTheUnPairFromSet(context.Param);

                    context.Node = new CirclePush<TParam, TNode>();
                    context.Execute(new TNode());
                }
                else
                {
                    context.Node = new SearchPair<TParam, TNode>();
                    context.Execute(new TNode());
                }
            }
            else
            {
                context.Node = new CirclePush<TParam, TNode>();
                context.Execute(new TNode());
            }

            return null;
        }

        private object GetMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;

            var minProcessIDModelInCircle = entity.Redis.ListLeftPop<BalanceQueueRedis.BalanceQueueInfo>(entity.RedisKey.CircleList);
            return minProcessIDModelInCircle;
        }

        /// <summary>
        /// 将无需配对的从对应的Set中移除
        /// </summary>
        /// <param name="contextParam"></param>
        private void RemoveTheUnPairFromSet(TParam contextParam)
        {
            var entity = contextParam;
            var minProcessIDModelInCircle = entity.MinProcessIDModelInCircle;

            if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".xml"))
            {
                if (entity.Redis.HashExists(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID))
                {
                    entity.Redis.HashDelete(entity.RedisKey.XmlSet, minProcessIDModelInCircle.BusinessID);
                }
            }
            else if (!string.IsNullOrEmpty(minProcessIDModelInCircle.FilePath) && minProcessIDModelInCircle.FilePath.ToLower().EndsWith(".zip"))
            {
                if (entity.Redis.HashExists(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID))
                {
                    entity.Redis.HashDelete(entity.RedisKey.FailBoxSet, minProcessIDModelInCircle.BusinessID);
                }
            }
        }
    }
}
