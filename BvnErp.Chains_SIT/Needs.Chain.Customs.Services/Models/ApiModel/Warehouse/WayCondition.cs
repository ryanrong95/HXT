using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 运输条件
    /// </summary>
    public class WayCondition
    {
        /// <summary>
        /// 拆箱验货
        /// </summary>
        public bool UnBoxed { get; set; }

        /// <summary>
        /// 垫付运费
        /// </summary>
        public bool PayForFreight { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public bool IsCharterBus { get; set; }

        /// <summary>
        /// 重新包装
        /// </summary>
        public bool Repackaging { get; set; }

        /// <summary>
        /// 真空包装
        /// </summary>
        public bool VacuumPackaging { get; set; }

        /// <summary>
        /// 防水包装
        /// </summary>
        public bool WaterproofPackaging { get; set; }

        /// <summary>
        /// 代垫货款
        /// </summary>
        public bool AgencyPayment { get; set; }

        /// <summary>
        /// 代收货款
        /// </summary>
        public bool AgencyReceive { get; set; }

        /// <summary>
        /// 重新包装
        /// </summary>
        public bool ChangePackaging { get; set; }

        /// <summary>
        /// 标签服务
        /// </summary>
        public bool LableServices { get; set; }

        /// <summary>
        /// 代检查
        /// </summary>
        public bool AgencyCheck { get; set; }

    }
}
