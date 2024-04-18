using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.BalanceQueueRedis
{
    /// <summary>
    /// BalanceQueue 主信息
    /// </summary>
    [Serializable]
    public class BalanceQueueInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string InfoID { get; set; } = string.Empty;

        /// <summary>
        /// 客户端机器MAC地址
        /// </summary>
        public string MacAddr { get; set; } = string.Empty;

        /// <summary>
        /// 哪个业务中的哪个过程，用一个字符串表示，比如：FailBox、InBox
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 所要平衡的各种类型(同一个BusinessID的不同类型)，比如：报关单、舱单
        /// </summary>
        public Enums.BalanceQueueBusinessType BusinessType { get; set; }

        /// <summary>
        /// 在某个业务的某个过程中的“业务ID”，比如：DecHeadID、仓单ID
        /// </summary>
        public string BusinessID { get; set; } = string.Empty;

        /// <summary>
        /// 所对应的文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 简明内容
        /// </summary>
        public string Brief { get; set; } = string.Empty;

        /// <summary>
        /// 所处的处理状态(100-Queue中, 200-Circle中, 300-已取出【异常取出】, 400-已取出【正常取出】)
        /// </summary>
        public Enums.BalanceQueueProcessStatus ProcessStatus { get; set; }

        /// <summary>
        /// 处理ID
        /// </summary>
        public long ProcessID { get; set; }

        /// <summary>
        /// 配对码
        /// </summary>
        public string PairCode { get; set; } = string.Empty;

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
            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BalanceQueueInfos
            {
                InfoID = this.InfoID,
                MacAddr = this.MacAddr,
                ProcessName = this.ProcessName,
                BusinessType = (int)this.BusinessType,
                BusinessID = this.BusinessID,
                FilePath = this.FilePath,
                Brief = this.Brief,
                ProcessStatus = (int)this.ProcessStatus,
                ProcessID = this.ProcessID,
                PairCode = this.PairCode,
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
            reponsitory.Update(new Layer.Data.Sqls.ScCustoms.BalanceQueueInfos
            {
                InfoID = this.InfoID,
                MacAddr = this.MacAddr,
                ProcessName = this.ProcessName,
                BusinessType = (int)this.BusinessType,
                BusinessID = this.BusinessID,
                FilePath = this.FilePath,
                Brief = this.Brief,
                ProcessStatus = (int)this.ProcessStatus,
                ProcessID = this.ProcessID,
                PairCode = this.PairCode,
                Status = (int)this.Status,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate,
                Summary = this.Summary,
            }, item => item.InfoID == this.InfoID);
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BalanceQueueInfos>().Count(item => item.InfoID == this.InfoID);

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
