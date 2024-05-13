using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services;
using Yahv.Utils.Extends;

namespace WinApp.Services
{
    class GeckoUploader
    {
        PhotoMaps maps;

        PhotoMap map;
        public string FileName { get; private set; }

        public GeckoUploader(PhotoMap map, string fileName)
        {
            this.map = map;
            this.FileName = fileName;
        }


        public GeckoUploader(PhotoMaps maps, string fileName)
        {
            this.maps = maps;
            this.FileName = fileName;
        }



        public void Upload(string sender = null)
        {

            //未实现
            //throw new Exception("8");

            string address = Config.UploadUrl;
            if (map != null)
            {
                if (string.IsNullOrWhiteSpace(sender))
                {
                    address = address + "?" + map.GetQueryParams();
                }
                else
                {
                    var dic = map.GetQueryDictionary();
                    dic["sender"] = sender;
                    address = address + "?" + dic.GetQueryParams();
                }
            }

            string fileName = this.FileName;

            using (WebClient client = new WebClient())
            {

                client.UploadProgressChanged += Client_UploadProgressChanged;
                client.UploadFileCompleted += Client_UploadFileCompleted;

                client.Headers.Add("Content-Type", "application/form-data");
                client.UploadFileAsync(new Uri(address), "POST", fileName);
            }
        }

        object obj = new object();
        public void Uploads(string sender = null)
        {
            //lock (obj)
            //{

            string address = Config.UploadUrl; 
            if (maps != null)
            {
                string fileName = this.FileName;
                if (string.IsNullOrWhiteSpace(sender))
                {
                    foreach (var map in maps.PhotoMapes)
                    {
                        address = address + "?" + map.GetQueryParams();
                        using (WebClient client = new WebClient())
                        {
                            client.UploadProgressChanged += Client_UploadProgressChanged;
                            client.UploadFileCompleted += Client_UploadFileCompleted;

                            client.Headers.Add("Content-Type", "application/form-data");
                            client.UploadFileAsync(new Uri(address), "POST", fileName);
                            address = Config.UploadUrl; 
                        }
                    }
                }
                else
                {
                    foreach (var map in maps.PhotoMapes)
                    {
                        var dic = map.GetQueryDictionary();
                        dic["sender"] = sender;
                        address = address + "?" + dic.GetQueryParams();
                        using (WebClient client = new WebClient())
                        {
                            client.UploadProgressChanged += Client_UploadProgressChanged;
                            client.UploadFileCompleted += Client_UploadFileCompleted;

                            client.Headers.Add("Content-Type", "application/form-data");
                            client.UploadFileAsync(new Uri(address), "POST", fileName);
                            address = Config.UploadUrl; 
                        }
                    }

                }
            }
            //}
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            SimHelper.TransferStatus =
                $"正在上传照片：{this.FileName},{(e.BytesSent / 1024m).ToString("0.000")}/{(e.TotalBytesToSend / 1024m).ToString("0.000")}";
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show("图片服务器可能有错误：" + e.Error.Message);
                return;
            }

            SimHelper.TransferStatus = $"完成上传照片：{this.FileName}";
            var bytes = e.Result;
            string message = Encoding.UTF8.GetString(bytes);

            Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;

            if (firefox == null)
            {
                return;
            }

            using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
            {
                string result;
                context.EvaluateScript($"this['PhotoUploaded']({message});", firefox.Window.DomWindow, out result);
            }
        }
    }
}
