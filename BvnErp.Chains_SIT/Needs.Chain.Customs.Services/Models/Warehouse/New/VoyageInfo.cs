using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class VoyageInfoViewModel
    {
        public string VoyageNo { get; set; }
        public CutStatus CutStatus { get; set; }
        public string CutStatusName
        {
            get
            {
                return CutStatus.GetDescription();
            }
        }
        /// <summary>
        /// 封条号
        /// </summary>
        public string SealNo { get; set; }
        public string CarrierName { get; set; }
        public VoyageType VoyageType { get; set; }
        public string VoyageTypeName
        {
            get
            {
                return VoyageType.GetDescription();
            }
        }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public DateTime? TransportTime { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public decimal TotalQuantity { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public decimal TotalItems { get; set; }
        /// <summary>
        /// 总箱数
        /// </summary>
        public decimal TotalPackNo { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal TotalGrossWt { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        public List<PickupFile> VoyageFiles { get; set; }
    }
}
