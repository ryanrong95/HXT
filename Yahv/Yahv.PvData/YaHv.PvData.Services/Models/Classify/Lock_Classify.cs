using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 归类锁定
    /// </summary>
    public class Lock_Classify : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// OrderItemID/预归类产品ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime LockDate { get; set; }

        /// <summary>
        /// 锁定人
        /// </summary>
        public string LockerID { get; set; }
        public string LockerName { get; set; }
        public Admin Locker { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.Locks_Classify>().Any(t => t.ID == this.ID))
                {
                    this.ID = Utils.GuidUtil.NewGuidUp();
                    repository.Insert(new Layers.Data.Sqls.PvData.Locks_Classify()
                    {
                        ID = this.ID,
                        MainID = this.MainID,
                        LockDate = DateTime.Now,
                        LockerID = this.LockerID,
                        LockerName = this.LockerName,
                        Status = (int)this.Status,
                        Summary = this.Summary
                    });
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
