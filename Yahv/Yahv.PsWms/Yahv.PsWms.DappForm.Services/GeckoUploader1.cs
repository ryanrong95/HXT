using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.Usually;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services
{
    class GeckoUploader1
    {
        PhotoMap map;

        public event SuccessHanlder Success;

        public string FileName { get; private set; }
        public string[] FileNames { get; private set; }

        public GeckoUploader1(PhotoMap map, string fileName)
        {
            this.map = map;
            this.FileName = fileName;

        }


        public GeckoUploader1(PhotoMap map, string[] fileNames)
        {
            this.map = map;
            this.FileNames = fileNames;
        }

        List<FileResult> fileResults = new List<FileResult>();


        object obj = new object();

        public void Upload(string sender = null)
        {

            //未实现
            //throw new Exception("8");

            string address = Config.ApiUrlPrex + "/Files/Upload";
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

            if (string.IsNullOrWhiteSpace(FileName) && this.FileNames != null)
            {
                string[] fileNames = this.FileNames;

                foreach (var fileName in fileNames)
                {
                    lock (obj)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.UploadProgressChanged += Client_UploadProgressChanged;
                            client.UploadFileCompleted += Client_UploadFileCompleted;

                            client.UploadFileAsync(new Uri(address), "POST", fileName);
                        }

                    }
                }

                Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;
                if (firefox == null)
                {
                    return;
                }
                using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
                {
                    string result;
                    context.EvaluateScript($"this['PhotoUploaded']({fileResults.Json()});", firefox.Window.DomWindow, out result);
                }
            }
            else if (!string.IsNullOrWhiteSpace(FileName) && this.FileNames == null)
            {
                string fileName = this.FileName;

                using (WebClient client = new WebClient())
                {

                    client.UploadProgressChanged += Client_UploadProgressChanged;
                    client.UploadFileCompleted += Client_UploadFileCompleted;

                    client.UploadFileAsync(new Uri(address), "POST", fileName);
                }

                Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;
                if (firefox == null)
                {
                    return;
                }
                using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
                {
                    string result;
                    context.EvaluateScript($"this['PhotoUploaded']({fileResults.Json()});", firefox.Window.DomWindow, out result);
                }
            }
            else
            {
                MessageBox.Show("图片拍照和上传出现错误！！");
                return;
            }


        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            SimHelper.TransferStatus =
                $"正在上传照片：{this.FileNames},{(e.BytesSent / 1024m).ToString("0.000")}/{(e.TotalBytesToSend / 1024m).ToString("0.000")}";
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {

            try
            {


                if (e.Error != null)
                {
                    MessageBox.Show("图片服务器可能有错误：" + e.Error.Message);
                    return;
                }

                SimHelper.TransferStatus = $"完成上传照片：{this.FileNames}";
                var bytes = e.Result;

                Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;

                if (firefox == null)
                {
                    return;
                }

                string message = Encoding.UTF8.GetString(bytes);
                JObject obj = message.JsonTo<JObject>();
                var data = obj["data"].ToArray();
                FileResult fileResult = new FileResult
                {
                    Url = data[0]["Url"].Value<string>(),
                    CustomName = data[0]["CustomName"].Value<string>(),
                    ID = data[0]["FileID"].Value<string>(),
                };


                if (this != null && this.Success != null)
                {
                    this.Success(this, new SuccessEventArgs(fileResult));
                }

                fileResults.Add(fileResult);


                using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
                {
                    string result;
                    context.EvaluateScript($"this['PhotoUploaded']({message});", firefox.Window.DomWindow, out result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
