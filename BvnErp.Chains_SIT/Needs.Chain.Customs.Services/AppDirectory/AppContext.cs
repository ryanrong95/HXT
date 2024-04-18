using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    /// <summary>
    /// 即将作废，
    /// 请使用Needs.Utils下的 FileDirectory 类
    /// </summary>
    [Obsolete]
    public sealed class AppContext
    {
        static object locker = new object();
        static AppContext current;

        /// <summary>
        /// 程序路径
        /// </summary>
        public string BaseDirectory;

        /// <summary>
        /// 项目文件目录
        /// </summary>
        public string FileDirectory;

        /// <summary>
        /// 项目域名地址
        /// </summary>
        public string DomainUrl;

        private AppContext()
        {
            //系统文件目录，不需要在webconfig中配置。
            this.DomainUrl = System.Configuration.ConfigurationManager.AppSettings["netfilebaseurl"];
            this.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.FileDirectory = this.BaseDirectory + SysConfig.FileDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        public static AppContext Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new AppContext();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 按日期创建文件夹
        /// 格式：201902/01，201902/28
        /// </summary>
        [Obsolete]
        public string CreateDataFileDirectory(string fileFolder)
        {
            //创建文件夹
            string data = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.Day.ToString().PadLeft(2, '0');

            string dataPath = @"\" + data + @"\" + day + @"\";
            string fileDirectory = this.FileDirectory + dataPath;
            System.IO.FileInfo last = new System.IO.FileInfo(fileDirectory);
            
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }  

            return fileFolder + @"\" + data + @"\" + day;
        }
    }
}
