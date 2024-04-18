using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class NewTakingInfoSubmitModel
    {
        /// <summary>
        /// 提货人的值
        /// </summary>
        public string TakingMan { get; set; }

        /// <summary>
        /// 提货人电话的值
        /// </summary>
        public string TakingTel { get; set; }

        /// <summary>
        /// 证件类型的值
        /// </summary>
        public string ProofTypeValue { get; set; }

        /// <summary>
        /// 证件号码的值
        /// </summary>
        public string ProofNumber { get; set; }
    }
}