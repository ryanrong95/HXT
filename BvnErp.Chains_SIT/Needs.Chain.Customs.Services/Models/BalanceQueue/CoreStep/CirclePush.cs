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
            param.Reponsitory = context.Param.Reponsitory;
            param.MacAddr = context.Param.MacAddr;
            param.ProcessName = context.Param.ProcessName;
            param.BusinessType = context.Param.BusinessType;
            param.MaxCountInCircle = context.Param.MaxCountInCircle;

            base.EventBuilder.Execute(param);

            context.Param.Reponsitory.Submit();

            //匹配后或不需要匹配处理方式
            context.Param.OnPairedOrNotNeedPair();

            return null;
        }

        private object CirclePushFromQueuePop(object sender, EventArgs<TNode> e)
        {
            var entity = (TNode)e.Entity;

            int currentCountInCircle = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InCircle
                        && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType).Count();

            if (currentCountInCircle < entity.MaxCountInCircle)
            {
                var maxProcessIDModelInCircle = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                    .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InCircle
                            && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType)
                    .OrderByDescending(t => t.ProcessID).FirstOrDefault();
                long currentMaxProcessIDInCircle = maxProcessIDModelInCircle != null ? maxProcessIDModelInCircle.ProcessID : -1;


                var minProcessIDModelsInQueue = new Views.BalanceQueue.BalanceQueueView(entity.Reponsitory).GetBalanceQueueInfo()
                    .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InQueue
                            && t.MacAddr == entity.MacAddr && t.ProcessName == entity.ProcessName && t.BusinessType == entity.BusinessType)
                    .OrderBy(t => t.ProcessID).Take(entity.MaxCountInCircle - currentCountInCircle).ToList();

                if (minProcessIDModelsInQueue != null && minProcessIDModelsInQueue.Any())
                {
                    foreach (var item in minProcessIDModelsInQueue)
                    {
                        currentMaxProcessIDInCircle += 1;

                        new BalanceQueueRecord()
                        {
                            RecordID = Guid.NewGuid().ToString("N"),
                            InfoID = item.InfoID,
                            OldProcessName = item.ProcessName,
                            NewProcessName = item.ProcessName,
                            OldProcessStatus = item.ProcessStatus,
                            NewProcessStatus = Enums.BalanceQueueProcessStatus.InCircle,
                            OldProcessID = item.ProcessID,
                            NewProcessID = currentMaxProcessIDInCircle,
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                        }.Insert(entity.Reponsitory);

                        item.ProcessStatus = Enums.BalanceQueueProcessStatus.InCircle;
                        item.ProcessID = currentMaxProcessIDInCircle;
                        item.UpdateDate = DateTime.Now;
                        item.Update(entity.Reponsitory);
                    }
                }
            }


            return null;
        }
    }
}
