using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CustomsClearance : IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginCountry { get; set; }
        /// <summary>
        /// 两位的原产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 8位的海关编码
        /// </summary>
        public string HSCode8
        {
            get
            {
                return this.HSCode.Substring(0, 8);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 法一数量
        /// </summary>
        public decimal FirstQty { get; set; }
        /// <summary>
        /// 法一单位
        /// </summary>
        public string Unit { get; set; }
        public string UnitName { get; set; }

        public string UnitShow
        {
            get
            {
                switch (this.UnitName)
                {
                    case ("个"):
                        return "C62";

                    case ("千克"):
                        return "KGM";

                    case ("千个"):
                        return "MIL";

                    default:
                        return "-";
                }
            }
        }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DecTotal { get; set; }
        /// <summary>
        /// 币制
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWt { get; set; }
        public string VoyNo { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
