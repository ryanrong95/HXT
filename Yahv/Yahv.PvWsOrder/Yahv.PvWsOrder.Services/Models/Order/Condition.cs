using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 订单条件
    /// </summary>
    public class OrderCondition
    {
        /// <summary>
        /// 是否自定义标签
        /// </summary>
        public bool IsCustomLabel { get;  set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public bool IsDetection { get; set; }

        /// <summary>
        /// 是否重新包装
        /// </summary>
        public bool IsRepackaging { get; set; }

        /// <summary>
        /// 是否真空包装
        /// </summary>
        public bool IsVacuumPackaging { get; set; }
        
        /// <summary>
        /// 是否防水包装
        /// </summary>
        public bool IsWaterproofPackaging { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 是否高价值
        /// </summary>
        public bool IsHighValue { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsInspection { get; set; }

        /// <summary>
        /// 是否消毒
        /// </summary>
        public bool IsQuarantine { get; set; }

        /// <summary>
        /// 是否CCC
        /// </summary>
        public bool IsCCC { get; set; }

    }

    /// <summary>
    /// 订单项条件
    /// </summary>
    public class OrderItemCondition
    {
        /// <summary>
        /// 是否自定义标签
        /// </summary>
        public bool IsCustomLabel { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public bool IsDetection { get; set; }

        /// <summary>
        /// 是否重新包装
        /// </summary>
        public bool IsRepackaging { get; set; }

        /// <summary>
        /// 是否真空包装
        /// </summary>
        public bool IsVacuumPackaging { get; set; }

        /// <summary>
        /// 是否防水包装
        /// </summary>
        public bool IsWaterproofPackaging { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 是否涂抹Lot码
        /// </summary>
        public bool IsDaubLotCode { get; set; }

        /// <summary>
        /// 是否称重
        /// </summary>
        public bool IsWeighing { get; set; }

        /// <summary>
        /// 是否点数
        /// </summary>
        public bool CheckNumber { get; set; }

        /// <summary>
        /// 是否可报关
        /// </summary>
        public bool IsDeclare { get; set; }
    }
}
