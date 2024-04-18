using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public class InnerCompanyContext<T> where T : class, new()
    {
        public static List<T> Current
        {
            get
            {
                var name = System.Configuration.ConfigurationManager.AppSettings[typeof(T).Name];
                return GetParty(name);
            }
        }

        static List<T> GetParty(string name)
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\jsons\\" + name + ".json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    var result = JsonConvert.DeserializeObject<List<T>>(jsonObject["Companies"].ToString()); 
                    return result;
                }
            }
        }
    }

    public sealed class AgentCompanyContext : InnerCompanyContext<InnerCompany>
    {

    }

    public sealed class SingleOwnerCompanyContext : InnerCompanyContext<SingleOwnerCompany>
    {

    }

    public class InnerCompany
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class SingleOwnerCompany
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

}
