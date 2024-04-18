using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;

namespace Needs.Underly
{
    public class DocumentJsonConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Document)) || objectType == typeof(Document);
        }

        public DocumentJsonConvert()
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!this.CanConvert(objectType))
            {
                throw new Exception($"DocumentJsonConvert 对于类型[{objectType.FullName}]不可用");
            }
            //Document value = new Document();
            var value = Activator.CreateInstance(objectType, false) as Document;
            if (reader.TokenType == JsonToken.StartObject)
            {
                this.Read(reader, value, existingValue, serializer);
            }
            return value;
        }
        string currentPropertyName;
        void Read(JsonReader reader, Document document, object existingValue, JsonSerializer serializer)
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        {
                            Document sub = document[currentPropertyName];
                            Read(reader, sub, existingValue, serializer);
                        }
                        break;
                    case JsonToken.EndObject:
                        {
                            return;
                        }
                    case JsonToken.StartConstructor:
                        break;
                    case JsonToken.EndConstructor:
                        break;

                    //属性开始
                    case JsonToken.PropertyName:
                        currentPropertyName = reader.Value as string;
                        break;
                    case JsonToken.StartArray:
                        {
                            List<dynamic> list = new List<dynamic>();

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    document[currentPropertyName] = list.ToArray();
                                    break;
                                }
                                list.Add(reader.Value);
                            }
                        }
                        break;
                    case JsonToken.EndArray:
                        throw new NotImplementedException($"The specified type:({reader.TokenType}) is not implemented!");

                    case JsonToken.Null:
                        break;
                    case JsonToken.Undefined:
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.Raw:
                        break;
                    case JsonToken.Bytes:
                        break;
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Date:
                        {
                            var value = reader.Value;
                            if (string.IsNullOrWhiteSpace(currentPropertyName))
                            {
                                throw new Exception($"There is no value:({value}) for the specified property!");
                            }
                            document[currentPropertyName] = value;
                        }
                        break;
                    case JsonToken.None:
                    default:
                        throw new NotImplementedException($"The specified type:({reader.TokenType}) is not implemented!");
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteStartObject();
            this.Write(writer, value as Document, serializer);
            writer.WriteEndObject();
        }
        void Write(JsonWriter writer, Document value, JsonSerializer serializer)
        {
            foreach (var item in this.GetDictionary(value))
            {
                writer.WritePropertyName(item.Key);
                if (item.Value is Document)
                {

                    writer.WriteStartObject();
                    Write(writer, item.Value as Document, serializer);
                    writer.WriteEndObject();
                }
                else if (item.Value == null || item.Value is string || item.Value.GetType().IsValueType && !(item.Value is KeyValuePair<string, object>))
                {
                    writer.WriteValue(item.Value);
                }
                else
                {
                    writer.WriteRawValue(item.Value.Json());
                }
            }
        }

        Dictionary<string, object> GetDictionary(Document value)
        {
            return typeof(Document).GetField("dictionary", BindingFlags.Instance
                    | BindingFlags.NonPublic).GetValue(value) as Dictionary<string, object>;
        }
    }
}
