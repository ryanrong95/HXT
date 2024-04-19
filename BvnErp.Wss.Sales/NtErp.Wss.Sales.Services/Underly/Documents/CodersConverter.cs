

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NtErp.Wss.Sales.Services.Utils.Converters;
using NtErp.Wss.Sales.Services.Utils.Convertibles;

namespace NtErp.Wss.Sales.Services.Underly
{
    public abstract class JsonConverterBase : JsonConverter
    {
        protected class Names
        {
            internal const string source = nameof(source);
            internal const string district = nameof(district);
        }

        protected FieldInfo GetField(Type type, string name)
        {
            var flag = BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo fi = type.GetField(name, flag);
            if (fi == null && type.BaseType != typeof(object))
            {
                fi = type.BaseType.GetField(name, flag);
            }
            return fi;
        }

        protected Type GetBaseType(Type type)
        {
            if (type.BaseType == typeof(object))
            {
                return type;
            }
            else
            {
                return GetBaseType(type.BaseType);
            }
        }
    }
    public class CodersConverter : JsonConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            return this.GetBaseType(objectType).Name == typeof(Underly.Products.Coding.Coders<>).Name;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (this.CanConvert(objectType))
            {
                var result = Activator.CreateInstance(objectType);
                if (reader.TokenType == JsonToken.StartObject)
                {
                    var source = this.GetField(objectType, Names.source);
                    var district = this.GetField(objectType, Names.district);
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartArray && reader.Path == Names.source.FirstUpper())
                        {
                            var list = serializer.Deserialize(reader, source.FieldType);
                            source.SetValue(result, list);
                        }

                        if (reader.TokenType == JsonToken.String && reader.Path == Names.district.FirstUpper())
                        {
                            district.SetValue(result, reader.Value.ChangeType(district.FieldType));
                        }
                        if (reader.TokenType == JsonToken.EndObject)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var source = this.GetField(objectType, Names.source);
                    var list = serializer.Deserialize(reader, source.FieldType);
                    source.SetValue(result, list);
                }

                return result;
            }

            throw new NotSupportedException($"The type:{objectType.FullName} is not supported!");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (this.CanConvert(value.GetType()))
            {
                writer.WriteStartObject();
                writer.WritePropertyName(Names.source.FirstUpper());
                writer.WriteStartArray();
                foreach (object field in value as IEnumerable)
                {
                    serializer.Serialize(writer, field);
                }
                writer.WriteEndArray();
                writer.WritePropertyName(Names.district.FirstUpper());
                writer.WriteValue(this.GetField(value.GetType(), Names.district).GetValue(value).ToString());
                writer.WriteEndObject();
            }
        }
    }
}
