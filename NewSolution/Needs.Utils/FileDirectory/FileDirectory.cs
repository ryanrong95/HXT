using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils
{
    /// <summary>
    /// 项目文件目录的获取与设置
    /// 创建按日期及子文件夹的文件目录，返回文件的虚拟路径
    /// 获取文件的物理路径
    /// </summary>
    public sealed class FileDirectory
    {
        /// <summary>
        /// 获取文件的全路径
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// 程序路径
        /// </summary>
        private string BaseDirectory;

        /// <summary>
        /// 项目文件根目录
        /// </summary>
        private readonly string RootFileFolder = "Files";

        /// <summary>
        /// 子文件夹
        /// </summary>
        private string ChildFolder;

        /// <summary>
        /// 文件名
        /// </summary>
        private string FileName;

        /// <summary>
        /// 获取项目域名地址
        /// </summary>
        public string FileServerUrl
        {
            get;
            private set;
        }

        public string FileServerUrl1
        {
            get;
            private set;
        }
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
        /// 如：D:\WebApp\Files
        /// </summary>
        public string RootDirectory
        {
            get;
            private set;
        }

        /// <summary>
        /// 中心读取URL
        /// </summary>
        public string PvDataFileUrl
        {
            get;
            private set;

        }
        /// <summary>
        /// 中心读取URL
        /// </summary>
        public string IsChainsDate
        {
            get;
            private set;

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileDirectory()
        {
            this.FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            this.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.RootDirectory = this.BaseDirectory + this.RootFileFolder;
            this.FilePath = this.BaseDirectory + this.RootFileFolder;
            this.PvDataFileUrl= System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
            this.IsChainsDate = System.Configuration.ConfigurationManager.AppSettings["IsChainsDate"];
        }

        /// <summary>
        /// 返回当前实例
        /// </summary>
        /// <param name="fileName">文件名</param>
        public FileDirectory(string fileName) : this()
        {
            this.FileName = fileName;
        }

        public static FileDirectory Current
        {
            get
            {
                return new FileDirectory();
            }
        }

        /// <summary>
        /// 设置子文件夹
        /// </summary>
        /// <param name="subDirectory"></param>
        public void SetChildFolder(string subDirectory)
        {
            if (string.IsNullOrEmpty(subDirectory))
            {
                throw new Exception("不可创建名称为空的文件夹");
            }
            this.ChildFolder = subDirectory;
            this.VirtualPath += subDirectory;
            this.FilePath += @"\" + subDirectory;
        }
        
        /// <summary>
        /// 在Api站点，新建App站点的文件夹
        /// </summary>
        /// <param name="subDirectory"></param>
        public void SetFilePathFormDifferentSite(string subDirectory)
        {
            this.FilePath = subDirectory;
        }

        /// <summary>
        /// 按日期创建文件夹
        /// 格式：201902/01，201902/28
        /// </summary>
        public void CreateDataDirectory()
        {
            //创建文件夹
            string date = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.Day.ToString().PadLeft(2, '0');

            string dataPath = @"\" + date + @"\" + day + @"\";
            this.FilePath += dataPath + this.FileName;
            this.VirtualPath += dataPath + this.FileName;

            System.IO.FileInfo last = new System.IO.FileInfo(this.FilePath);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }
        }

        /// <summary>
        /// 获取文件Url地址
        /// </summary>
        /// <returns></returns>
        public string FileUrl
        {
            get
            {
                return this.FileServerUrl + "/" + this.VirtualPath.Replace(@"\", "/"); 
            }
        }
    }
}
