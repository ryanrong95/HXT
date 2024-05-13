using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services.FilePrints
{
    /// <summary>
    /// 打印基类
    /// </summary>
    abstract public class FilePrintBase
    {
        /// <summary>
        /// 打印机
        /// </summary>
        public string Printer { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="printer">打印机名</param>
        internal FilePrintBase(string printer)
        {
            this.Printer = printer;
        }

        /// <summary>
        /// 打印指定的文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        abstract public void Print(string fileName);
    }
}
