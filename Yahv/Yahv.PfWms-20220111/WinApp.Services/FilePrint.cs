using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinApp.Services.FilePrints;

namespace WinApp.Services
{


    public class FilePrint
    {
        /// <summary>
        /// 默认打印类型
        /// </summary>
        static public string PrintPaperSize
        {
            get { return "A4"; }
        }
        ConcurrentDictionary<string, FilePrint> concurrent;
        Extensions extension;

        FilePrint()
        {
            this.concurrent = new ConcurrentDictionary<string, FilePrint>();
            this.extension = new Extensions();
        }

        FilePrint(string printer)
        {
            this.Printer = printer;
            this.extension = new Extensions();
        }

        /// <summary>
        /// 打印机
        /// </summary>
        public string Printer { get; private set; }

        /// <summary>
        /// 返回指定的打印者
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FilePrint this[string index]
        {
            get
            {
                return this.concurrent.GetOrAdd(index, (printer) =>
                {
                    return new FilePrint(printer);
                });
            }
        }

        /// <summary>
        /// 现在文件，并放到指定的位置准备打印
        /// </summary>
        /// <param name="url"></param>
        /// <returns>保存地址</returns>
        private string DownFile(string url)
        {
            var path=  AppContext.BaseDirectory + "\\file";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileExtension = Path.GetExtension(url);
            var fileName= string.Concat( path,"\\", BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0),fileExtension);
            using (var webclient=new WebClient())
            {
                webclient.DownloadFile(url, fileName);
            }
            
            return fileName;//返回打印的地址
        }

        /// <summary>
        /// 打印文件（可能包含标签文件）
        /// </summary>
        /// <param name="info">文件信息</param>
        /// <remarks>乔霞点击打印按钮后触发</remarks>
        public void PrintFile(FileDescriptor info)
        {
            string fileName = this.DownFile(info.Url);
            Print(fileName);
            //如果需要的话，判断 Url 的后缀。例如：docx、doc、xls、xlsx、txt使用某打印机进行打印。
            //目前打印只需要打印A4纸张，并且所有下载打印的非标签文件都应该使用A4打印。
            //选择配置好打印（高汇航）模块进行打印
            //如果是下载的客户自定义标签打印（这个需求还需要与朝旺联系看看如何限制？）
        }

        /// <summary>
        /// 文件打印
        /// </summary>
        /// <param name="fileName"></param>
        public void Print(string fileName)
        {
            FileInfo info = new FileInfo(fileName);

            FilePrintBase fpb = null;

            switch (this.extension[info.Extension])
            {
                case FilePrintType.Word:
                    {
                        fpb = new WordApp(this.Printer);
                    }
                    break;
                //暂不支持Excel和PDF
                //case FilePrintType.Excel:
                //    {
                //        fpb = new ExcelApp(this.Printer);
                //    }
                //    break;
                case FilePrintType.Pdf:
                    //准备使用  gf 模拟浏览器的方式进行打印
                    {
                        fpb = new PdfApp(this.Printer);
                    }
                    break;

                default:
                    throw new NotSupportedException($"Type:{FilePrintType.None} are not supported!");

            }
            fpb.Print(fileName);
        }

        static FilePrint current;
        static object locker = new object();
        static public FilePrint Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new FilePrint();
                        }
                    }
                }

                return current;
            }
        }
    }
}
