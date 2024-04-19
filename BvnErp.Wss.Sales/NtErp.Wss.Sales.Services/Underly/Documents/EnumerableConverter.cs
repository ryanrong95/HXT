

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class EnumerableConverter : JsonConverter
    {
        Type arryType;
        public override bool CanConvert(Type objectType)
        {
            var interfaceType = objectType.GetInterface(typeof(Underly.IForSerializers<>).Name);
            if (interfaceType != null)
            {
                this.arryType = interfaceType.GenericTypeArguments[0].MakeArrayType();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (this.CanConvert(objectType))
            {
                var arry = serializer.Deserialize(reader, arryType);
                var doc = Activator.CreateInstance(objectType);
                var method = objectType.GetMethod("Add", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new[] { arryType }, null);
                method.Invoke(doc, new[] { arry });
                return doc;
            }

            throw new NotSupportedException($"The type:{objectType.FullName} is not supported!");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (this.CanConvert(value.GetType()))
            {
                writer.WriteStartArray();
                foreach (object field in value as IEnumerable)
                {
                    serializer.Serialize(writer, field);
                }
                writer.WriteEndArray();
            }
        }
    }

}
