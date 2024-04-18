using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Usually;

namespace Yahv.PsWms.DappForm.Services.Printers
{
    public class FileProcess
    {

        public event ErrorHanlder DownFailed;
        internal FileProcess()
        {
            this.DownFailed += FileProcess_DownFailed;
        }

        private void FileProcess_DownFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            SimHelper.TransferStatus = "下载失败";
        }

        string DownFile(string uri)
        {
            if (File.Exists(uri))
            {
                return uri;
            }

            DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
            //var filePath = ConfigurationManager.AppSettings["warehouseFile"];
            //FileInfo fi = new FileInfo(Path.Combine(filePath, Path.GetFileName(uri)));
            FileInfo fi = new FileInfo(Path.Combine(@"..\", di.Name + "\\Downloads", Path.GetFileName(uri)));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using (var webclient = new WebClient())
            {
                webclient.DownloadFileCompleted += Webclient_DownloadFileCompleted;
                webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Webclient_DownloadProgressChanged);
                webclient.DownloadFileAsync(new Uri(uri), fi.FullName);
                //返回打印的地址
                return fi.FullName;
            }
        }

        private void Webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //下载进度
            SimHelper.TransferStatus = "下载进度：" + e.ProgressPercentage + "%";
        }

        private void Webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //文件下载完成??如何获得文件名字
            SimHelper.TransferStatus = "下载完成";
        }


        /// <summary>
        /// 文件处理
        /// </summary>
        /// <param name="url"></param>
        public void Process(string url)
        {
            //因为加载了其它页面（PictureShow页面）并且显示了出来，所以不能用线程（用线程程序会报异常）
            //new Thread(() =>
            //{


            string fileName;

            try
            {
                fileName = this.DownFile(url);
            }
            catch (Exception)
            {
                //显示下载失败
                if (this != null && this.DownFailed != null)
                {
                    this.DownFailed(this, new Yahv.Usually.ErrorEventArgs("下载失败"));
                }
                return;
            }

            FileInfo info = new FileInfo(fileName);
            var extension = PrintHelper.Current.Extensions[info.Extension];
            switch (extension)
            {
                case FilePrintType.Image:
                    {
                        Controls.PictureShow.url = url;
                        Controls.PictureShow.Current.Show();
                    }
                    break;
                case FilePrintType.Word:
                    {
                        Controls.WordShow.docUrl = fileName;
                        Controls.WordShow.Current.Show();
                    }
                    break;
                case FilePrintType.Pdf:
                    {
                        //System.Windows.Forms.MessageBox.Show($"PDF文档暂不支持预览，请于本地{fileName} 查看该文件！！","提示",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Warning,System.Windows.Forms.MessageBoxDefaultButton.Button1,System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                        try
                        {
                            System.Diagnostics.Process.Start(fileName);
                        }
                        catch
                        {
                            System.Windows.Forms.MessageBox.Show($"需要先下载能够支持打开PDF文件的软件！！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                        }

                    }
                    //未实现
                    //throw new Exception("7");
                    //{
                    //    Controls.PDFShow.pdfFile = fileName;
                    //    Controls.PDFShow.Current.Show();
                    //}
                    break;
                default:
                    throw new NotSupportedException($"Type:{extension} are not supported!");

            }

            //}).Start();
        }

        static object locker = new object();

        static FileProcess current;
        static public FileProcess Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new FileProcess();
                        }
                    }
                }

                return current;
            }
        }

    }
}
