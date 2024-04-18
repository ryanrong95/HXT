using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetMyStorageListSearchModel
    {
        /// <summary>
        /// page
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// rows
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string Code { get; set; }
    }
}