using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 日程安排
    /// </summary>
    public class Schedule : IUnique
    {
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 日程安排类型
        /// </summary>
        public ScheduleType Type { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifyID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 日期字符串
        /// </summary>
        public string DateString => this.Date.ToString("yyyy-MM-dd");
        #endregion
    }
}