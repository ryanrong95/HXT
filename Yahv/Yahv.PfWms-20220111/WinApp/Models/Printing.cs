using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class Printing
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeDes { get; set; }
        /// <summary>
        /// 模板地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }


        public string Printer { get; set; }

        public int Status { get; set; }

        public string StatusDes { get; set; }

    }
}
