using Gecko;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using WinApp.Services;
using WinApp.Services.FilePrints;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace WinApp.Printers
{

    public class MyEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public PrinterConfig Config { get; set; }
    }

    public class FilePrinter : PrinterBase
    {
        /// <summary>
        /// 默认打印类型
        /// </summary>
        static readonly public string PrintPaperSize = "A4";
        public event ErrorHanlder DownFailed;
        public event EventHandler<MyEventArgs> DownCompleted;


        internal FilePrinter()
        {
            this.DownFailed += FilePrinter_DownFailed;
            this.DownCompleted += FilePrinter_DownCompleted;
        }

        /// <summary>
        /// 执行打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilePrinter_DownCompleted(object sender, MyEventArgs e)
        {
            new Thread(() =>
            {
                var config = e.Config;

                FileInfo info = new FileInfo(e.FileName);
                var extension = PrintHelper.Current.Extensions[info.Extension];
                switch (extension)
                {
                    case FilePrintType.Word:
                        {
                            FilePrintBase fpb = new WordApp(config.PrinterName);
                            fpb.Print(fileName);
                        }
                        break;
                    case FilePrintType.Pdf:
                        //准备使用  gf 模拟浏览器的方式进行打印
                        {
                            FilePrintBase fpb = new PdfApp(config.PrinterName);
                            fpb.Print(fileName);
                        }
                        break;
                    case FilePrintType.Image:
                        {
                            FilePrintBase fpb = new ImageApp(config.PrinterName);
                            fpb.Print(fileName);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Type:{extension} are not supported!");

                }
            }).Start();
        }



        /// <summary>
        /// 现在文件，并放到指定的位置准备打印
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>保存地址</returns>
        string DownFile(string uri)
        {
            if (File.Exists(uri))
            {
                return uri;
            }

            DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
            //FileInfo fi = new FileInfo(Path.Combine(@"..\", di.Name + "Downloads", VirtualPathUtility.GetFileName(uri)));

            //FileInfo fi = new FileInfo(Path.Combine(@"..\", di.Name + "\\Downloads", Path.GetFileName(uri)));
            string fileName = Path.GetFileNameWithoutExtension(uri).Length >= 24 ? Path.GetFileNameWithoutExtension(uri).Substring(0, 23) : Path.GetFileNameWithoutExtension(uri);
            FileInfo fi = new FileInfo(Path.Combine(@"..\", di.Name + "\\Downloads", fileName + Guid.NewGuid() + Path.GetExtension(uri)));

            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using (var webclient = new WebClient())
            {
                //webclient.DownloadFileCompleted += new AsyncCompletedEventHandler(Webclient_DownloadFileCompleted);
                webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Webclient_DownloadProgressChanged);

                //异步下载改成同步下载解决文件正由另一进程使用，因此该进程无法访问此文件
                webclient.DownloadFile(new Uri(uri), fi.FullName);

                //返回打印的地址
                return fi.FullName;
            }
        }


        private void Webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //下载进度
            SimHelper.TransferStatus = "下载进度：" + e.ProgressPercentage + "%";
        }

        ///// <summary>
        ///// 下载完成事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    //文件下载完成
        //    SimHelper.TransferStatus = "下载完成";

        //}

        string fileName;
        PrinterConfig config;


        public void Print(PrinterConfig config/*, object data = null*/)
        {
            //如果需要的话，判断 Url 的后缀。例如：docx、doc、xls、xlsx、txt使用某打印机进行打印。
            //目前打印只需要打印A4纸张，并且所有下载打印的非标签文件都应该使用A4打印。
            //选择配置好打印（高汇航）模块进行打印

            //如果是下载的客户自定义标签打印（这个需求还需要与朝旺联系看看如何限制？）


            //判断是否需要下载？（本地文件不需要下载）
            //地址是Uri的时候，才需要下载，否则应该直接返回  url

            //file://www.baidu.com/c:/dddssd/1.txt
            //本地文件

            new Thread(() =>
            {

                this.config = config;
                ////这里修改为线程方式

                try
                {
                    fileName = this.DownFile(config.Url);

                    if (this != null & this.DownCompleted != null)
                    {
                        this.DownCompleted(this, new MyEventArgs
                        {
                            FileName = fileName,
                            Config = config
                        });
                    }

                    SimHelper.TransferStatus = "下载完成";

                }
                catch (Exception ex)
                {
                    //显示下载失败
                    if (this != null && this.DownFailed != null)
                    {
                        this.DownFailed(this, new Yahv.Usually.ErrorEventArgs("下载失败"));
                    }
                    return;
                }

            }).Start();

        }

        private void FilePrinter_DownFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            SimHelper.TransferStatus = "下载失败";
        }
    }

    public class Images
    {
        public string Src { get; set; }

    }
}
