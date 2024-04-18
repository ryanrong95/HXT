using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class NewDeliveryInfoSubmitModel
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 地址的值
        /// </summary>
        public string[] Address { get; set; }

        /// <summary>
        /// 详细地址的值
        /// </summary>
        public string AddressDetail { get; set; }
    }
}