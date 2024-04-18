using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 班别管理
    /// </summary>
    public class Scheduling
    {
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 上午开始时间
        /// </summary>
        public TimeSpan? AmStartTime { get; set; }

        /// <summary>
        /// 上午结束时间
        /// </summary>
        public TimeSpan? AmEndTime { get; set; }

        /// <summary>
        /// 下午开始时间
        /// </summary>
        public TimeSpan PmStartTime { get; set; }

        /// <summary>
        /// 下午结束时间
        /// </summary>
        public TimeSpan PmEndTime { get; set; }

        /// <summary>
        /// 判断迟到早退的阈值  以分钟为单位
        /// </summary>
        public int DomainValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }
}
