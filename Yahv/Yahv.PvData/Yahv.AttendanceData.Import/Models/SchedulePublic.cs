using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Enums;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 公共日程安排
    /// </summary>
    public class SchedulePublic : Linq.IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 安排方式：工作日、公休日、法定节假日
        /// </summary>
        public ScheduleMethod Method { get; set; }

        /// <summary>
        /// 安排名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ScheduleFrom From { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string RegionID { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 薪酬倍数
        /// </summary>
        public decimal? SalaryMultiple { get; set; }

        /// <summary>
        /// 受众
        /// </summary>
        public string ShiftID { get; set; }

        /// <summary>
        /// 实际班别
        /// </summary>
        public string SchedulingID { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        #endregion
    }
}
