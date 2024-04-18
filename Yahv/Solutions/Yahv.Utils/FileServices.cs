using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Yahv.Utils
{
    public static class FileServices
    {
        static Dictionary<string, string> fileTypes =
            new Dictionary<string, string> {
                { "data:text/plain;base64", ".txt" },
                { "data:application/msword;base64",".doc"},
                { "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64",".docx"},
                { "data:application/vnd.ms-excel;base64",".xls"},
                { "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64",".xlsx"},
                { "data:application/pdf;base64",".pdf"},
                { "data:application/vnd.openxmlformats-officedocument.presentationml.presentation;base64",".pptx"},
                { "data:application/vnd.ms-powerpoint;base64",".ppt"},
                { "data:image/png;base64",".png"},
                { "data:image/jpeg;base64",".jpg"},
                { "data:image/gif;base64",".gif"},
                { "data:image/svg+xml;base64",".svg"},
                { "data:image/x-icon;base64",".ico"},
                { "data:image/bmp;base64",".bmp"}
            };



        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64file">文件的base64位编码</param>
        /// <param name="subdir">自定义路径</param>
        /// <returns></returns>
        public static string Save(string base64file=null, string subdir = "")
        {
            var rootPath = System.Configuration.ConfigurationManager.AppSettings["FileUpLoadRootPath"];
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new DirectoryNotFoundException("请配置AppSettings(*.config中)文件上传的物理根目录（key=\"FileUpLoadRootPath\"）！");
            }


            var rootUrl = System.Configuration.ConfigurationManager.AppSettings["FileUpLoadRootUrl"];
            if (string.IsNullOrEmpty(rootUrl))
            {
                throw new DirectoryNotFoundException("请配置AppSettings(*.config中)文件上传的虚拟根目录（key=\"FileUpLoadRootUrl\"）！");
            }

            if (string.IsNullOrEmpty(subdir))
            {
                subdir = DateTime.Now.ToString("yyyyMMdd");
            }

            var dirurl = string.Join("/", rootUrl.TrimEnd('/'), subdir.Trim('/'));

            var dirpath = string.Join("\\", rootPath.TrimEnd('\\'), subdir.Trim('\\'));

            if (System.Web.HttpContext.Current != null)
            {
                dirpath = System.Web.HttpContext.Current.Server.MapPath(dirurl);
            }


            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            if (string.IsNullOrEmpty(base64file))
            {
                return SavePostFile(dirpath, dirurl);               
            }
            else
            {
                return SaveBase64(base64file, dirpath, dirurl);
            }
        }


        private static string SavePostFile(string dirpath, string dirurl)
        {
            string url = null;
            foreach (string f in HttpContext.Current.Request.Files.AllKeys)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[f];
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                var fileName = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
                var filepath = string.Concat(dirpath, "\\", fileName, fileExtension);
                url = string.Concat(dirurl, "/", fileName, fileExtension);
                file.SaveAs(filepath);
            }


            //foreach ( var item in System.Web.HttpContext.Current.Request.Files)
            //{
            //    var file = (HttpPostedFile)item;
            //    var fileExtension = System.IO.Path.GetExtension(file.FileName);
            //    var fileName = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            //    var filepath = string.Concat(dirpath, "\\", fileName, fileExtension);
            //    url = string.Concat(dirurl, "/", fileName, fileExtension);
            //    file.SaveAs(filepath);

            //}

            return url;
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="base64string">base64编码的文件字符串</param>
        /// <returns>文件的url地址</returns>
        private static string SaveBase64(string base64string, string dirpath,string dirurl)
        {
            string url = null;
            try
            {
                


                if (!string.IsNullOrEmpty(base64string))
                {
                    var splits = base64string.Split(',');
                    if (splits.Length != 2)
                    {
                        throw new ArgumentException("传入的参数有误！");
                    }

                    if (!fileTypes.ContainsKey(splits[0]))
                    {
                        throw new NotSupportedException("不支持的文件类型！");
                    }

                    var fileExtension = fileTypes[splits[0]];
                    var fileName = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
                    var filepath = string.Concat(dirpath, "\\", fileName, fileExtension);
                    url = string.Concat(dirurl, "/", fileName, fileExtension);
                    using (var stream = new FileStream(filepath, FileMode.Append))
                    {
                        var bytes = Convert.FromBase64String(splits[1]);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close(); 
                      
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return url;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="url"></param>
        public static void Delete(string url)
        {

        }

        public static string FileTypes
        {
            get
            {
                return string.Join(",", fileTypes.Select(item => item.Value));
            }
        }

    }
}
