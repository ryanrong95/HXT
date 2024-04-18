using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
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
        /// OrderItemID
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
        public Yahv.Services.Models.Admin Locker { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        /// <summary>
        /// 归类锁定，内部查询使用
        /// </summary>
        internal Lock_Classify()
        {

        }
    }
}
