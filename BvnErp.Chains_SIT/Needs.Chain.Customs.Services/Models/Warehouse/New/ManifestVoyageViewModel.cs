using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestVoyageViewModel
    {
        /// <summary>
        /// 货物运输批次号
        /// </summary>
        public string VoyageNo { get; set; } = string.Empty;

        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; } = string.Empty;

        /// <summary>
        /// 车牌号
        /// </summary>
        public string HKLicense { get; set; } = string.Empty;

        /// <summary>
        /// 大陆车牌
        /// </summary>
        public string VehicleLicence { get; set; } = string.Empty;

        /// <summary>
        /// 运输时间
        /// </summary>
        public DateTime? TransportTime { get; set; }

        /// <summary>
        /// 驾驶员姓名
        /// </summary>
        public string DriverName { get; set; } = string.Empty;

        /// <summary>
        /// 运输类型
        /// </summary>
        public VoyageType VoyageType { get; set; }

        /// <summary>
        /// 截单状态
        /// </summary>
        public CutStatus CutStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public string HKSealNumber { get; set; }
    }
}
