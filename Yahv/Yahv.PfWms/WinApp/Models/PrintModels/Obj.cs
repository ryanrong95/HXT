using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class Obj
    {
        public string ID { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 文件地址(不为空则证明是文档打印)
        /// </summary>
        public string FileUrl { get; set; }
        //public Wms.Services.PrintingType Type { get; set; }

        public object data { get; set; }

        public Size size { get; set; }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
