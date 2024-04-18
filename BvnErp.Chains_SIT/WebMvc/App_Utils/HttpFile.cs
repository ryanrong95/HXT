using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    /// <summary>
    /// 使用Http上传文件，指定文件的名称
    /// 实例化时获取系统文件的根目录，设置上传的子文件夹及生成日期文件夹
    /// 返回上传后的文件物理路径、虚拟路径、文件大小、Url
    /// </summary>
    public class HttpFile
    {
        /// <summary>
        /// 上传的文件的文件名称
        /// </summary>
        private string FileName;

        /// <summary>
        /// 子文件夹
        /// </summary>
        private string ChildFolder;

        /// <summary>
        /// 文件路径
        /// 全文件路径
        /// 如：D:/File/2018/01/8208861122.png
        /// </summary>
        private string FilePath;

        /// <summary>
        /// 获取文件的虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 项目文件根目录
        /// </summary>
        private readonly string RootFileFolder = "Files";

        /// <summary>
        /// 获取项目域名地址
        /// </summary>
        public string DomainUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取文件服务器Url
        /// </summary>
        public string FileServerUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        /// <param name="fileName"></param>
        public HttpFile(string fileName)
        {
            this.ChildFolder = "";
            this.FileName = fileName;
            this.FilePath = AppDomain.CurrentDomain.BaseDirectory + this.RootFileFolder;
            this.DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
            this.FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
        }

        /// <summary>
        /// 设置子文件夹
        /// </summary>
        /// <param name="childFolder"></param>
        public void SetChildFolder(string childFolder)
        {
            if (string.IsNullOrEmpty(childFolder))
            {
                throw new Exception("不可创建名称为空的文件夹");
            }
            this.ChildFolder = childFolder;
            this.FilePath += @"\" + childFolder;
            this.VirtualPath = this.ChildFolder;
        }

        /// <summary>
        /// 按日期创建文件夹
        /// 格式：201902/01，201902/28
        /// </summary>
        /// <param name="subDirectory">子文件目录</param>
        /// <returns></returns>
        public void CreateDataDirectory()
        {
            //创建文件夹
            string data = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);

            string dataPath = @"\" + data + @"\" + day;
            this.VirtualPath += dataPath;
            this.FilePath += dataPath;
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="httpPostedFile">上传的文件</param>
        /// <returns>[0]文件的物理路径 [1]文件虚拟路径 例如：Orders/2019/01/89987772.pdf [2]文件的网络url [3]文件的大小</returns>
        public string[] SaveAs(HttpPostedFileBase httpPostedFile)
        {
            this.VirtualPath += @"\" + this.FileName;
            string fullName = this.FilePath + @"\" + this.FileName;
            string url = this.FileServerUrl + "/" + this.VirtualPath.Replace(@"\", "/");
            FileInfo last = new FileInfo(fullName);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }

            httpPostedFile.SaveAs(fullName);
            return new[] { fullName, this.VirtualPath, url, last.Length.ToString() };
        }
    }
}