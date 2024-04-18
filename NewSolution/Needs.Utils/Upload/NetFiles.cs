using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils.Upload
{
    /// <summary>
    /// 上传文件类型
    /// </summary>
    public enum NetFilesType
    {
        /// <summary>
        /// 采购的invoice上传
        /// </summary>
        [Description("发票上传")]
        invoice=1,

        /// <summary>
        /// 订单的合同上传
        /// </summary>
        [Description("合同上传")]
        Orders = 2,

        /// <summary>
        /// 打款凭证的上传
        /// </summary>
        [Description("打款凭证上传")]
        Money = 3,

    }

    /// <summary>
    /// 上传文件操作类
    /// 请在配置文件中配置 System.Configuration.ConfigurationManager.AppSettings["erpuploadfile"]
    /// 不配置则为运行环境中本地根目录下的root更路径下的erpuploadfile文件夹内
    /// </summary>
    public class NetFiles
    {
        const string rootName = "erpuploadfile";
        string key;
        string fileName;
        NetFilesType type;
        /// <summary>
        /// 补充参数
        /// </summary>
        string[] paramArry = new string[0];

        /// <summary>
        /// 构造函数,指定上传的文件名
        /// </summary>
        /// <param name="type">存储类型</param>
        /// <param name="fileName">上传的文件名称</param>
        public NetFiles(string fileName, NetFilesType type)
        {
            this.type = type;
            this.fileName = fileName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">（UserID）</param>
        /// <param name="type">存储类型</param>
        /// <param name="fileName">上传的文件名称</param>
        public NetFiles(string key, string fileName, NetFilesType type)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                this.key = Guid.NewGuid().ToString("N");

            }
            else
            {
                this.key = key;
            }
            this.type = type;
            FileInfo fi = new FileInfo(fileName);
            this.fileName = string.Concat(Guid.NewGuid().ToString("N"), fi.Extension);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">（UserID）</param>
        /// <param name="type">存储类型</param>
        /// <param name="fileName">上传的文件名称</param>
        /// <param name="arry">可以用于补充文件（如订单ID）</param>
        public NetFiles(string key, string fileName, NetFilesType type, params string[] arry)
            : this(key, fileName, type)
        {
            this.paramArry = arry;
        }

        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        /// <param name="type">网络文件类型</param>
        public static string GetFolderPath(NetFileType type)
        {
            return Path.Combine(GetRoot(), Enum.GetName(typeof(NetFileType), type)).ToLower();
        }
        /// <summary>
        /// 获取文件保存文件夹路径
        /// </summary>
        /// <param name="key">（UserID）</param>
        /// <param name="type">网络文件类型</param>
        /// <returns></returns>
        public static string GetFolderPath(string key, NetFileType type)
        {
            return Path.Combine(GetRoot(), key, Enum.GetName(typeof(NetFileType), type)).ToLower();
        }

        public string GetFileFullName()
        {
            string root = GetRoot();
            if (this.key != null)
            {
                return Path.Combine(root, this.key, Enum.GetName(typeof(NetFileType), this.type)
                , string.Join(@"\", this.paramArry), this.fileName).ToLower();
            }
            else
            {
                return Path.Combine(root, Enum.GetName(typeof(NetFileType), this.type)
                , string.Join(@"\", this.paramArry), this.fileName).ToLower();
            }
        }


        public string GetFileUrl()
        {

            string baserUrl = System.Configuration.ConfigurationManager.AppSettings.Get("netfilebaseurl");

            if (string.IsNullOrEmpty(baserUrl))
            {
                baserUrl = $"{URI.Scheme}://bvn.erp.b1b.com";
            }

            string root = GetRoot();
            return string.Join("/", baserUrl, this.key, Enum.GetName(typeof(NetFileType), this.type)
                , this.paramArry.Length > 0 ? string.Join("/", this.paramArry) + "/" + fileName : fileName).ToLower();
        }
        /// <summary>
        /// 图片站的头
        /// weipan@20160218  解决图片站url被写死的问题
        /// </summary>
        /// <param name="UrlBase">例如: bvn.erp.b1b.com</param>
        /// <returns></returns>
        public string GetFileUrl(string UrlBase)
        {
            string root = GetRoot();
            return string.Join("/", $"{URI.Scheme}://{UrlBase}", this.key, Enum.GetName(typeof(NetFileType), this.type)
                , this.paramArry.Length > 0 ? string.Join("/", this.paramArry) + "/" + fileName : fileName).ToLower();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="httpPostedFile">上传文件</param>
        /// <returns>保存的文件名称
        /// [0]文件的物理路径
        /// [1]文件的网络url
        /// </returns>
        public string[] Save(HttpPostedFile httpPostedFile)
        {
            string fullName = this.GetFileFullName();
            FileInfo last = new FileInfo(fullName);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }
            httpPostedFile.SaveAs(fullName);
            return new[] { fullName, this.GetFileUrl() };
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="httpPostedFile">上传文件</param>
        /// <returns>保存的文件名称
        /// [0]文件的物理路径
        /// [1]文件的网络url
        /// </returns>
        public string[] Save(HttpPostedFile httpPostedFile, string UrlBase)
        {
            string fullName = this.GetFileFullName();
            FileInfo last = new FileInfo(fullName);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }
            httpPostedFile.SaveAs(fullName);
            return new[] { fullName, this.GetFileUrl(UrlBase) };
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>保存的文件名称
        /// [0]文件的物理路径
        /// [1]文件的网络url
        /// </returns>
        public string[] Save(Stream stream)
        {
            string fullName = this.GetFileFullName();
            FileInfo last = new FileInfo(fullName);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }
            using (StreamWriter sw = new StreamWriter(fullName, false))
            {
                stream.CopyTo(sw.BaseStream);
                sw.Flush();
                sw.Close();
            }
            return new[] { fullName, this.GetFileUrl() };
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns>保存的文件名称
        /// [0]文件的物理路径
        /// [1]文件的网络url
        /// </returns>
        public string[] Save(Stream stream, string UrlBase)
        {
            string fullName = this.GetFileFullName();
            FileInfo last = new FileInfo(fullName);
            //确保所在文件夹存在
            if (!last.Directory.Exists)
            {
                last.Directory.Create();
            }
            using (StreamWriter sw = new StreamWriter(fullName, false))
            {
                stream.CopyTo(sw.BaseStream);
                sw.Flush();
                sw.Close();
            }
            return new[] { fullName, this.GetFileUrl(UrlBase) };
        }
        private static string GetRoot()
        {
            string root = System.Configuration.ConfigurationManager.AppSettings[rootName];
            if (string.IsNullOrWhiteSpace(root))
            {
                DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                root = string.Concat(di.Root.FullName, rootName);
            }
            return root;
        }
    }
}
