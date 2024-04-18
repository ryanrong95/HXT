using Needs.Linq;
using System;


namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 中港报关
    /// </summary>

    public class WayChcd : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 货物运输批次号
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// 两地车牌1
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 两地车牌2
        /// </summary>
        public string CarNumber2 { get; set; }

        /// <summary>
        /// 汽车荷载量
        /// </summary>
        public int? Carload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOnevehicle { get; set; }

        /// <summary>
        /// 司机名字
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 计划拉货时间
        /// </summary>
        public DateTime? PlanDate { get; set; }

        /// <summary>
        /// 实际起运时间
        /// </summary>
        public DateTime? DepartDate { get; set; }

        /// <summary>
        /// 总数量，报关部特殊要求
        /// </summary>
        public int? TotalQuantity { get; set; }

        #endregion

    }
}
