using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 文件服务
    /// </summary>
    public class FileService
    {
        public void DownLoad()
        {
            //更新文件的路径
            using (WebClient wc = new WebClient())
            {
                //服务器地址
                string server = System.Configuration.ConfigurationManager.AppSettings["Server"];
                string relativePath = "wladmin/Files/Warehouse/202001/16";
                string name = "0438398562047.jpg";
                string url = $"{server}/{relativePath}/{name}";

                //本地保存路径
                string savePath = System.Configuration.ConfigurationManager.AppSettings["SavePath"];
                string path = System.IO.Path.Combine(savePath, relativePath.Replace("/", "\\"));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = System.IO.Path.Combine(savePath, relativePath.Replace("/", "\\"), name);
                wc.DownloadFile(url, fileName);
            }
        }

        public void Upload()
        {

        }
    }
}
