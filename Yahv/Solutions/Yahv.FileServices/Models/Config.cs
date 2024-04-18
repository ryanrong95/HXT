using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Yahv.Utils.Serializers;

namespace Yahv.FileServices.Models
{
    public static class UploadConfig
    {
        public static Config[] Configs = File.ReadAllText(HttpContext.Current.Server.MapPath("/App_Data/Config.json")).JsonTo<Models.Config[]>();

        public static string FileUploadPath = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
        public static string FileUploadUrl = System.Configuration.ConfigurationManager.AppSettings["FileUploadUrl"];

    }

    public class Config
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public TypeSize[] TypeSizes { get; set; }
    }

    public class TypeSize
    {
        /// <summary>
        /// 上传的文件类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 上传的文件大小(单位：k）
        /// </summary>
        public decimal FileSize { get; set; }
    }
}