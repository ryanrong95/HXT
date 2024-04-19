using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Utils.Convertibles
{
    static public class SerializerConvert
    {
        static public object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string, true);
                else
                    return Enum.ToObject(type, value);
            }
            if (value.GetType() == typeof(Guid) && type == typeof(string))
            {
                return value.ToString();
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            if (value is string && type == typeof(bool) && string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }
            return Convert.ChangeType(value, type);
        }
    }
}
