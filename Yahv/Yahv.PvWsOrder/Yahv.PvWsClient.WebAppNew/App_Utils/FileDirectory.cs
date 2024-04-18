using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
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
        private readonly string BaseDirectory;

        /// <summary>
        /// 项目文件根目录
        /// </summary>
        public readonly string RootFileFolder = "Files";

        /// <summary>
        /// 子文件夹
        /// </summary>
        private string ChildFolder;

        /// <summary>
        /// 文件名
        /// </summary>
        private string FileName;

        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalFilePath
        {
            get;
            private set;
        }

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
        /// 获取文件的虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get;
            private set;
        }

        public string RootDirectory { get; private set; }

        public FileDirectory()
        {
            this.DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
            this.FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            this.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.FilePath = this.BaseDirectory + this.RootFileFolder;
            this.RootDirectory = this.BaseDirectory + this.RootFileFolder;
            this.LocalFilePath = System.Configuration.ConfigurationManager.AppSettings["LocalFileServerUrl"];
        }

        /// <summary>
        /// 返回当前实例
        /// </summary>
        /// <param name="fileName">文件名</param>
        public FileDirectory(string fileName) : this()
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// 设置子文件夹
        /// </summary>
        /// <param name="subDirectory"></param>
        public FileDirectory SetChildFolder(string subDirectory)
        {
            if (string.IsNullOrEmpty(subDirectory))
            {
                throw new Exception("不可创建名称为空的文件夹");
            }
            this.ChildFolder = subDirectory;
            this.VirtualPath += subDirectory;
            this.FilePath += @"\" + subDirectory;
            return this;
        }

        /// <summary>
        /// 按日期创建文件夹
        /// 格式：201902/01，201902/28
        /// </summary>
        public FileDirectory CreateDateDirectory()
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

            return this;
        }

        /// <summary>
        /// 获取文件Url地址
        /// </summary>
        /// <returns></returns>
        public string FileUrl
        {
            get
            {
                return this.FileServerUrl + "/" + this.VirtualPath.ToUrl();
            }
        }

        /// <summary>
        /// 获取本地文件Url地址
        /// </summary>
        public string LocalFileUrl
        {
            get
            {
                return this.LocalFilePath + "/" + this.VirtualPath.ToUrl();
            }
        }
    }

    public class SysConfig
    {
        /// <summary>
        /// 系统文件目录 （即将作废）
        /// </summary>
        [Obsolete]
        public const string FileDirectory = "Files";

        /// <summary>
        /// 报关单电子单据文件目录
        /// </summary>
        public const string DeclareDirectory = "DeclareEdoc";

        /// <summary>
        /// 报关单电子单据文件目录
        /// </summary>
        public const string DecMessageDirectory = "DecMessage";

        /// <summary>
        /// 报关单据压缩文件目录
        /// </summary>
        public const string ZipFiles = "ZipFiles";

        /// <summary>
        /// 下载的文件目录
        /// </summary>
        public const string Dowload = "Dowload";

        /// <summary>
        /// 导入模板的文件目录
        /// </summary>
        public const string Import = "Import";

        /// <summary>
        /// 订单文件目录
        /// </summary>
        public const string Order = "Order";

        ///// <summary>
        ///// 报关单报文根目录
        ///// </summary>
        //public const string DeclareMessageRootPath = @"D:\ImpPath\Deccus001\";

        ///// <summary>
        ///// 舱单报文根目录
        ///// </summary>
        //public const string ManifestMessageRootPath = @":\ImpPath\Rmft\";

        /// <summary>
        /// 报文发送文件目录
        /// </summary>
        public const string MeaasgeFolder = "OutBox";

        /// <summary>
        /// 报关单文件目录
        /// </summary>
        public const string DecHead = "DecHead";

        /// <summary>
        /// 客户文件目录
        /// </summary>
        public const string Client = "Client";

        /// <summary>
        /// 库房文件目录
        /// </summary>
        public const string Warehouse = "Warehouse";

        /// <summary>
        /// 付汇文件目录
        /// </summary>
        public const string PayExchange = "PayExchange";

        /// <summary>
        /// 换汇文件目录
        /// </summary>
        public const string SwapFile = "SwapFile";
        /// <summary>
        /// 换汇文件的压缩目录
        /// </summary>
        public const string ZipSwapFile = "ZipSwapFile";

        /// <summary>
        /// 财务收付款附件目录
        /// </summary>
        public const string Finance = "Finance";

        /// <summary>
        /// 运输批次附件目录
        /// </summary>
        public const string Voyage = "Voyage";

        /// <summary>
        /// 导出模板路径
        /// </summary>

        public const string ExportUrl = "Content\\templates\\进口付汇报关单清单模板(农业).xlsx";

        public const string ExportXingZhanUrl = "Content\\templates\\进口付汇报关单清单模板(星展).xlsx";
        /// <summary>
        /// 导出文件路径
        /// </summary>

        public const string Export = "Export";

        public const string ExcelDeclareFileName = "Content\\templates\\报关单模板单一窗口.xlsx";
        /// <summary>
        /// 导出每日报关数据
        /// </summary>

        // public const string ExcelDailyDeclare = "Content\\templates\\每日报关导出模板.xlsx";
        /// <summary>
        /// 删除报文
        /// </summary>
        public const string MessageDelete = "03";

        public const string Postfix = ".zip";
    }
}