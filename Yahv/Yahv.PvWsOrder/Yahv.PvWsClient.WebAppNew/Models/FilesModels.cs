using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    /// <summary>
    /// 文件对象
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string fileFormat { get; set; }

        /// <summary>
        /// 文件URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 文件的网络地址
        /// </summary>
        public string fullURL { get; set; }

    }


    /// <summary>
    /// 请求响应数据
    /// </summary>
    public class ResponseData
    {
        public string code { get; set; }
        public string data { get; set; }
        public string success { get; set; }

        public string message { get; set; }

        public string url { get; set; }
    }
}