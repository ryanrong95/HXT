using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class InvoiceFile
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public  string FileID { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public int FileType { get; set; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 文件状态
        /// </summary>
        public int Status { get; set; }
    }
}
