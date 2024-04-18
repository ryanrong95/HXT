using Needs.Utils.Flow.Event;
using Needs.Utils.Flow.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueue.CoreStep
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
            param.Reponsitory = context.Param.Reponsitory;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;

            var minProcessIDModelInCircle = (Models.BalanceQueue.BalanceQueueInfo)base.EventBuilder.Execute(param);

            if (minProcessIDModelInCircle != null)
            {
                context.Param.MinProcessIDModelInCircle = minProcessIDModelInCircle;

                context.Node = new SearchPair<TParam, TNode>();
                context.Execute(new TNode());
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

            var minProcessIDModelInCircle = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InCircle
                        && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType)
                .OrderBy(t => t.ProcessID).FirstOrDefault();

            return minProcessIDModelInCircle;
        }
    }
}
