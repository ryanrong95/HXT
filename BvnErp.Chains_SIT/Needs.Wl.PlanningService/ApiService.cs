using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    /// <summary>
    /// 接口服务入口
    /// </summary>
    public partial class ApiService
    {
        static object locker = new object();
        static ApiService current;

        private ApiClients clients;
        public ApiClients Clients
        {
            get
            {
                if (this.clients == null)
                {
                    this.clients = new ApiClients();
                }
                return this.clients;
            }
            set { this.clients = value; }
        }

        private ApiSettings apiSettings;
        public ApiSettings ApiSettings
        {
            get
            {
                if (this.apiSettings == null)
                {
                    this.apiSettings = new ApiSettings();
                }
                return this.apiSettings;
            }
            set { this.apiSettings = value; }
        }

        private ApiOrderCompanies orderCompanies;

        public ApiOrderCompanies OrderCompanies
        {
            get
            {
                if (this.orderCompanies == null)
                {
                    this.orderCompanies = new ApiOrderCompanies();
                }
                return this.orderCompanies;
            }
            set { this.orderCompanies = value; }
        }

        private ApiService()
        {
            string env = System.Configuration.ConfigurationManager.AppSettings["ApiEnv"];

            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json\\" + env + "\\client.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    this.Clients = JsonConvert.DeserializeObject<ApiClients>(jsonObject["Clients"].ToString());
                }
            }

            //加载Api
            foreach (ApiClient client in this.Clients)
            {
                jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json\\" + env + "\\" + client.Key + ".json");
                using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                        ApiSetting apiSetting = jsonObject.ToObject<ApiSetting>();
                        this.ApiSettings.Add(apiSetting);
                    }
                }
            }

            var companyJsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json\\" + env + "\\Company.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(companyJsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    this.OrderCompanies = JsonConvert.DeserializeObject<ApiOrderCompanies>(jsonObject["Companies"].ToString());
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public static ApiService Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ApiService();
                        }
                    }
                }

                return current;
            }
        }
    }
}