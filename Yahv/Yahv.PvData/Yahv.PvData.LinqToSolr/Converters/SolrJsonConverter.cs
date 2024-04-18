using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Converters
{
    public class SolrJsonConverter : JsonConverter
    {
        private PropertyInfo[] properties;

        public override bool CanConvert(Type objectType)
        {
            var members = objectType.GetProperties().Where(x => x.GetCustomAttributes(typeof(Attributes.SolrFieldAttribute), true).Any()).ToList();
            if (members.Any())
            {
                properties = members.ToArray();
                return true;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = Activator.CreateInstance(objectType);
            var json = JObject.Load(reader);
            serializer.Error += (sender, e) => e.ErrorContext.Handled = true;

            if (reader.TokenType == JsonToken.EndObject)
            {
                foreach (var prop in properties)
                {
                    var attr = prop.GetCustomAttribute<Attributes.SolrFieldAttribute>();
                    string propName = attr?.Name ?? prop.Name;
                    if (json[propName] != null)
                    {
                        prop.SetValue(obj, JsonConvert.DeserializeObject(json[propName].Value<string>(), prop.PropertyType), null);
                    }
                }
            }

            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
