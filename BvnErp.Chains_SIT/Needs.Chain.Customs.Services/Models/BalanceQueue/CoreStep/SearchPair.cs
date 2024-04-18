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
            base.EventBuilder.Append(GetPairModelInCircleAndQueue);
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.Reponsitory = context.Param.Reponsitory;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.SearchPairParam.InfoID = context.Param.MinProcessIDModelInCircle.InfoID;
            param.SearchPairParam.BusinessID = context.Param.MinProcessIDModelInCircle.BusinessID;

            var pairModelInCircleAndQueue = (Models.BalanceQueue.BalanceQueueInfo)base.EventBuilder.Execute(param);

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

            return null;
        }

        private object GetPairModelInCircleAndQueue(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;

            var pairModelInCircleAndQueue = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                .Where(t => t.Status == Enums.Status.Normal
                        && new Enums.BalanceQueueProcessStatus[] { Enums.BalanceQueueProcessStatus.InCircle, Enums.BalanceQueueProcessStatus.InQueue }.Contains(t.ProcessStatus)
                        && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType
                        && t.BusinessID == entity.SearchPairParam.BusinessID && t.InfoID != entity.SearchPairParam.InfoID)
                .OrderBy(t => t.ProcessID).FirstOrDefault();

            return pairModelInCircleAndQueue;
        }
    }
}
