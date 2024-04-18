using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 标准品牌
    /// </summary>
    public class StandardManufacturer : IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { get; set; }
    }
}
