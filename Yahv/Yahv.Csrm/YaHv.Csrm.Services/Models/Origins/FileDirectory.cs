//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using Yahv.Underly;

//namespace YaHv.Csrm.Services.Models.Origins
//{
//    public sealed class FileDirectory
//    {
//        //默认本地下载路径
//        public readonly string DownLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\DownLoad\";
//        //默认本地上传路径
//        public readonly string UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\UpLoad\";

//        readonly string PathRoot = string.Empty;
//#if DEBUG
//        //默认服务器路径
//        public static string ServiceRoot = @"http://uuws.b1b.com/";
//#elif TEST
//        //默认服务器路径
//        public static string ServiceRoot = @"http://uuws.b1b.com/";
//#else
//        public static string ServiceRoot = @"http://uuws.ic360.cn/";
//#endif

//        //文件名称（带后缀）
//        private string fileName;
//        public string FileName { get { return this.fileName; } }

//        //文件类型
//        private FileType fileType;
//        public FileType FileType { get { return this.fileType; } }

//        /// <summary>
//        /// 中心返回结果
//        /// </summary>
//        public Yahv.Services.Models.UploadResult uploadResult { get; set; }

//        /// <summary>
//        /// 上传人
//        /// </summary>
//        public string AdminID { get; set; }

//        public string ClientID { get; set; }

//        public FileDirectory()
//        {
//        }
//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="fileName"></param>
//        public FileDirectory(string fileName, FileType fileType)
//        {
//            this.fileName = fileName;
//            this.fileType = fileType;
//            if (this.fileType == FileType.BusinessLicense)
//            {
//                this.PathRoot = @"Files/UpLoad/BusinessLicense/";
//                this.UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + this.PathRoot;
//            }

//        }

//        /// <summary>
//        /// 上传中心
//        /// </summary>
//        /// <param name="file"></param>
//        public void Save(HttpPostedFile file)
//        {
//            CreateDirectory();
//            string filePath = this.UpLoadRoot + this.fileName;
//            ////上传本地
//            file.SaveAs(filePath);
//            //上传中心
//            var result = Yahv.Services.Views.CenterFilesTopView.Upload(filePath, fileType,
//                new
//                {
//                    CustomName = fileName,
//                    AdminID = AdminID,
//                    ClientID = this.ClientID,
//                });
//            uploadResult = result.FirstOrDefault();
//            //删除本地文件
//            File.Delete(filePath);
//        }

//        /// <summary>
//        /// 上传到中心
//        /// </summary>
//        /// <param name="file"></param>
//        /// <param name="dic"></param>
//        public void Save(HttpPostedFile file, object dic)
//        {
//            CreateDirectory();
//            string filePath = this.UpLoadRoot + this.fileName;
//            //上传本地
//            file.SaveAs(filePath);
//            //上传中心
//            var result = Yahv.Services.Views.CenterFilesTopView.Upload(filePath, fileType, dic);
//            uploadResult = result.FirstOrDefault();
//            //删除本地文件
//            File.Delete(filePath);
//        }
//        public void Save(string url, object dic)
//        {
//            string filePath = this.UpLoadRoot + this.fileName;
//            //上传中心
//            var result = Yahv.Services.Views.CenterFilesTopView.Upload(filePath, fileType, dic);
//            uploadResult = result.FirstOrDefault();
//            //删除本地文件
//            File.Delete(url);
//        }
//        /// <summary>
//        /// 上传本地
//        /// </summary>
//        /// <param name="file">文件</param>
//        /// <returns>本地路径</returns>
//        public string SaveLocal(System.Web.HttpPostedFile file)
//        {
//            CreateDirectory();
//            DeleteFolder(this.UpLoadRoot);
//            string filePath = this.UpLoadRoot + this.fileName;
//            //上传本地
//            file.SaveAs(filePath);

//            return $"../../{PathRoot}/{fileName}";
//        }

//        /// <summary>
//        /// 创建文件目录
//        /// </summary>
//        public void CreateDirectory()
//        {
//            //上传目录
//            FileInfo uploadDirectory = new FileInfo(this.UpLoadRoot);
//            if (!uploadDirectory.Directory.Exists)
//            {
//                uploadDirectory.Directory.Create();
//            }
//            //下载目录
//            FileInfo downDirectory = new FileInfo(this.DownLoadRoot);
//            if (!downDirectory.Directory.Exists)
//            {
//                downDirectory.Directory.Create();
//            }
//        }
//        /// <summary>
//        /// 创建文件目录
//        /// </summary>
//        public object Delete(string[] fileids)
//        {
//            if (fileids.Length > 0)
//            {
//                var files = new Yahv.Services.Views.CenterFilesTopView();
//                files.Delete(fileids);
//            }
//            return new { success = false };
//        }
//        void DeleteFolder(string dir)
//        {
//            foreach (string d in Directory.GetFileSystemEntries(dir))
//            {
//                if (File.Exists(d))
//                {
//                    FileInfo fi = new FileInfo(d);
//                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
//                        fi.Attributes = FileAttributes.Normal;
//                    File.Delete(d);//直接删除其中的文件  
//                }
//                else
//                {
//                    DirectoryInfo d1 = new DirectoryInfo(d);
//                    if (d1.GetFiles().Length != 0)
//                    {
//                        DeleteFolder(d1.FullName);////递归删除子文件夹
//                    }
//                    Directory.Delete(d);
//                }
//            }
//        }
//    }

//}

