using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class Product
    {
        /// <summary>
        /// 品名
        /// </summary>
        public string Catalog { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packing { get; set; }
        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { get; set; } 
        /// <summary>
        /// 单毛重上界
        /// </summary>
        public decimal? UnitGrossWeightTL { get; set; }
        /// <summary>
        /// 单毛重下界
        /// </summary>
        public decimal? UnitGrossWeightBL { get; set; }
        /// <summary>
        /// 单体积
        /// </summary>
        public decimal? UnitGrossVolume { get; set; }
    }
}
