using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class ProductParas /*: Obj*/
    {
        /// <summary>
        /// 进项编号
        /// </summary>
        public string inputsID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Catalog { get; set; }
        
        public int Quantity { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商、厂商、品牌
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
        /// 来源
        /// </summary>
        public string origin { get; set; }
    }
}
