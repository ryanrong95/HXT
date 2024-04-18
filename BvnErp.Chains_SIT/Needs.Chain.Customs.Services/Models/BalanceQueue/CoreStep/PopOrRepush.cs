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
            param.Reponsitory = context.Param.Reponsitory;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.PopOrRepushParam.MinProcessIDModelInCircle = context.Param.MinProcessIDModelInCircle;

            var minProcessIDModelInCircle = (Models.BalanceQueue.BalanceQueueInfo)context.Param.MinProcessIDModelInCircle;
            if (DateTime.Now - minProcessIDModelInCircle.CreateDate > context.Param.MaxWaitPairTimeSpan)
            {
                base.EventBuilder.Append(PopMinProcessIDModelInCircle);
            }
            else
            {
                base.EventBuilder.Append(RepushMinProcessIDModelInCircle);
            }

            base.EventBuilder.Execute(param);

            context.Param.Reponsitory.Submit();

            context.Node = new CirclePush<TParam, TNode>();
            context.Execute(new TNode());

            return null;
        }

        private object PopMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PopOrRepushParam.MinProcessIDModelInCircle;

            new BalanceQueueRecord()
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
            }.Insert(entity.Reponsitory);

            minProcessIDModelInCircle.ProcessStatus = Enums.BalanceQueueProcessStatus.ExceptionOut;
            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            minProcessIDModelInCircle.Update(entity.Reponsitory);

            return null;
        }

        private object RepushMinProcessIDModelInCircle(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;
            var minProcessIDModelInCircle = entity.PopOrRepushParam.MinProcessIDModelInCircle;

            var maxProcessIDModelInCircle = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                    .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InCircle
                            && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType)
                    .OrderByDescending(t => t.ProcessID).FirstOrDefault();

            new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = minProcessIDModelInCircle.InfoID,
                OldProcessName = minProcessIDModelInCircle.ProcessName,
                NewProcessName = minProcessIDModelInCircle.ProcessName,
                OldProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                NewProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                OldProcessID = minProcessIDModelInCircle.ProcessID,
                NewProcessID = maxProcessIDModelInCircle.ProcessID + 1,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Insert(entity.Reponsitory);

            minProcessIDModelInCircle.ProcessID = maxProcessIDModelInCircle.ProcessID + 1;
            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            minProcessIDModelInCircle.Update(entity.Reponsitory);

            return null;
        }
    }
}
