using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceResponse
    {
        /// <summary>
        /// 00或99  00：成功，99：失败
        /// </summary>
        public string RtnCode { get; set; }
        /// <summary>
        /// 1000或2001 查询发票状态码，1000：查询到票的信息，2001：没有查询到票的信息
        /// </summary>
        public string resultCode { get; set; }
        /// <summary>
        /// 201，210，220等 失败状态码，如果resultCode为1000，该字段不返回，如果resultCode为2001，会返回不同类型错误码          
        /// </summary>
        public string invoicefalseCode { get; set; }
        /// <summary>
        /// 查验结果成功 提示信息，resultCode为1000返回：查验结果成功，resultCode为2001返回对应invoicefalseCode的错误信息
        /// </summary>
        public string resultMsg { get; set; }
        /// <summary>
        /// 江苏增值税（专用发票） 发票名称 
        /// </summary>
        public string invoiceName { get; set; }
        /// <summary>
        /// 数据查询结果，详情请查看返回业务参数
        /// </summary>
        public InvoiceResult invoiceResult { get; set; }
    }
}
