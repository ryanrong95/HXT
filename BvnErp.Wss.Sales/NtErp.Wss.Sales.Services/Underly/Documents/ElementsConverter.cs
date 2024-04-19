

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtErp.Wss.Sales.Services.Utils.Convertibles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class ElementsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Document)) || objectType == typeof(Document);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (this.CanConvert(objectType))
            {
                var doc = Activator.CreateInstance(objectType) as IDocument<Elements>;
                this.ReadJson(reader, doc);


                return doc;
            }
            else
            {
                throw new NotSupportedException($"The type:{objectType.FullName} is not supported!");
            }
        }

        public void ReadJson(JsonReader reader, IDocument<Elements> doc)
        {
            object tValue = "";
            ArrayList arrayList = null;
            Type valueType = null;


            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        //开始递归
                        this.ReadJson(reader, doc[tValue.ToString()]);
                        break;

                    case JsonToken.None:
                    case JsonToken.StartConstructor:
                    case JsonToken.EndConstructor:
                        throw new NotImplementedException("Please contact Chen Han!");

                    case JsonToken.StartArray:
                        arrayList = new ArrayList();
                        break;
                    case JsonToken.EndArray:
                        doc[tValue.ToString()] = new DValue(arrayList.ToArray(valueType));
                        arrayList = null;
                        break;

                    case JsonToken.PropertyName:
                        tValue = reader.Value;
                        break;

                    case JsonToken.Raw:
                        break;
                    case JsonToken.Boolean:
                    case JsonToken.String:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        if (arrayList == null)
                        {
                            doc[tValue.ToString()] = new DValue(reader.Value.ChangeType(reader.ValueType));
                        }
                        else
                        {
                            valueType = reader.ValueType;
                            arrayList.Add(reader.Value.ChangeType(reader.ValueType));
                        }
                        break;
                    case JsonToken.Comment:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                        //就是要 ignore
                        break;
                    case JsonToken.EndObject:
                        //结束递归
                        return;
                    default:
                        break;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (this.CanConvert(value.GetType()))
            {
                this.WriteJson(writer, value as IDocument<Elements>);
            }
            else
            {
                throw new NotSupportedException($"The type:{value.GetType().FullName} is not supported!");
            }
        }

        void WriteJson(JsonWriter writer, IDocument<Elements> doc)
        {
            writer.WriteStartObject();

            foreach (var element in doc)
            {
                if (element.Count == 0)
                {
                    if (element.Value == null)
                    {
                        break;
                    }
                    writer.WritePropertyName(element.Name);
                    if (element.Value is Underly.Document)
                    {
                        WriteJson(writer, element.Value as Document);
                    }
                    else if (element.Value is IEnumerable && !(element.Value is string))
                    {
                        writer.WriteStartArray();
                        if (element.Value is Document[])
                        {
                            foreach (var item in element.Value as Underly.Document[])
                            {
                                WriteJson(writer, item);
                            }
                        }
                        else
                        {
                            foreach (var item in element.Value as IEnumerable)
                            {
                                writer.WriteValue(item);
                            }
                        }
                        writer.WriteEndArray();

                    }
                    else
                    {
                        writer.WriteValue(element.Value);
                    }
                }
                else
                {
                    writer.WritePropertyName(element.Name);
                    WriteJson(writer, doc[element.Name]);
                }
            }

            writer.WriteEnd();
        }
    }
}
