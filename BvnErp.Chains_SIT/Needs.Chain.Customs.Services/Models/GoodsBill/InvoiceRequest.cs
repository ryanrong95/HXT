using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceRequest
    {
        /// <summary>
        /// 发票代码（长度10位或者12位） 必填
        /// </summary>
        public string invoiceCode { get; set; }
        /// <summary>
        /// 发票号码（长度8位）  必填
        /// </summary>
        public string invoiceNumber { get; set; }
        /// <summary>
        /// 开票时间（时间格式必须为：2017-05-11，不支持其他格式）     必填
        /// </summary>
        public string billTime { get; set; }
        /// <summary>
        /// 校验码（检验码必须是后六位！，增值税专用发票，增值税机动车发票，二手车统一发票可以不传）        
        /// </summary>
        public string checkCode { get; set; }
        /// <summary>
        /// 开具金额、不含税价（增值税普通发票，增值税电子发票，卷式发票，电子普通[通行费]发票可以不传）     
        /// </summary>
        public string invoiceAmount { get; set; }
        /// <summary>
        /// 授权码  必填
        /// </summary>
        public string token { get; set; }
    }
}
