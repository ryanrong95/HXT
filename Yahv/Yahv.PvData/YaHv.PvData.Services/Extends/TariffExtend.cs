using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace YaHv.PvData.Services.Extends
{
    public static class TariffExtend
    {
        public static Dictionary<string, string> ElementFormatter(this Models.Tariff tariff, string element)
        {
            string[] formatArr = tariff.DeclareElements.Split(';');
            string[] elementArr = element.Split('|');
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int i = 0; i < formatArr.Length; i++)
            {
                dic.Add(formatArr[i], elementArr[i]);
            }
            if (elementArr.Length > formatArr.Length)
            {
                dic.Add(elementArr.Length + ":其他", elementArr[formatArr.Length]);
            }

            return dic;
        }
    }
}
