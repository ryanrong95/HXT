using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Common
{
    public class FileDirectory
    {
        //默认本地下载路径
        public static string DownLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\DownLoad\";
        //默认本地上传路径
        public static string UpLoadRoot = AppDomain.CurrentDomain.BaseDirectory + @"Files\UpLoad\";

        /// <summary>
        /// 创建文件目录
        /// </summary>
        public static void CreateDirectory()
        {
            //上传目录
            FileInfo uploadDirectory = new FileInfo(UpLoadRoot);
            if (!uploadDirectory.Directory.Exists)
            {
                uploadDirectory.Directory.Create();
            }
            //下载目录
            FileInfo downDirectory = new FileInfo(DownLoadRoot);
            if (!downDirectory.Directory.Exists)
            {
                downDirectory.Directory.Create();
            }
        }
    }
}
