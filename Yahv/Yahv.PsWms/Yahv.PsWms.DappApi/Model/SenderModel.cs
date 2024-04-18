using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi
{
    public class SenderModel
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        /// <remarks>
        /// 需要后续修改
        /// </remarks>
        public string Company { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Contact { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
    }
}