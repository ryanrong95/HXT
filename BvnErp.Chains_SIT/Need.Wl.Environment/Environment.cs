using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Needs.Wl.Environment
{
    /// <summary>
    /// 当前系统环境
    /// </summary>
    public class Environment
    {
        private static readonly object locker = new object();
        private static Environment current;

        /// <summary>
        /// 深圳进口报关公司
        /// 代理报关公司
        /// </summary>
        public Models.Company Company;

        /// <summary>
        /// 香港收货公司
        /// icgoo等特殊公司，根据公司类型重载
        /// </summary>
        public Models.Company HongKongCompany;

        /// <summary>
        /// 联系人
        /// </summary>
        public Models.Contact Contact;

        /// <summary>
        /// 前缀
        /// </summary>
        public Models.PreFix PreFix;

        private Environment()
        {
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];

            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Environment\\Json\\" + environment + ".json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    //this.Clients = JsonConvert.DeserializeObject<ApiClients>(jsonObject["Clients"].ToString());
                }
            }
        }

        public static Environment Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Environment();
                        }
                    }
                }

                return current;
            }
        }
    }
}