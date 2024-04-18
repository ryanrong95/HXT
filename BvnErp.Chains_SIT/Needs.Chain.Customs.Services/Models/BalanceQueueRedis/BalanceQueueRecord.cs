using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis
{
    /// <summary>
    /// BalanceQueue 记录
    /// </summary>
    [Serializable]
    public class BalanceQueueRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string RecordID { get; set; } = string.Empty;

        /// <summary>
        /// InfoID
        /// </summary>
        public string InfoID { get; set; } = string.Empty;

        /// <summary>
        /// 旧的 ProcessName
        /// </summary>
        public string OldProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 新的 ProcessName
        /// </summary>
        public string NewProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 旧的 ProcessStatus
        /// </summary>
        public Enums.BalanceQueueProcessStatus? OldProcessStatus { get; set; }

        /// <summary>
        /// 新的 ProcessStatus
        /// </summary>
        public Enums.BalanceQueueProcessStatus NewProcessStatus { get; set; }

        /// <summary>
        /// 旧的 ProcessID
        /// </summary>
        public long? OldProcessID { get; set; }

        /// <summary>
        /// 新的 ProcessID
        /// </summary>
        public long NewProcessID { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="reponsitory"></param>
        public void Insert(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BalanceQueueRecords
            {
                RecordID = this.RecordID,
                InfoID = this.InfoID,
                OldProcessName = this.OldProcessName,
                NewProcessName = this.NewProcessName,
                OldProcessStatus = this.OldProcessStatus != null ? (int)this.OldProcessStatus : 0,
                NewProcessStatus = (int)this.NewProcessStatus,
                OldProcessID = this.OldProcessID ?? -1,
                NewProcessID = this.NewProcessID,
                Status = (int)this.Status,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate,
                Summary = this.Summary,
            });
        }

        /// <summary>
        /// 新增
        /// </summary>
        public void Insert()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.Insert(reponsitory);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="reponsitory"></param>
        public void Update(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update(new Layer.Data.Sqls.ScCustoms.BalanceQueueRecords
            {
                RecordID = this.RecordID,
                InfoID = this.InfoID,
                OldProcessName = this.OldProcessName,
                NewProcessName = this.NewProcessName,
                OldProcessStatus = this.OldProcessStatus != null ? (int)this.OldProcessStatus : 0,
                NewProcessStatus = (int)this.NewProcessStatus,
                OldProcessID = this.OldProcessID ?? -1,
                NewProcessID = this.NewProcessID,
                Status = (int)this.Status,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate,
                Summary = this.Summary,
            }, item => item.RecordID == this.RecordID);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.Update(reponsitory);
            }
        }

        public void NoDuplicateOperation()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BalanceQueueRecords>().Count(item => item.RecordID == this.RecordID);

                if (count == 0)
                {
                    this.Insert(reponsitory);
                }
                else
                {
                    this.Update(reponsitory);
                }
            }
        }
    }
}
