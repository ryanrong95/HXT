using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class FrequencyConfig
    {
        static object locker = new object();
        static FrequencyConfig current;
        public List<ConfigModel> WarningFrequency { get; set; }

        private FrequencyConfig()
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json\\Config\\Config.json");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    this.WarningFrequency = JsonConvert.DeserializeObject<List<ConfigModel>>(jsonObject["Configuration"].ToString());
                }
            }
        }

        public static FrequencyConfig Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new FrequencyConfig();
                        }
                    }
                }
                return current;
            }
        }

    }
}
