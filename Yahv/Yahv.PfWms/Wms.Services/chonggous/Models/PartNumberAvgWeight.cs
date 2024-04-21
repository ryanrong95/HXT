using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous.Models
{
    public class PartNumberAvgWeight
    {
        /// <summary>
        /// 产品型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 产品平均单重
        /// </summary>
        public decimal? AVGWeight { get; set; }
    }
}
