using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Models.BalanceQueue
{
    public class BalanceQueue
    {
        /// <summary>
        /// Circle 中的最大个数
        /// </summary>
        private int MaxCountInCircle;

        /// <summary>
        /// 最大等待匹配时间
        /// </summary>
        private TimeSpan MaxWaitPairTimeSpan;

        private event PairedOrNotNeedPairHandler PairedOrNotNeedPair;

        public BalanceQueue()
        {

        }

        public BalanceQueue(PairedOrNotNeedPairHandler pairedOrNotNeedPairHandler, int maxCountInCircle, TimeSpan maxWaitPairTimeSpan)
        {
            this.PairedOrNotNeedPair = pairedOrNotNeedPairHandler;
            this.MaxCountInCircle = maxCountInCircle;
            this.MaxWaitPairTimeSpan = maxWaitPairTimeSpan;
        }

        public BalanceQueueInfo Info { get; set; }

        public List<BalanceQueueInfo> Infos { get; set; }

        /// <summary>
        /// 进入 Queue
        /// 外部需要填充的Info信息：
        /// MacAddr
        /// ProcessName
        /// BusinessType
        /// BusinessID
        /// FilePath
        /// Brief
        /// </summary>
        /// <param name="reponsitory"></param>
        public void EnterQueue(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            //获取 ProcessID：根据 MacAddr, ProcessName, BusinessType, ProcessStatus(100-Queue中), Status(200) 查出最大的 ProcessID 
            var maxProcessIDModelInQueue = new Views.BalanceQueue.BalanceQueueView(reponsitory).GetBalanceQueueInfo()
                .Where(t => t.Status == Enums.Status.Normal && t.ProcessStatus == Enums.BalanceQueueProcessStatus.InQueue
                        && t.MacAddr == this.Info.MacAddr && t.ProcessName == this.Info.ProcessName && t.BusinessType == this.Info.BusinessType)
                .OrderByDescending(t => t.ProcessID).FirstOrDefault();

            this.Info.InfoID = Guid.NewGuid().ToString("N");
            this.Info.ProcessStatus = Enums.BalanceQueueProcessStatus.InQueue;
            this.Info.ProcessID = maxProcessIDModelInQueue != null ? (maxProcessIDModelInQueue.ProcessID + 1) : 0;
            this.Info.Status = Enums.Status.Normal;
            this.Info.CreateDate = DateTime.Now;
            this.Info.UpdateDate = DateTime.Now;
            this.Info.Insert(reponsitory);

            new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = this.Info.InfoID,
                NewProcessName = this.Info.ProcessName,
                NewProcessStatus = this.Info.ProcessStatus,
                NewProcessID = this.Info.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Insert(reponsitory);
        }

        /// <summary>
        /// 进入 Queue
        /// </summary>
        public void EnterQueue()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(isAutoSumit: false))
            {
                this.EnterQueue(reponsitory);
                reponsitory.Submit();
            }
        }


        public void CoreHandler(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            Needs.Utils.Flow.Process.Context<ContextParam, NodeParam> context = new Needs.Utils.Flow.Process.Context<ContextParam, NodeParam>(new ContextParam()
            {
                Reponsitory = reponsitory,
                MacAddr = this.Info.MacAddr,
                ProcessName = this.Info.ProcessName,
                BusinessType = this.Info.BusinessType,
                MaxCountInCircle = this.MaxCountInCircle,
                MaxWaitPairTimeSpan = this.MaxWaitPairTimeSpan,
            });

            context.Param.PairedOrNotNeedPair += this.PairedOrNotNeedPair;

            context.Node = new CirclePop<ContextParam, NodeParam>();
            context.Execute(new NodeParam());

        }

        public void CoreHandler()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(isAutoSumit: false))
            {
                this.CoreHandler(reponsitory);
                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 设置 BalanceQueueInfo 的状态
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="balanceQueueProcessStatus"></param>
        public void SetProcessStatus(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Enums.BalanceQueueProcessStatus balanceQueueProcessStatus)
        {
            new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = this.Info.InfoID,
                OldProcessName = this.Info.ProcessName,
                NewProcessName = this.Info.ProcessName,
                OldProcessStatus = this.Info.ProcessStatus,
                NewProcessStatus = balanceQueueProcessStatus,
                OldProcessID = this.Info.ProcessID,
                NewProcessID = this.Info.ProcessID,
                Status = Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }.Insert(reponsitory);

            this.Info.ProcessStatus = balanceQueueProcessStatus;
            this.Info.UpdateDate = DateTime.Now;
            this.Info.Update(reponsitory);
        }

        /// <summary>
        /// 设置 BalanceQueueInfo 的状态
        /// </summary>
        /// <param name="balanceQueueProcessStatus"></param>
        public void SetProcessStatus(Enums.BalanceQueueProcessStatus balanceQueueProcessStatus)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(isAutoSumit: false))
            {
                this.SetProcessStatus(reponsitory, balanceQueueProcessStatus);
                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 批量设置 BalanceQueueInfo 的状态
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="balanceQueueProcessStatus"></param>
        public void BatchSetProcessStatus(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Enums.BalanceQueueProcessStatus balanceQueueProcessStatus)
        {
            List<Layer.Data.Sqls.ScCustoms.BalanceQueueRecords> balanceQueueRecords = new List<Layer.Data.Sqls.ScCustoms.BalanceQueueRecords>();
            foreach (var item in this.Infos)
            {
                balanceQueueRecords.Add(new Layer.Data.Sqls.ScCustoms.BalanceQueueRecords()
                {
                    RecordID = Guid.NewGuid().ToString("N"),
                    InfoID = item.InfoID,
                    OldProcessName = item.ProcessName,
                    NewProcessName = item.ProcessName,
                    OldProcessStatus = (int)item.ProcessStatus,
                    NewProcessStatus = (int)balanceQueueProcessStatus,
                    OldProcessID = item.ProcessID,
                    NewProcessID = item.ProcessID,
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }
            reponsitory.Insert(balanceQueueRecords.ToArray());

            List<string> infoIds = this.Infos.Select(t => t.InfoID).ToList();
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.BalanceQueueInfos>(new { ProcessStatus = (int)balanceQueueProcessStatus }, item => infoIds.Contains(item.InfoID));
        }

        /// <summary>
        /// 批量设置 BalanceQueueInfo 的状态
        /// </summary>
        /// <param name="reponsitory"></param>
        public void BatchSetProcessStatus(Enums.BalanceQueueProcessStatus balanceQueueProcessStatus)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(isAutoSumit: false))
            {
                this.BatchSetProcessStatus(reponsitory, balanceQueueProcessStatus);
                reponsitory.Submit();
            }
        }
    }
}
