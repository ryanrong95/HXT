using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinApp.Printers;
using WinApp.Services.FilePrints;

namespace WinApp.Services
{
    /// <summary>
    /// 打印帮助者
    /// </summary>
    public class PrintHelper
    {
        internal Extensions Extensions { get; private set; }

        PrintHelper()
        {
            this.Extensions = new Extensions();
        }

        FilePrinter file;

        public FilePrinter File
        {
            get
            {
                if (this.file == null)
                {
                    this.file = new FilePrinter();
                }
                return this.file;
            }
        }

        public TemplatePrinter Template
        {
            get
            {
                return new TemplatePrinter();
            }
        }

        static PrintHelper current;
        static object locker = new object();

        /// <summary>
        /// 当期系统实例
        /// </summary>
        static public PrintHelper Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PrintHelper();
                        }
                    }
                }

                return current;
            }
        }
    }
}
