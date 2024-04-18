using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    /// <summary>
    /// 接口服务入口
    /// </summary>
    public partial class Tool
    {
        static object locker = new object();
        static Tool current;

        /// <summary>
        /// 当前公司
        /// </summary>
        private Company compamy;
        public Company Company
        {
            get
            {
                if (this.compamy == null)
                {
                    this.compamy = new Company();
                }
                return this.compamy;
            }
            set { this.compamy = value; }
        }

        /// <summary>
        /// 基础文件路径
        /// </summary>
        private Folder folder;
        public Folder Folder
        {
            get
            {
                if (this.folder == null)
                {
                    this.folder = new Folder();
                }
                return this.folder;
            }
            set { this.folder = value; }
        }

        private Tool()
        {
            var key = System.Configuration.ConfigurationManager.AppSettings["Company"];
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs" + "\\" + key + ".json");
            using (StreamReader file = new StreamReader(jsonPath, Encoding.Default))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    this.Company = jsonObject.ToObject<Company>();
                }
            }
            this.Folder.DecMainFolder = System.Configuration.ConfigurationManager.AppSettings["DecMainFolder"]; 
            this.Folder.RmftMainFolder = System.Configuration.ConfigurationManager.AppSettings["RmftMainFolder"];
            this.Folder.OtherMainFolder = System.Configuration.ConfigurationManager.AppSettings["OtherMainFolder"];
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public static Tool Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Tool();
                        }
                    }
                }

                return current;
            }
        }

    }
}