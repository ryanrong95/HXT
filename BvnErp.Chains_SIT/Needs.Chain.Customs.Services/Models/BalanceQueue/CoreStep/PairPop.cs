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
    /// Step 3
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class PairPop<TParam, TNode> : BaseNode<TParam, TNode>
        where TParam : ContextParam
        where TNode : NodeParam, new()
    {
        public PairPop()
        {
            base.EventBuilder.Append(PopMinProcessIDModelInCircle);
            base.EventBuilder.Append(PopPairModelInCircleAndQueue);
        }

        public override object Handler(Context<TParam, TNode> context, TNode param)
        {
            param.Reponsitory = context.Param.Reponsitory;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;

            param.PairPopParam.PairCode = Guid.NewGuid().ToString("N");
            param.PairPopParam.MinProcessIDModelInCircle = context.Param.MinProcessIDModelInCircle;
            param.PairPopParam.PairModelInCircleAndQueue = context.Param.PairModelInCircleAndQueue;

            base.EventBuilder.Execute(param);

            context.Param.Reponsitory.Submit();

            context.Node = new CirclePush<TParam, TNode>();
            context.Execute(new TNode());

            return null;
        }

        private object PopMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PairPopParam.MinProcessIDModelInCircle;  

            new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = minProcessIDModelInCircle.InfoID,
                OldProcessName = minProcessIDModelInCircle.ProcessName,
                NewProcessName = minProcessIDModelInCircle.ProcessName,
                OldProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                NewProcessStatus = Enums.BalanceQueueProcessStatus.NormalOut,
                OldProcessID = minProcessIDModelInCircle.ProcessID,
                NewProcessID = minProcessIDModelInCircle.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Insert(entity.Reponsitory);

            minProcessIDModelInCircle.ProcessStatus = Enums.BalanceQueueProcessStatus.NormalOut;
            minProcessIDModelInCircle.PairCode = entity.PairPopParam.PairCode;
            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            minProcessIDModelInCircle.Update(entity.Reponsitory);

            return null;
        }

        private object PopPairModelInCircleAndQueue(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var pairModelInCircleAndQueue = entity.PairPopParam.PairModelInCircleAndQueue;

            new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = pairModelInCircleAndQueue.InfoID,
                OldProcessName = pairModelInCircleAndQueue.ProcessName,
                NewProcessName = pairModelInCircleAndQueue.ProcessName,
                OldProcessStatus = pairModelInCircleAndQueue.ProcessStatus,
                NewProcessStatus = Enums.BalanceQueueProcessStatus.NormalOut,
                OldProcessID = pairModelInCircleAndQueue.ProcessID,
                NewProcessID = pairModelInCircleAndQueue.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Insert(entity.Reponsitory);

            pairModelInCircleAndQueue.ProcessStatus = Enums.BalanceQueueProcessStatus.NormalOut;
            pairModelInCircleAndQueue.PairCode = entity.PairPopParam.PairCode;
            pairModelInCircleAndQueue.UpdateDate = DateTime.Now;
            pairModelInCircleAndQueue.Update(entity.Reponsitory);

            return null;
        }
    }
}
