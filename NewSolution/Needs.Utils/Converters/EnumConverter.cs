using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Utils.Converters
{
    public static class ________enum______ajsdjf______
    {
        public static T ToEnum<T>(this string name) where T : struct
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException($"{nameof(name)}不能为null或空字符");
            }

            T outer;
            if (Enum.TryParse(name, out outer))
            {
                return outer;
            }

            throw new Exception($"无法将[{name}]转换为{typeof(T).FullName}枚举");
        }
    }
}
