using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Common
{
    public sealed class FileDirectory
    {
        //默认本地下载路径
        public readonly string DownLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\DownLoad\";
        //默认本地上传路径
        public readonly string UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\UpLoad\";
        ////默认服务器路径
        //public static string ServiceRoot = @"http://uuws.b1b.com/";
#if DEBUG
        //默认服务器路径
        public static string ServiceRoot = @"http://uuws.b1b.com/";
#elif TEST
        //默认服务器路径
        public static string ServiceRoot = @"http://uuws.b1b.com/";
#else
        public static string ServiceRoot = @"http://uuws.for-ic.net/";
#endif

        //文件名称（带后缀）
        private string fileName;
        public string FileName { get { return this.fileName; } }
        
        //文件类型
        private Underly.FileType fileType;
        public Underly.FileType FileType { get { return this.fileType; } }

        /// <summary>
        /// 中心返回结果
        /// </summary>
        public Yahv.Services.Models.UploadResult uploadResult { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName"></param>
        public FileDirectory(string fileName, Underly.FileType fileType)
        {
            this.fileName = fileName;
            this.fileType = fileType;
        }

        /// <summary>
        /// 上传中心
        /// </summary>
        /// <param name="file"></param>
        public void Save(System.Web.HttpPostedFile file)
        {
            CreateDirectory();
            string filePath = this.UpLoadRoot + this.fileName;
            //上传本地
            file.SaveAs(filePath);
            //上传中心
            var result = Yahv.Services.Views.CenterFilesTopView.Upload(filePath, fileType, new { CustomName = fileName, AdminID = AdminID });
            uploadResult = result.FirstOrDefault();
            //删除本地文件
            File.Delete(filePath);
        }

        /// <summary>
        /// 上传到中心
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dic"></param>
        public void Save(System.Web.HttpPostedFile file, object dic)
        {
            CreateDirectory();
            string filePath = this.UpLoadRoot + this.fileName;
            //上传本地
            file.SaveAs(filePath);
            //上传中心
            var result = Yahv.Services.Views.CenterFilesTopView.Upload(filePath, fileType, dic);
            uploadResult = result.FirstOrDefault();
            //删除本地文件
            File.Delete(filePath);
        }

        /// <summary>
        /// 上传本地
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns>本地路径</returns>
        public string SaveLocal(System.Web.HttpPostedFile file)
        {
            CreateDirectory();
            string filePath = this.UpLoadRoot + this.fileName;
            //上传本地
            file.SaveAs(filePath);

            return filePath;
        }

        /// <summary>
        /// 创建文件目录
        /// </summary>
        public void CreateDirectory()
        {
            //上传目录
            FileInfo uploadDirectory = new FileInfo(this.UpLoadRoot);
            if (!uploadDirectory.Directory.Exists)
            {
                uploadDirectory.Directory.Create();
            }
            //下载目录
            FileInfo downDirectory = new FileInfo(this.DownLoadRoot);
            if (!downDirectory.Directory.Exists)
            {
                downDirectory.Directory.Create();
            }
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        private string ReName()
        {
            int index = fileName.LastIndexOf('.');
            string name = fileName.Substring(0, index);
            string exeName = fileName.Substring(index, fileName.Length);
            return name + "_" + Guid.NewGuid() + exeName;
        }
    }
}
