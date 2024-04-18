using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetOutStorageFeeListReturnModel
    {
        /// <summary>
        /// 期号
        /// </summary>
        public string CutDateIndex { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string TotalDes { get; set; }
    }
}